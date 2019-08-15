using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Utility factory methods for Cflashsoft AppContext.
    /// </summary>
    public class AppContextFactory<TAppContext>
            where TAppContext : AppContextBase, new()
    {
        private List<KeyValuePair<Type, Type>> _appServices = null;

        /// <summary>
        /// Initializes a new instance of the AppContextFactory class.
        /// </summary>
        public AppContextFactory(IEnumerable<KeyValuePair<Type, Type>> appServices)
        {
            _appServices = appServices.ToList();
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
        /// <returns></returns>
        public TAppContext NewAppContext(params KeyValuePair<string, object>[] options)
        {
            return NewAppContext(new AppOptions(options));
        }

        /// <summary>
        /// Returns a new AppContext instance with the specificed services injected.
        /// </summary>
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
        /// <returns></returns>
        public TAppContext NewAppContext(IEnumerable<KeyValuePair<string, object>> options)
        {
            return NewAppContext(new AppOptions(options));
        }

        /// <summary>
        /// Returns a new AppContext instance with the specificed services injected.
        /// </summary>
        /// <returns></returns>
        public TAppContext NewAppContext(AppOptions options)
        {
            TAppContext result = new TAppContext();

            if (options != null)
                result._options = options;

            foreach (var appService in _appServices)
            {
                AppServicesBase appServiceInst = (AppServicesBase)Activator.CreateInstance(appService.Value);

                appServiceInst.InitializeAppService(result);

                result._reusableAppServices.Add(appService.Key, appServiceInst);
            }

            return result;
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
