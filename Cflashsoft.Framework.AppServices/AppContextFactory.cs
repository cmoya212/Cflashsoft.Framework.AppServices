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
    public static class AppContextFactory<TAppContext>
            where TAppContext : AppContextBase, new()
    {
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
