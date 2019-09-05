using System;
using System.Collections.Generic;
using System.Text;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Represents a DataContext Data class that is aware of its containing AppContext.
    /// </summary>
    public interface IAppContextAware
    {
        /// <summary>
        /// Allows the DataContext Data class to get an instance of the containing AppContext.
        /// </summary>
        void InitializeFromAppContext(AppContextBase appContext);
    }
}
