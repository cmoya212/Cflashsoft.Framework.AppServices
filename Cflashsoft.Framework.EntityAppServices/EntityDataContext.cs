using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Cflashsoft.Framework.AppServices;
using Cflashsoft.Framework.Types;

namespace Cflashsoft.Framework.EntityAppServices
{
    /// <summary>
    /// Represents a wrapper for Entity Framework DbContext and used by UoW EntityAppServices classes to share a context.
    /// </summary>
    public class EntityDataContext<TDbContext> : DataContext<TDbContext>, IDisposable, ISaveable where TDbContext : DbContext, new()
    {
        private bool _disposed = false;
        private IEntityDataValidator<EntityDataContext<TDbContext>, TDbContext> _validator = null;
        private List<object> _lastAdded = new List<object>();
        private List<object> _lastModified = new List<object>();
        private List<object> _lastDeleted = new List<object>();

        /// <summary>
        /// Provides the ability to implement custom data validation for Entity Framework entities before changes are committed to a store.
        /// </summary>
        public static IEntityDataValidator<EntityDataContext<TDbContext>, TDbContext> DefaultValidator { get; set; }

        /// <summary>
        /// Provides the ability to implement custom data validation for Entity Framework entities before changes are committed to a store.
        /// </summary>
        protected IEntityDataValidator<EntityDataContext<TDbContext>, TDbContext> Validator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        /// <summary>
        /// Initializes the data context and creates the EF DbContext instance.
        /// </summary>
        public override void InitializeDataContext(AppContextBase appContext)
        {
            this.AppContext = appContext;

            bool? enableLazyLoading = this.AppContext.Options.ContainsKey("EF_EnableLazyLoading") ? (bool?)this.AppContext.Options["EF_EnableLazyLoading"] : null;
            bool? enableProxyCreation = this.AppContext.Options.ContainsKey("EF_EnableProxyCreation") ? (bool?)this.AppContext.Options["EF_EnableProxyCreation"] : null;
            IEntityDataValidator<EntityDataContext<TDbContext>, TDbContext> validator = this.AppContext.Options.ContainsKey("EF_EntityAppValidator") ? (IEntityDataValidator<EntityDataContext<TDbContext>, TDbContext>)this.AppContext.Options["EF_EntityAppValidator"] : DefaultValidator;

            this.Data = new TDbContext();

            if (enableLazyLoading.HasValue)
                this.Data.Configuration.LazyLoadingEnabled = enableLazyLoading.Value;
            if (enableProxyCreation.HasValue)
                this.Data.Configuration.ProxyCreationEnabled = enableProxyCreation.Value;

            _validator = validator;
        }

        /// <summary>
        /// Attach an entity to the context and mark it as added.
        /// </summary>
        public void AttachAsAdded<TEntity>(TEntity entity) where TEntity : class
        {
            AttachRangeAsAdded(new TEntity[] { entity });
        }

        /// <summary>
        /// Attach a set of entities to the context and mark them as added.
        /// </summary>
        public void AttachRangeAsAdded<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            AttachRange(entities, EntityState.Added);
        }

        /// <summary>
        /// Attach an entity to the context and mark it as modified.
        /// </summary>
        public void AttachedAsModified<TEntity>(TEntity entity) where TEntity : class
        {
            AttachRangeAsModified(new TEntity[] { entity });
        }

