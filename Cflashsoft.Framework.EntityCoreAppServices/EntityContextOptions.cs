using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cflashsoft.Framework.AppServices;

namespace Cflashsoft.Framework.EntityCoreAppServices
{
    /// <summary>
    /// Helper utility to create app options to pass to AppContext.
    /// </summary>
    public static class EntityContextOptions
    {
        /// <summary>
        /// Create app options collection.
        /// </summary>
        /// <param name="enableLazyLoading">Enable DbContext navigation properties lazy loading. Specify null for DbContext to use its default model-first or code-first setting.</param>
        /// <param name="enableProxyCreation">Enable DbContext entity proxy creation. Specify null for DbContext to use its default model-first or code-first setting.</param>
        /// <param name="validator">Alternate custom data validator to use in this context rather than the default.</param>
        public static IEnumerable<KeyValuePair<string, object>> CreateOptions(bool? enableLazyLoading = null, bool? enableProxyCreation = null, object validator = null)
        {
            return new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("EF_EnableLazyLoading", enableLazyLoading),
                new KeyValuePair<string, object>("EF_EnableProxyCreation", enableProxyCreation),
                new KeyValuePair<string, object>("EF_EntityAppValidator", validator),
            };
        }
    }
}
