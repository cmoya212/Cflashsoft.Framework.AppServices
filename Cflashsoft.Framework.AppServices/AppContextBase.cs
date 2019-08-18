using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cflashsoft.Framework.Types;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// An AppContext instance represents Unit Of Work patterns that can encapsulate atomic business API methods. It is meant to hide Entity Framework DbContext and other backing data providers such as WCF. 
    /// </summary>
    public abstract class AppContextBase : IDisposable
    {
        private bool _disposed = false;
        internal ErrorLoggerBase _errorLogger = null;
        internal AuditLoggerBase _auditLogger = null;
        internal Action<Action> _backgroundAction = null;
        internal AppOptions _options = null;
        private object _reusableAppServicesSyncRoot = new object();
        private object _sharedObjectsSyncRoot = new object();
        internal OptionsDictionary<Type, AppServicesBase> _reusableAppServices = new OptionsDictionary<Type, AppServicesBase>();
        private OptionsDictionary<Type, SharedObjectEntry> _sharedObjects = new OptionsDictionary<Type, SharedObjectEntry>();
        private OptionsDictionary<Type, object> _lastSaveResults = new OptionsDictionary<Type, object>();
        private List<ErrorResult> _lastSaveErrors = new List<ErrorResult>();

        /// <summary>
        /// When overridden in derived classes, returns a list of allowed AppServices that can be created in this AppContext. Returns null if there is no constraint.
        /// </summary>
        protected abstract HashSet<Type> AllowedAppServiceTypes { get; }

        /// <summary>
        /// A collection of custom options that can be used by AppService UoW classes for application specific purposes.
        /// </summary>
        public AppOptions Options => _options;

        /// <summary>
        /// Provides logging services. NOTE: this error logger is static and does not share a state or connections with the app context.
        /// </summary>
        public ErrorLoggerBase ErrorLogger => _errorLogger;

        /// <summary>
        /// Provides audit logging services. NOTE: this audit logger is static and does not share a state or connections with the app context.
        /// </summary>
        public AuditLoggerBase AuditLogger => _auditLogger;

        /// <summary>
        /// Inititializes a new instance of the AppContext class.
        /// </summary>
        public AppContextBase()
            : this(null, null, null, new AppOptions())
        { }

        /// <summary>
        /// Inititializes a new instance of the AppContext class.
        /// </summary>
        public AppContextBase(ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, AppOptions options)
        {
            _errorLogger = errorLogger ?? new TraceErrorLogger();
            _auditLogger = auditLogger ?? new TraceAuditLogger();
            _backgroundAction = backgroundAction != null ? backgroundAction : (Action action) => ThreadPool.QueueUserWorkItem(a => action());
            _options = options ?? new AppOptions();
        }

        /// <summary>
        /// Inititializes a new instance of the AppContext class.
        /// </summary>
        /// <param name="errorLogger">The default stateless error logger that can be used by app services and the parent application to log errors. If null, the default error logger will output to the console or trace listeners.</param>
        /// <param name="auditLogger">The default stateless audit logger that can be used by app services and the parent application to log an audit trail. If null, the default audit logger will output to the console or trace listeners.</param>
        /// <param name="backgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        public AppContextBase(ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, params KeyValuePair<string, object>[] options)
            : this(errorLogger, auditLogger, backgroundAction, new AppOptions())
        {
            foreach (KeyValuePair<string, object> option in options)
            {
                _options.Add(option);
            }
        }

        /// <summary>
        /// Inititializes a new instance of the AppContext class.
        /// </summary>
        /// <param name="errorLogger">The default stateless error logger that can be used by app services and the parent application to log errors. If null, the default error logger will output to the console or trace listeners.</param>
        /// <param name="auditLogger">The default stateless audit logger that can be used by app services and the parent application to log an audit trail. If null, the default audit logger will output to the console or trace listeners.</param>
        /// <param name="backgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        public AppContextBase(ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, params IEnumerable<KeyValuePair<string, object>>[] optionsLists)
            : this(errorLogger, auditLogger, backgroundAction, new AppOptions())
        {
            foreach (IEnumerable<KeyValuePair<string, object>> optionsList in optionsLists)
            {
                foreach (KeyValuePair<string, object> option in optionsList)
                {
                    _options.Add(option);
                }
            }
        }

        /// <summary>
        /// Inititializes a new instance of the AppContext class.
        /// </summary>
        /// <param name="errorLogger">The default stateless error logger that can be used by app services and the parent application to log errors. If null, the default error logger will output to the console or trace listeners.</param>
        /// <param name="auditLogger">The default stateless audit logger that can be used by app services and the parent application to log an audit trail. If null, the default audit logger will output to the console or trace listeners.</param>
        /// <param name="backgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        public AppContextBase(ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, IEnumerable<KeyValuePair<string, object>> options)
            : this(errorLogger, auditLogger, backgroundAction, new AppOptions(options))
        { }

        /// <summary>
        /// Create a new UoW app service in this context.
        /// </summary>
        public T GetAppService<T>() where T : AppServicesBase, new()
        {
            Type appServiceType = typeof(T);

            if (this.AllowedAppServiceTypes != null && !this.AllowedAppServiceTypes.Contains(appServiceType))
                throw new InvalidOperationException("App service of this type is not allowed in this app context.");

            T result = null;

            ReusableInContextAppServiceAttribute reusableInContextAttribute = Attribute.GetCustomAttribute(appServiceType, typeof(ReusableInContextAppServiceAttribute)) as ReusableInContextAppServiceAttribute;

            if (reusableInContextAttribute != null)
            {
                result = _reusableAppServices[appServiceType] as T;

                if (result == null)
                {
                    lock (_reusableAppServicesSyncRoot)
                    {
                        result = _reusableAppServices[appServiceType] as T;

                        if (result == null)
                        {
                            result = new T();
                            _reusableAppServices.Add(appServiceType, result);
                            result.InitializeAppService(this);
                        }
                    }
                }
            }
            else
            {
                result = new T();
                result.InitializeAppService(this);
            }

            return result;
        }

        /// <summary>
        /// Create a new UoW app service in this context.
        /// </summary>
        public AppServicesBase GetAppService(Type appServiceType)
        {
            if (this.AllowedAppServiceTypes != null && !this.AllowedAppServiceTypes.Contains(appServiceType))
                throw new InvalidOperationException("App service of this type is not allowed in this app context.");

            AppServicesBase result = null;

            if (appServiceType.IsInterface)
            {
                result = _reusableAppServices[appServiceType];
            }
            else
            {
                if (appServiceType.IsAssignableFrom(typeof(AppServicesBase)))
                {
                    ReusableInContextAppServiceAttribute reusableInContextAttribute = Attribute.GetCustomAttribute(appServiceType, typeof(ReusableInContextAppServiceAttribute)) as ReusableInContextAppServiceAttribute;

                    if (reusableInContextAttribute != null)
                    {
                        result = _reusableAppServices[appServiceType];

                        if (result == null)
                        {
                            lock (_reusableAppServicesSyncRoot)
                            {
                                result = _reusableAppServices[appServiceType];

                                if (result == null)
                                {
                                    result = (AppServicesBase)Activator.CreateInstance(appServiceType);
                                    _reusableAppServices.Add(appServiceType, result);
                                    result.InitializeAppService(this);
                                }
                            }
                        }
                    }
                    else
                    {
                        result = (AppServicesBase)Activator.CreateInstance(appServiceType);
                        result.InitializeAppService(this);
                    }
                }
                else
                {
                    throw new InvalidOperationException("The requested type is not an interface or an AppServiceBase derived type.");
                }
            }

            if (result == null)
                throw new KeyNotFoundException("The requested app service does not exist.");

            return result;
        }

        /// <summary>
        /// Detach an app service from this context. Useful for when you do not want SavingChanges() event to fire in the app service when committing to the store.
        /// </summary>
        public void DetachAppService(AppServicesBase appService)
        {
            appService.DetachFromAppContext();
        }

        /// <summary>
        /// Retrieve a data context from the app context or create it with default options.
        /// </summary>
        public DataContext<T> GetOrCreateDataContext<T>(bool ignoreDisposable = false) where T : class, new()
        {
            bool disposable = ignoreDisposable ? false : typeof(T).IsAssignableFrom(typeof(IDisposable));
            Type type = disposable ? typeof(DisposableDataContext<T>) : typeof(DataContext<T>);

            DataContext<T> result = (DataContext<T>)GetSharedDataObject(type);

            if (result == null)
            {
                lock (_sharedObjectsSyncRoot)
                {
                    result = (DataContext<T>)GetSharedDataObject(type);

                    if (result == null)
                    {
                        result = CreateDataContext<T>(disposable);
                        AddSharedDataObject(type, result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create a data context outside of the AppContext with default options. NOTE: the returned data context is not tracked by the app context and must be disposed by the caller if it implements IDisposable.
        /// </summary>
        public DataContext<T> CreateDataContext<T>(bool disposable) where T : class, new()
        {
            DataContext<T> result = null;

            if (disposable)
                result = new DisposableDataContext<T>();
            else
                result = new DataContext<T>();

            result.InitializeDataContext(this);

            return result;
        }

        /// <summary>
        /// Retrieve a data context from the app context or create it with default options.
        /// </summary>
        public TDataContext GetOrCreateDataContext<TDataContext, T>() where T : class, new() where TDataContext : DataContext<T>, new()
        {
            Type type = typeof(TDataContext);
            TDataContext result = (TDataContext)GetSharedDataObject(type);

            if (result == null)
            {
                lock (_sharedObjectsSyncRoot)
                {
                    result = (TDataContext)GetSharedDataObject(type);

                    if (result == null)
                    {
                        result = CreateDataContext<TDataContext, T>();
                        AddSharedDataObject(type, result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create a data context outside of the AppContext with default options. NOTE: the returned data context is not tracked by the app context and must be disposed by the caller if it implements IDisposable.
        /// </summary>
        public TDataContext CreateDataContext<TDataContext, T>() where T : class, new() where TDataContext : DataContext<T>, new()
        {
            TDataContext result = new TDataContext();

            result.InitializeDataContext(this);

            return result;
        }

        /// <summary>
        /// Retrieve a data context from the app context or create it using the supplied initializer.
        /// </summary>
        public DataContext<T> GetOrCreateDataContext<T>(Func<AppContextBase, T> initializer) where T : class, new()
        {
            Type type = typeof(DataContext<T>);
            DataContext<T> result = (DataContext<T>)GetSharedDataObject(type);

            if (result == null)
            {
                lock (_sharedObjectsSyncRoot)
                {
                    result = (DataContext<T>)GetSharedDataObject(type);

                    if (result == null)
                    {
                        result = CreateDataContext<T>(initializer);
                        AddSharedDataObject(type, result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Create a data context outside of the AppContext using the supplied initializer. NOTE: the returned data context is not tracked by the app context and must be disposed by the caller if it implements IDisposable.
        /// </summary>
        public DataContext<T> CreateDataContext<T>(Func<AppContextBase, T> initializer) where T : class, new()
        {
            DataContext<T> result = new DataContext<T>();

            initializer(this);

            return result;
        }

        /// <summary>
        /// Get an object that is shared between UoW classes of the same base type (i.e. Entity Framework UoW classes).
        /// </summary>
        internal protected object GetSharedDataObject(Type type)
        {
            object result = null;

            if (_sharedObjects.TryGetValue(type, out SharedObjectEntry sharedObjectEntry))
            {
                result = sharedObjectEntry.Value;
            }

            return result;
        }

        /// <summary>
        /// Get an object that is shared between UoW classes of the same base type (i.e. Entity Framework UoW classes).
        /// </summary>
        internal protected T GetSharedDataObject<T>()
        {
            T result = default(T);

            if (_sharedObjects.TryGetValue(typeof(T), out SharedObjectEntry sharedObjectEntry))
            {
                result = (T)sharedObjectEntry.Value;
            }

            return result;
        }

        /// <summary>
        /// Add an object that can be shared between UoW classes of the same base type (i.e. Entity Framework UoW classes).
        /// </summary>
        internal protected void AddSharedDataObject(object o)
        {
            AddSharedDataObject(o.GetType(), o);
        }

        /// <summary>
        /// Add an object that can be shared between UoW classes of the same base type (i.e. Entity Framework UoW classes).
        /// </summary>
        internal protected void AddSharedDataObject(Type type, object o)
        {
            if (!_sharedObjects.ContainsKey(type))
                _sharedObjects.Add(type, new SharedObjectEntry(_sharedObjects.Count, o));
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual bool SaveChanges(bool assertSuccess = false)
        {
            return SaveChanges(null, null, assertSuccess);
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual bool SaveChanges(AppPrincipal contextAppUser, bool assertSuccess = false)
        {
            return SaveChanges(contextAppUser, null, assertSuccess);
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual bool SaveChanges(Type sharedDataObjectType, bool assertSuccess = false)
        {
            return SaveChanges(null, sharedDataObjectType, assertSuccess);
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual bool SaveChanges(AppPrincipal contextUser, Type sharedDataObjectType, bool assertSuccess = false)
        {
            bool result = false;

            List<ErrorResult> errors = new List<ErrorResult>();
            OptionsDictionary<Type, object> results = new OptionsDictionary<Type, object>();
            List<KeyValuePair<Type, SharedObjectEntry>> sharedObjects = _sharedObjects
                .Where(o => (sharedDataObjectType == null || o.Key == sharedDataObjectType) && o.Value.Value != null && o.Value.Value is ISaveable)
                .OrderBy(o => o.Value.Index).ToList();

            bool canSave = true;

            foreach (KeyValuePair<Type, SharedObjectEntry> sharedObject in sharedObjects)
            {
                canSave = (sharedObject.Value.Value as ISaveable).CanSave(contextUser, errors);

                if (!canSave)
                    break;
            }

            _lastSaveErrors = errors;

            if (canSave)
            {
                foreach (KeyValuePair<Type, SharedObjectEntry> sharedObject in sharedObjects)
                {
                    canSave = (sharedObject.Value.Value as ISaveable).CanSave(contextUser, errors);

                    if (!canSave)
                        break;

                    object saveResult = ((ISaveable)sharedObject.Value.Value).Save(contextUser);

                    results.Add(sharedObject.Key, saveResult);
                }

                result = true;
            }

            _lastSaveResults = results;

            if (assertSuccess)
                AssertSaveSuccess(result);

            return result;
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual async Task<bool> SaveChangesAsync(bool assertSuccess = false)
        {
            return await SaveChangesAsync(null, null, assertSuccess);
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual async Task<bool> SaveChangesAsync(AppPrincipal contextAppUser, bool assertSuccess = false)
        {
            return await SaveChangesAsync(contextAppUser, null, assertSuccess);
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual async Task<bool> SaveChangesAsync(Type sharedDataObjectType, bool assertSuccess = false)
        {
            return await SaveChangesAsync(null, sharedDataObjectType, assertSuccess);
        }

        /// <summary>
        /// Commit changes to any store in the context that supports it.
        /// </summary>
        public virtual async Task<bool> SaveChangesAsync(AppPrincipal contextUser, Type sharedDataObjectType, bool assertSuccess = false)
        {
            bool result = false;

            List<ErrorResult> errors = new List<ErrorResult>();
            OptionsDictionary<Type, object> results = new OptionsDictionary<Type, object>();
            List<KeyValuePair<Type, SharedObjectEntry>> sharedObjects = _sharedObjects
                .Where(o => (sharedDataObjectType == null || o.Key == sharedDataObjectType) && o.Value.Value != null && o.Value.Value is ISaveable)
                .OrderBy(o => o.Value.Index).ToList();

            bool canSave = true;

            foreach (KeyValuePair<Type, SharedObjectEntry> sharedObject in sharedObjects)
            {
                canSave = (sharedObject.Value.Value as ISaveable).CanSave(contextUser, errors);

                if (!canSave)
                    break;
            }

            _lastSaveErrors = errors;

            if (canSave)
            {
                foreach (KeyValuePair<Type, SharedObjectEntry> sharedObject in sharedObjects)
                {
                    canSave = (sharedObject.Value.Value as ISaveable).CanSave(contextUser, errors);

                    if (!canSave)
                        break;

                    object saveResult = await ((ISaveable)sharedObject.Value.Value).SaveAsync(contextUser);

                    results.Add(sharedObject.Key, saveResult);
                }

                result = true;
            }

            _lastSaveResults = results;

            if (assertSuccess)
                AssertSaveSuccess(result);

            return result;
        }

        private void AssertSaveSuccess(bool value)
        {
            if (!value)
            {
                IEnumerable<string> errors = new string[] { "The last save was not successfull." };

                try
                {
                    IEnumerable<string> lastErrors = GetAllLastErrorAndSaveResults();

                    if (lastErrors != null && lastErrors.Any())
                    {
                        errors = errors.Concat(lastErrors);
                    }
                }
                catch { }

                throw new AppServicesSaveException(string.Join("; ", errors));
            }
        }

        /// <summary>
        /// Get list of errors and warnings after the last Save operation.
        /// </summary>
        public IEnumerable<ErrorResult> GetLastErrors()
        {
            return _lastSaveErrors.AsReadOnly();
        }

        /// <summary>
        /// Get the result of the last SaveChanges for a set of UoW objects. For instance, Entity Framework UoW would have returned RecordsAffected.
        /// </summary>
        public object GetLastSaveResult(Type appServicesGroupType)
        {
            return _lastSaveResults[appServicesGroupType];
        }

        private IEnumerable<string> GetAllLastErrorAndSaveResults()
        {
            IEnumerable<string> result = Enumerable.Empty<string>();

            if (_lastSaveErrors != null && _lastSaveErrors.Any())
            {
                result = result.Concat(_lastSaveErrors.Select(e => e.Message));
            }

            if (_lastSaveResults != null && _lastSaveResults.Any())
            {
                result = result.Concat(_lastSaveResults.Where(r => r.Value != null).Select(r => r.Value.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Queues a method for execution. By default, the method executes when a thread pool thread becomes available, but parent applications can override this by specifying a different action using AppContext.SetDefaults(...).
        /// </summary>
        public void QueueBackgroundAction(Action action)
        {
            _backgroundAction(action);
        }

        /// <summary>
        /// Log an error in the store asynchronously.
        /// </summary>
        /// <param name="exception">The underlying exception of the error.</param>
        /// <param name="info">Optional additional info to record in the store.</param>
        /// <param name="code">Optional code to record in the store.</param>
        /// <param name="applicationId">The id of the application that caused the error.</param>
        /// <param name="userId">The unique id of the user that caused the error.</param>
        /// <param name="userGuid">The unique id of the user that caused the error.</param>
        /// <param name="userInfo">The unique id of the user that caused the error.</param>
        public void QueueLogError(Exception exception, string info, short code, int applicationId, int? userId, Guid? userGuid, string userInfo)
        {
            QueueBackgroundAction(() => ErrorLogger.LogError(exception, info, code, applicationId, userId, userGuid, userInfo));
        }

        /// <summary>
        /// Log an action in the store asynchronously.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userId">Integer id, if any, of the user performing the action.</param>
        /// <param name="userGuid">Guid id, if any, of the user performing the action.</param>
        /// <param name="userInfo">App defined id info (such as compound database primary key) identifying the user performing the action.</param>
        /// <param name="itemType">App defined id for the table or set of information being audited.</param>
        /// <param name="itemId">Integer id, if any, of the item being audited.</param>
        /// <param name="itemGuid">Guid id, if any, of the item being audited.</param>
        /// <param name="itemInfo">App defined id info (such as compound database primary key) identifying the item being audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        public void QueueLogAuditAction(AppAuditAction action, int applicationId, int? userId, Guid? userGuid, string userInfo, int? itemType, int? itemId, Guid? itemGuid, string itemInfo, string notes)
        {
            QueueBackgroundAction(() => AuditLogger.LogAction(action, applicationId, userId, userGuid, userInfo, itemType, itemId, itemGuid, itemInfo, notes));
        }

        /// <summary>
        /// Log an action in the store asynchronously.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userId">Integer id, if any, of the user performing the action.</param>
        /// <param name="userGuid">Guid id, if any, of the user performing the action.</param>
        /// <param name="userInfo">App defined id info (such as compound database primary key) identifying the user performing the action.</param>
        /// <param name="itemType">App defined id for the table or set of information being audited.</param>
        /// <param name="itemId">Integer id, if any, of the item being audited.</param>
        /// <param name="itemGuid">Guid id, if any, of the item being audited.</param>
        /// <param name="itemInfo">App defined id info (such as compound database primary key) identifying the item being audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        public void QueueLogAuditAction(short action, int applicationId, int? userId, Guid? userGuid, string userInfo, int? itemType, int? itemId, Guid? itemGuid, string itemInfo, string notes)
        {
            QueueBackgroundAction(() => AuditLogger.LogAction(action, applicationId, userId, userGuid, userInfo, itemType, itemId, itemGuid, itemInfo, notes));
        }

        /// <summary>
        /// Log actions in the store asynchronously.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userIdentifier">Identifying properties of the user.</param>
        /// <param name="itemsInfo">A dictionary that contains information about the items to be audited.</param>
        /// <param name="items">The items to be audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        public void QueueLogAuditAction(AppAuditAction action, int applicationId, ItemIdentifier userIdentifier, IDictionary<Type, AppAuditItemInfo> itemsInfo, IEnumerable<object> items, string notes)
        {
            QueueBackgroundAction(() => AuditLogger.LogActions(action, applicationId, userIdentifier, itemsInfo, items, notes));
        }

        /// <summary>
        /// Log actions in the store asynchronously.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userIdentifier">Identifying properties of the user.</param>
        /// <param name="itemsInfo">A dictionary that contains information about the items to be audited.</param>
        /// <param name="items">The items to be audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        public void QueueLogAuditAction(short action, int applicationId, ItemIdentifier userIdentifier, IDictionary<Type, AppAuditItemInfo> itemsInfo, IEnumerable<object> items, string notes)
        {
            QueueBackgroundAction(() => AuditLogger.LogActions(action, applicationId, userIdentifier, itemsInfo, items, notes));
        }

        /// <summary>
        /// Checks for a condition; if the condition is false, throws an UnauthorizedAccess exception.
        /// </summary>
        public virtual void AssertAuthorization(bool value)
        {
            if (!value)
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Releases all resources used by this app context and any unit of work objects associated with it.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this app context and any unit of work objects associated with it.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    List<SharedObjectEntry> sharedObjects = _sharedObjects.Values.OrderByDescending(s => s.Index).ToList();

                    foreach (SharedObjectEntry sharedObject in sharedObjects)
                    {
                        if (!sharedObject.IgnoreDisposable && sharedObject.Value != null && sharedObject.Value is IDisposable)
                        {
                            IDisposable disposableObject = sharedObject.Value as IDisposable;

                            disposableObject.Dispose();
                        }
                    }
                }

                _disposed = true;
            }
        }
    }
}
