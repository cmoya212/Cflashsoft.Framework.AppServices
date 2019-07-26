using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents an access token that will be encrypted and used inside of an AuthToken for API requests. NOTE: similar to JWT tokens, this class strives to keep properties concise for serialization.
    /// </summary>
    public class AccessTokenSlim
    {
        /// <summary>
        /// User id.
        /// </summary>
        public string u { get; set; }
        
        /// <summary>
        /// Expiration date.
        /// </summary>
        public DateTime e { get; set; }
    }
}
