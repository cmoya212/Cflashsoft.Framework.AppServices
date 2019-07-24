using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Cflashsoft.Framework.AppServices;
using Cflashsoft.Framework.Types;
using System.ComponentModel.DataAnnotations;

namespace Cflashsoft.Framework.EntityCoreAppServices
{
    /// <summary>
    /// Represents a function that checks whether changes to an entity are authorized for the current user.
    /// </summary>
    public delegate bool IsChangeAuthorized(AppPrincipal contextPrincipal, AppContextBase appContext, DbContext dbContext, object entity);

    /// <summary>
    /// Represents a function that checks whether changes to an entity pass custom validations such as range checks and other complex conditions.
    /// </summary>
    public delegate bool IsChangeValid(AppPrincipal contextPrincipal, AppContextBase appContext, DbContext dbContext, object entity, IList<ErrorResult> errors);

    /// <summary>
    /// Defines an authorization function that can be used to determine whether changes to an entity are authorized.
    /// </summary>
    public struct Authorization
    {
        /// <summary>
        /// The type of entity represented by this authorization.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The authorize function that will be called.
        /// </summary>
        public IsChangeAuthorized Authorize { get; }

        /// <summary>
        /// Initializes a new instance of Authorization structure.
        /// </summary>
        public Authorization(Type type, IsChangeAuthorized authorize) { this.Type = type; this.Authorize = authorize; }
    }

    /// <summary>
    /// Defines a validation function that can be used to check whether changes to an entity are valid such as range checks and other complex conditions.
    /// </summary>
    public struct Validation
    {
        /// <summary>
        /// The type of entity represented by this validation.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The validate function that will be called.
        /// </summary>
        public IsChangeValid Validate { get; }

        /// <summary>
        /// Initializes a new instance of Validation structure.
        /// </summary>
        public Validation(Type type, IsChangeValid validate) { this.Type = type; this.Validate = validate; }
    }

    /// <summary>
    /// Provides the ability to implement custom data validation for Entity Framework entities before changes are committed to a store.
    /// </summary>
    public class EntityDataValidator<T, TDbContext> : IEntityDataValidator<T, TDbContext> where T : EntityDataContext<TDbContext> where TDbContext : DbContext, new()
    {
        /// <summary>
        /// List of authorization functions for changed entities.
        /// </summary>
        public List<Authorization> Authorizations { get; private set; }

        /// <summary>
        /// List of entities that are not allowed to be modified.
        /// </summary>
        public List<Type> ReadOnlyTypes { get; private set; }

        /// <summary>
        /// List of validation functions for changed entities.
        /// </summary>
        public List<Validation> Validations { get; private set; }

        /// <summary>
        /// List of entities that whose changes will be logged by an auditor.
        /// </summary>
        public Dictionary<Type, AppAuditItemInfo> AuditTypes { get; private set; }

        /// <summary>
        /// Returns true if this validator can assert authorizations on changed entities.
        /// </summary>
        public bool CanAssertAuthorizations => true;

        /// <summary>
        /// Returns true if this validator can perform validation.
        /// </summary>
        public bool CanValidate => true;

        /// <summary>
        /// Returns true if this validator can perform validation using data annotation attributes.
        /// </summary>
        public bool CanValidateUsingDataAnnotations => true;

        /// <summary>
        /// Returns true if this validator can perform auditing.
        /// </summary>
        public bool CanAudit => true;

        /// <summary>
        /// Initializes a new instance of EntityDataValidator class.
        /// </summary>
        public EntityDataValidator()
        {
            this.Authorizations = new List<Authorization>();
            this.ReadOnlyTypes = new List<Type>();
            this.Validations = new List<Validation>();
            this.AuditTypes = new Dictionary<Type, AppAuditItemInfo>();
        }

        /// <summary>
        /// Initializes a new instance of EntityDataValidator class.
        /// </summary>
        public EntityDataValidator(IEnumerable<Authorization> authorizations, IEnumerable<Type> readOnlyTypes, IEnumerable<Validation> validations, IDictionary<Type, AppAuditItemInfo> auditTypes)
        {
            this.Authorizations = authorizations != null ? authorizations.ToList() : new List<Authorization>();
            this.ReadOnlyTypes = readOnlyTypes != null ? readOnlyTypes.ToList() : new List<Type>();
            this.Validations = validations != null ? validations.ToList() : new List<Validation>();
            this.AuditTypes = auditTypes != null ? new Dictionary<Type, AppAuditItemInfo>(auditTypes) : new Dictionary<Type, AppAuditItemInfo>();
        }

        /// <summary>
        /// Check changed entities and assert authorizations.
        /// </summary>
        public void AssertAuthorizations(AppPrincipal contextPrincipal, AppContextBase appContext, T dataContext)
        {
            foreach (Authorization authorization in this.Authorizations)
                foreach (object entity in dataContext.GetChanges(authorization.Type))
                    appContext.AssertAuthorization(authorization.Authorize(contextPrincipal, appContext, dataContext.Data, entity));

            foreach (Type type in this.ReadOnlyTypes)
                appContext.AssertAuthorization(!dataContext.GetChanges(type).Any());
        }

        /// <summary>
        /// Validate the data in the specified entity framework context.
        /// </summary>
        public bool Validate(AppPrincipal contextPrincipal, AppContextBase appContext, T dataContext, IList<ErrorResult> errors)
        {
            bool result = true;

            if (this.CanValidateUsingDataAnnotations)
            {
                foreach (object entity in dataContext.GetChanges())
                {
                    var validatationContext = new ValidationContext(entity, serviceProvider: null, items: null);
                    var volidationContextResults = new List<ValidationResult>();

                    result = Validator.TryValidateObject(entity, validatationContext, volidationContextResults, true);

                    foreach (ValidationResult error in volidationContextResults)
                    {
                        errors.Add(new ErrorResult(ErrorResultKind.Validation, error.ErrorMessage));
                    }
                }
            }

            foreach (Validation validation in this.Validations)
                foreach (object entity in dataContext.GetChanges(validation.Type))
                    if (!validation.Validate(contextPrincipal, appContext, dataContext.Data, entity, errors))
                        result = false;

            return result;
        }

        /// <summary>
        /// Audit changes in the specified entity framework context.
        /// </summary>
        public void Audit(AppPrincipal contextPrincipal, AppContextBase appContext, T dataContext)
        {
            try
            {
                ItemIdentifier userIdentifier = new ItemIdentifier(contextPrincipal.UserInfo.Id, contextPrincipal.UserInfo.Oid, contextPrincipal.UserInfo.UserName);

                AppContextBase.AuditLogger.LogActions(AppAuditAction.Delete, 0, userIdentifier, this.AuditTypes, dataContext.GetLastDeleted(), null);
                AppContextBase.AuditLogger.LogActions(AppAuditAction.Add, 0, userIdentifier, this.AuditTypes, dataContext.GetLastAdded(), null);
                AppContextBase.AuditLogger.LogActions(AppAuditAction.Modify, 0, userIdentifier, this.AuditTypes, dataContext.GetLastModified(), null);
            }
            catch (Exception ex)
            {
                AppContextBase.ErrorLogger.LogError(ex, "Could not complete logging of audit actions.", 0, 0, contextPrincipal.UserInfo.Id, contextPrincipal.UserInfo.Oid, contextPrincipal.UserInfo.UserName);
                throw;
            }
        }
    }
}
