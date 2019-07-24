using System;
using System.Collections.Generic;
using System.Text;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents the result of a login.
    /// </summary>
    public class AppLoginResult
    {
        /// <summary>
        /// Returns true if the login was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Returns the reason for an unsuccessful login.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Returns the authentication token for a successful login.
        /// </summary>
        public Guid? Token { get; set; }

        /// <summary>
        /// Returns the expiration data of the authentication token for a successful login.
        /// </summary>
        public DateTime? TokenExpiration { get; set; }

        /// <summary>
        /// Returns the associated user object for a successful login.
        /// </summary>
        public object User { get; set; }
    }
}
