using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Application audit actions.
    /// </summary>
    public enum AppAuditAction : short
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0,
        /// <summary>
        /// A resource was accessed.
        /// </summary>
        Access = 1,
        /// <summary>
        /// A resource was retrieved.
        /// </summary>
        Retrieve = 2,
        /// <summary>
        /// A resource was added.
        /// </summary>
        Add = 3,
        /// <summary>
        /// A resource was modified.
        /// </summary>
        Modify = 4,
        /// <summary>
        /// A resource was deleted.
        /// </summary>
        Delete = 5,
        /// <summary>
        /// A user logged in.
        /// </summary>
        Login = 6,
        /// <summary>
        /// A user logged out.
        /// </summary>
        Logout = 7,
        /// <summary>
        /// A resource was copied.
        /// </summary>
        Copy = 8,
        /// <summary>
        /// A resource was transfered or downloaded.
        /// </summary>
        Transfer = 9,
        /// <summary>
        /// A resource was improperly accessed.
        /// </summary>
        Unauthorized = 10,
        /// <summary>
        /// A system or subsystem was started.
        /// </summary>
        Start = 11,
        /// <summary>
        /// A system or subsystem was stopped.
        /// </summary>
        Stop = 12
    }
}
