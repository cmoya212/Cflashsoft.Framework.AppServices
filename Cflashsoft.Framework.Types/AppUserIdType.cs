using System;
using System.Collections.Generic;
using System.Text;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents the property that uniquely identifies an app user
    /// </summary>
    public enum AppUserIdType
    {
        /// <summary>
        /// Identifier is not known or not specified
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// User is identified by an integer id
        /// </summary>
        IdInteger = 1,

        /// <summary>
        /// User is identified by a guid
        /// </summary>
        OidGuid = 2,

        /// <summary>
        /// User is identified by a username string
        /// </summary>
        UserNameString = 3
    }
}
