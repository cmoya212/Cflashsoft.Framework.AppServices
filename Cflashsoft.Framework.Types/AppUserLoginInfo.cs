using System;
using System.Collections.Generic;
using System.Text;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Misc login info for an application user.
    /// </summary>
    public class AppUserLoginInfo
    {
        /// <summary>
        /// Identifies the property that uniquely identifies the principal
        /// </summary>
        public AppUserIdType UserIdType { get; set; }

        /// <summary>
        /// Optional unique application specific integer ID of the principal (usually a database primary key)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Optional unique, application specific GUID ID of the principal (usually a database primary key or Azure identifier key)
        /// </summary>
        public Guid Oid { get; set; }

        /// <summary>
        /// Optional unique application specific username of the principal
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Returns true if the login for the user is currently locked.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Returns the lockout end date for the user.
        /// </summary>
        public DateTime? LockoutEndDate { get; set; }

        /// <summary>
        /// Returns true if the use must change their password during their next log in.
        /// </summary>
        public bool MustChangePsswd { get; set; }

        /// <summary>
        /// Returns the number of failed log in attempts.
        /// </summary>
        public int LoginAttempts { get; set; }
        
        /// <summary>
        /// Returns the last log in date for the user.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
    }
}
