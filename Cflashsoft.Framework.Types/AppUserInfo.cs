using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Misc additional info for the current principal that can be used in place of or in addition to claims.
    /// </summary>
    public class AppUserInfo
    {
        /// <summary>
        /// Identifies the property that uniquely identifies the principal
        /// </summary>
        public AppUserIdType UserIdType { get; protected set; }

        /// <summary>
        /// Optional unique application specific integer ID of the principal (usually a database primary key)
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// Optional unique, application specific GUID ID of the principal (usually a database primary key or Azure identifier key)
        /// </summary>
        public Guid Oid { get; protected set; }

        /// <summary>
        /// Optional unique application specific username of the principal
        /// </summary>
        public string UserName { get; protected set; }

        /// <summary>
        /// Optional display name of the user that can be used by the front-end
        /// </summary>
        public string DisplayName { get; protected set; }

        /// <summary>
        /// Optional primary email of the user that can be used by the front-end
        /// </summary>
        public string PrimaryEmail { get; protected set; }

        /// <summary>
        /// Initializes a new instance of AppUserInfo class.
        /// </summary>
        public AppUserInfo(AppUserIdType userIdType, int id, Guid oid, string userName, string displayName, string primaryEmail)
        {
            this.UserIdType = userIdType;
            this.Id = id;
            this.Oid = oid;
            this.UserName = userName;
            this.DisplayName = displayName;
            this.PrimaryEmail = primaryEmail;
        }
    }
}
