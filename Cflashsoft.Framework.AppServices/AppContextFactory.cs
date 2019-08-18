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
    /// Utility factory methods for Cflashsoft AppContext.
    /// </summary>
    public class AppContextFactory<TAppContext>
            where TAppContext : AppContextBase, new()
    {
        private List<KeyValuePair<Type, Type>> _appServices = null;
        private ErrorLoggerBase _errorLogger = null;
        private AuditLoggerBase _auditLogger = null;
        private Action<Action> _backgroundAction = null;
        private Action<Action<TAppContext>> _factoryBackgroundAction = null;
        private AppOptions _options = null;

        /// <summary>
        /// Provides logging services. NOTE: this error logger is static and does not share a state or connections with the app context.
        /// </summary>
        public ErrorLoggerBase ErrorLogger => _errorLogger;

        /// <summary>
        /// Provides audit logging services. NOTE: this audit logger is static and does not share a state or connections with the app context.
        /// </summary>
        public AuditLoggerBase AuditLogger => _auditLogger;

        /// <summary>
        /// Initializes a new instance of the AppContextFactory class.
        /// </summary>
        public AppContextFactory()
            : this(null, null, null, null, null, new AppOptions())
        { }

        /// <summary>
        /// Initializes a new instance of the AppContextFactory class.
        /// </summary>
        /// <param name="appServices">Injected AppServiceBase types.</param>
        public AppContextFactory(IEnumerable<KeyValuePair<Type, Type>> appServices)
            : this(appServices, null, null, null, null, new AppOptions())
        { }

        /// <summary>
        /// Initializes a new instance of the AppContextFactory class.
        /// </summary>
        /// <param name="appServices">Injected AppServiceBase types.</param>
        /// <param name="errorLogger">The default stateless error logger that can be used by app services and the parent application to log errors. If null, the default error logger will output to the console or trace listeners.</param>
        /// <param name="auditLogger">The default stateless audit logger that can be used by app services and the parent application to log an audit trail. If null, the default audit logger will output to the console or trace listeners.</param>
        /// <param name="backgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="factoryBackgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="options">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        public AppContextFactory(IEnumerable<KeyValuePair<Type, Type>> appServices, ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, Action<Action<TAppContext>> factoryBackgroundAction, params KeyValuePair<string, object>[] options)
            : this(appServices, errorLogger, auditLogger, backgroundAction, factoryBackgroundAction, new AppOptions(options))
        { }

        /// <summary>
        /// Initializes a new instance of the AppContextFactory class.
        /// </summary>
        /// <param name="appServices">Injected AppServiceBase types.</param>
        /// <param name="errorLogger">The default stateless error logger that can be used by app services and the parent application to log errors. If null, the default error logger will output to the console or trace listeners.</param>
        /// <param name="auditLogger">The default stateless audit logger that can be used by app services and the parent application to log an audit trail. If null, the default audit logger will output to the console or trace listeners.</param>
        /// <param name="backgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="factoryBackgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="optionsLists">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        public AppContextFactory(IEnumerable<KeyValuePair<Type, Type>> appServices, ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, Action<Action<TAppContext>> factoryBackgroundAction, params IEnumerable<KeyValuePair<string, object>>[] optionsLists)
            : this(appServices, errorLogger, auditLogger, backgroundAction, factoryBackgroundAction, new AppOptions())
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
        /// Initializes a new instance of the AppContextFactory class.
        /// </summary>
        /// <param name="appServices">Injected AppServiceBase types.</param>
        /// <param name="errorLogger">The default stateless error logger that can be used by app services and the parent application to log errors. If null, the default error logger will output to the console or trace listeners.</param>
        /// <param name="auditLogger">The default stateless audit logger that can be used by app services and the parent application to log an audit trail. If null, the default audit logger will output to the console or trace listeners.</param>
        /// <param name="backgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="factoryBackgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="options">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        public AppContextFactory(IEnumerable<KeyValuePair<Type, Type>> appServices, ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, Action<Action<TAppContext>> factoryBackgroundAction, IEnumerable<KeyValuePair<string, object>> options)
            : this(appServices, errorLogger, auditLogger, backgroundAction, factoryBackgroundAction, new AppOptions(options))
        { }

        /// <summary>
        /// Initializes a new instance of the AppContextFactory class.
        /// </summary>
        /// <param name="appServices">Injected AppServiceBase types.</param>
        /// <param name="errorLogger">The default stateless error logger that can be used by app services and the parent application to log errors. If null, the default error logger will output to the console or trace listeners.</param>
        /// <param name="auditLogger">The default stateless audit logger that can be used by app services and the parent application to log an audit trail. If null, the default audit logger will output to the console or trace listeners.</param>
        /// <param name="backgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="factoryBackgroundAction">If null, the default mechanism, ThreadPool.QueueUserWorkItem, is used. NOTE: ASP.NET applications should use (Action action) => HostingEnvironment.QueueBackgroundWorkItem(a => action()). Otherwise, background threads spawned using AppContext.QueueBackgroundAction are subject to AppPool teardowns and may never complete.</param>
        /// <param name="options">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        public AppContextFactory(IEnumerable<KeyValuePair<Type, Type>> appServices, ErrorLoggerBase errorLogger, AuditLoggerBase auditLogger, Action<Action> backgroundAction, Action<Action<TAppContext>> factoryBackgroundAction, AppOptions options)
        {
            _appServices = appServices != null ? appServices.ToList() : new List<KeyValuePair<Type, Type>>();
            _errorLogger = errorLogger ?? new TraceErrorLogger();
            _auditLogger = auditLogger ?? new TraceAuditLogger();
            _backgroundAction = backgroundAction != null ? backgroundAction : (Action action) => ThreadPool.QueueUserWorkItem(a => action());
            _factoryBackgroundAction = factoryBackgroundAction != null ? factoryBackgroundAction : (Action<TAppContext> action) => ThreadPool.QueueUserWorkItem(a => { using (var appContext = this.NewAppContext()) { action(appContext); } });
            _options = options ?? new AppOptions();
        }

        /// <summary>
        /// Returns a new AppContext instance with the specificed services injected.
        /// </summary>
        /// <returns></returns>
        public TAppContext NewAppContext()
        {
            return NewAppContext((AppOptions)null);
        }

        /// <summary>
        /// Returns a new AppContext instance with the specificed services injected.
        /// </summary>
        /// <param name="options">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        /// <returns></returns>
        public TAppContext NewAppContext(params KeyValuePair<string, object>[] options)
        {
            return NewAppContext(new AppOptions(options));
        }

        /// <summary>
        /// Returns a new AppContext instance with the specificed services injected.
        /// </summary>
        /// <param name="optionsLists">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        /// <returns></returns>
        public TAppContext NewAppContext(params IEnumerable<KeyValuePair<string, object>>[] optionsLists)
        {
            AppOptions options = new AppOptions();

            foreach (IEnumerable<KeyValuePair<string, object>> optionsList in optionsLists)
            {
                foreach (KeyValuePair<string, object> option in optionsList)
                {
                    options.Add(option);
                }
            }

            return NewAppContext(options);
        }

        /// <summary>
        /// Returns a new AppContext instance with the specificed services injected.
        /// </summary>
        /// <param name="options">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        /// <returns></returns>
        public TAppContext NewAppContext(IEnumerable<KeyValuePair<string, object>> options)
        {
            return NewAppContext(new AppOptions(options));
        }

        /// <summary>
        /// Returns a new AppContext instance with the specificed services injected.
        /// </summary>
        /// <param name="options">A collection of custom options that can be used by AppService UoW classes for application specific purposes.</param>
        /// <returns></returns>
        public TAppContext NewAppContext(AppOptions options)
        {
            TAppContext result = new TAppContext();

            result._errorLogger = _errorLogger;
            result._auditLogger = _auditLogger;
            result._backgroundAction = _backgroundAction;

            if (options != null)
                result._options = options;
            else
                result._options = _options;

            foreach (var appService in _appServices)
            {
                AppServicesBase appServiceInst = (AppServicesBase)Activator.CreateInstance(appService.Value);

                appServiceInst.InitializeAppService(result);

                result._reusableAppServices.Add(appService.Key, appServiceInst);
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
        /// Queues a method for execution. By default, the method executes when a thread pool thread becomes available, but parent applications can override this by specifying a different action using AppContext.SetDefaults(...).
        /// </summary>
        public void QueueBackgroundAction(Action<TAppContext> action)
        {
            _factoryBackgroundAction(action);
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
            QueueBackgroundAction(() => this.ErrorLogger.LogError(exception, info, code, applicationId, userId, userGuid, userInfo));
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
            QueueBackgroundAction(() => this.AuditLogger.LogAction(action, applicationId, userId, userGuid, userInfo, itemType, itemId, itemGuid, itemInfo, notes));
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
            QueueBackgroundAction(() => this.AuditLogger.LogAction(action, applicationId, userId, userGuid, userInfo, itemType, itemId, itemGuid, itemInfo, notes));
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
            QueueBackgroundAction(() => this.AuditLogger.LogActions(action, applicationId, userIdentifier, itemsInfo, items, notes));
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
            QueueBackgroundAction(() => this.AuditLogger.LogActions(action, applicationId, userIdentifier, itemsInfo, items, notes));
        }

        /// <summary>
        /// Utility method that provides a quick one-time execution of a UoW method.
        /// </summary>
        public static TResult Use<TAppService, TResult>(Func<TAppService, TResult> func)
            where TAppService : AppServicesBase, new()
        {
            using (TAppContext appContext = new TAppContext())
            {
                TAppService appService = appContext.GetAppService<TAppService>();

                return func(appService);
            }
        }

        /// <summary>
        /// Utility method that provides a quick one-time execution of a UoW method.
        /// </summary>
        public static async Task<TResult> UseAsync<TAppService, TResult>(Func<TAppService, Task<TResult>> func)
            where TAppService : AppServicesBase, new()
        {
            using (TAppContext appContext = new TAppContext())
            {
                TAppService appService = appContext.GetAppService<TAppService>();

                return await func(appService);
            }
        }

        /// <summary>
        /// Utility method that provides a quick one-time execution of a UoW method.
        /// </summary>
        public static void Use<TAppService>(Action<TAppService> action)
            where TAppService : AppServicesBase, new()
        {
            using (TAppContext appContext = new TAppContext())
            {
                TAppService appService = appContext.GetAppService<TAppService>();

                action(appService);
            }
        }

        /// <summary>
        /// Utility method that provides a quick one-time execution of a UoW method.
        /// </summary>
        public static async Task UseAsync<TAppService>(Func<TAppService, Task> action)
            where TAppService : AppServicesBase, new()
        {
            using (TAppContext appContext = new TAppContext())
            {
                TAppService appService = appContext.GetAppService<TAppService>();

                await action(appService);
            }
        }
    }
}
