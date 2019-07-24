using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Base class for a data context such as an Entity Framework DbContext or WCF Service Proxy and used by UoW AppServices classes to share a context.
    /// </summary>
    public class DataContext<T> where T : class, new()
    {
        private AppContextBase _appContext = null;
        private T _data = null;

        /// <summary>
        /// Shared application services context.
        /// </summary>
        public AppContextBase AppContext
        {
            get
            {
                return _appContext;
            }
            protected set
            {
                _appContext = value;
            }
        }

        /// <summary>
        /// A data object wrapped by this DataContext such as an Entity Framework DbContext or WCF Service Proxy.
        /// </summary>
        public T Data
        {
            get
            {
                return _data;
            }
            protected set
            {
                _data = value;
            }
        }

        /// <summary>
        /// Initialize the data context if necessary such as instantiating an Entity Framework DbContext or WCF Service Client etc.
        /// </summary>
        public virtual void InitializeDataContext(AppContextBase appContext)
        {
            this.AppContext = appContext;
            this.Data = new T();
        }
    }
}
