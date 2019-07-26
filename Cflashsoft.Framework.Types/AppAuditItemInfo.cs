using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Application item audit information.
    /// </summary>
    public struct AppAuditItemInfo
    {
        /// <summary>
        /// The property name that represents the unique key of the object. Commas can be used to specify multiple properties in the case of compound keys.
        /// </summary>
        public string FieldName;

        /// <summary>
        /// Application defined unique number for the type of object for auditing purposes.
        /// </summary>
        public int ItemType;

        /// <summary>
        /// Initializes a new instance of the AppAuditItemInfo structure.
        /// </summary>
        public AppAuditItemInfo(string fieldName, int itemType)
        {
            this.FieldName = fieldName;
            this.ItemType = itemType;
        }
    }
}
