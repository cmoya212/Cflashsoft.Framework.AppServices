using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents an authentication token for API requests. NOTE: similar to JWT tokens, this class strives to keep properties concise for serialization.
    /// </summary>
    public class AuthTokenSlim
    {
        /// <summary>
        /// Access token.
        /// </summary>
        public string a { get; set; }
        
        /// <summary>
        /// Expiration date.
        /// </summary>
        public DateTime e { get; set; }

        /// <summary>
        /// Refresh token.
        /// </summary>
        public string r { get; set; }
    }
}
