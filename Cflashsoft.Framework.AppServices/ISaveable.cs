using Cflashsoft.Framework.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Represents a class that can save data to a store.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// Returns true if the data can be saved in its current state.
        /// </summary>
        bool CanSave(AppPrincipal contextUser, IList<ErrorResult> errors);

        /// <summary>
        /// Save the data associated with this object.
        /// </summary>
        object Save(AppPrincipal contextUser);

        /// <summary>
        /// Save the data associated with this object.
        /// </summary>
        Task<object> SaveAsync(AppPrincipal contextUser);
    }
}
