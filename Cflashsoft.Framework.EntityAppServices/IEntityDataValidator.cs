using Cflashsoft.Framework.AppServices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cflashsoft.Framework.Types;

namespace Cflashsoft.Framework.EntityAppServices
{
    /// <summary>
    /// Provides the ability to implement custom data validation for Entity Framework entities before changes are committed to a store.
    /// </summary>
    public interface IEntityDataValidator<T, TDbContext> where T : EntityDataContext<TDbContext> where TDbContext : DbContext, new()
    {
        /// <summary>
        /// Returns true if this validator can assert authorizations on changed entities.
        /// </summary>
        bool CanAssertAuthorizations { get; }

        /// <summary>
        /// Returns true if this validator can perform validation.
        /// </summary>
        bool CanValidate { get; }

        /// <summary>
        /// Returns true if this validator can perform validation using data annotation attributes.
        /// </summary>
        bool CanValidateUsingDataAnnotations { get; }

        /// <summary>
        /// Returns true if this validator can perform auditing.
        /// </summary>
        bool CanAudit { get; }

        /// <summary>
        /// Check changed entities and assert authorizations.
        /// </summary>
        void AssertAuthorizations(AppPrincipal contextPrincipal, AppContextBase appContext, T dataContext);

        /// <summary>
        /// Validate the data in the specified entity framework context.
        /// </summary>
        bool Validate(AppPrincipal contextPrincipal, AppContextBase appContext, T dataContext, IList<ErrorResult> errors);

        /// <summary>
        /// Audit changes in the specified entity framework context.
        /// </summary>
        void Audit(AppPrincipal contextPrincipal, AppContextBase appContext, T dataContext);
    }
}