        /// <summary>
        /// Attach a set of entities to the context and mark them as modified.
        /// </summary>
        public void AttachRangeAsModified<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            AttachRange(entities, EntityState.Modified);
        }

        /// <summary>
        /// Attach an entity to the context and mark it as deleted.
        /// </summary>
        public void AttachAsDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            AttachAsDeleted(new TEntity[] { entity });
        }

        /// <summary>
        /// Attach a set of entities to the context and mark them as deleted.
        /// </summary>
        public void AttachRangeAsDeleted<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            AttachRange(entities, EntityState.Deleted);
        }

        /// <summary>
        /// Attach a range of entities to the context and mark them with the specified state.
        /// </summary>
        public void AttachRange<TEntity>(IEnumerable<TEntity> entities, EntityState state) where TEntity : class
        {
            foreach (TEntity entity in entities)
            {
                this.Data.Set<TEntity>().Attach(entity);
                this.Data.Entry(entity).State = state;
            }
        }

        /// <summary>
        /// Get all changed entities in the context.
        /// </summary>
        public IEnumerable<object> GetChanges()
        {
            return this.Data.ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Modified || p.State == EntityState.Added || p.State == EntityState.Deleted)
                .Select(p => p.Entity).AsEnumerable();
        }

        /// <summary>
        /// Get all changes to entities of the specified type in the context.
        /// </summary>
        public IEnumerable<TEntity> GetChanges<TEntity>() where TEntity : class
        {
            return this.Data.ChangeTracker.Entries<TEntity>()
                .Where(p => p.State == EntityState.Modified || p.State == EntityState.Added || p.State == EntityState.Deleted)
                .Select(p => p.Entity).AsEnumerable();
        }

        /// <summary>
        /// Get all changes to entities of the specified type in the context.
        /// </summary>
        public IEnumerable<object> GetChanges(Type type)
        {
            return this.Data.ChangeTracker.Entries()
                .Where(p => p.Entity.GetType() == type && (p.State == EntityState.Modified || p.State == EntityState.Added || p.State == EntityState.Deleted))
                .Select(p => p.Entity).AsEnumerable();
        }

        /// <summary>
        /// Get all changes to entities of the specified type in the context.
        /// </summary>
        public IEnumerable<object> GetChanges(IEnumerable<Type> types)
        {
            List<object> result = new List<object>();

            foreach (Type type in types)
            {
                result.AddRange(GetChanges(type));
            }

            return result;
        }

        /// <summary>
        /// Get a list of entities that were last added to the store.
        /// </summary>
        public IReadOnlyCollection<object> GetLastAdded()
        {
            return _lastAdded.AsReadOnly();
        }

        /// <summary>
        /// Get a list of entities that were last modified and sent to the store.
        /// </summary>
        public IReadOnlyCollection<object> GetLastModified()
        {
            return _lastModified.AsReadOnly();
        }

        /// <summary>
        /// Get a list of entities that were last deleted from the store.
        /// </summary>
        public IReadOnlyCollection<object> GetLastDeleted()
        {
            return _lastDeleted.AsReadOnly();
        }

        /// <summary>
        /// Get a list of entities that were last added, commited, or deleted from the store.
        /// </summary>
        public IReadOnlyCollection<object> GetAllLastChanged()
        {
            return _lastAdded
                .Union(_lastModified)
                .Union(_lastDeleted).ToList().AsReadOnly();
        }

        private void PrepareChangedCollections()
        {
            _lastAdded.Clear();
            _lastModified.Clear();
            _lastDeleted.Clear();

            _lastAdded.AddRange(this.Data.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).Select(p => p.Entity));
            _lastModified.AddRange(this.Data.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).Select(p => p.Entity));
            _lastDeleted.AddRange(this.Data.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted).Select(p => p.Entity));
        }

        /// <summary>
        /// Returns whether the current data context can currently be saved to the store.
        /// </summary>
        bool ISaveable.CanSave(AppPrincipal contextUser, IList<ErrorResult> errors)
        {
            bool result = false;

            if (_validator != null)
            {
                if (_validator.CanAssertAuthorizations)
                    _validator.AssertAuthorizations(contextUser, this.AppContext, this);

                if (_validator.CanValidate)
                    result = _validator.Validate(contextUser, this.AppContext, this, errors);
                else
                    result = true;
            }
            else
                result = true;

            return result;
        }

        /// <summary>
        /// Commits the current data context to the store.
        /// </summary>
        object ISaveable.Save(AppPrincipal contextUser)
        {
            object result = null;

            PrepareChangedCollections();

            result = this.Data.SaveChanges();

            if (_validator != null && _validator.CanAudit)
                _validator.Audit(contextUser, this.AppContext, this);

            return result;
        }

        /// <summary>
        /// Commits the current data context to the store.
        /// </summary>
        async Task<object> ISaveable.SaveAsync(AppPrincipal contextUser)
        {
            object result = null;

            PrepareChangedCollections();

            result = await this.Data.SaveChangesAsync();

            if (_validator != null && _validator.CanAudit)
                _validator.Audit(contextUser, this.AppContext, this);

            return result;
        }

        /// <summary>
        /// Releases all resources used by this data context.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this data context.
        /// </summary>
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DbContext data = this.Data;

                    if (data != null)
                    {
                        data.Dispose();
                        this.Data = null;
                    }
                }

                _disposed = true;
            }
        }
    }
}
