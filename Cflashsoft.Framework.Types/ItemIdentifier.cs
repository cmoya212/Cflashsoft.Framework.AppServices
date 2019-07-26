using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents a set of inclusive or exclusive properties that can be used to identify the unique key of an item such as a database entity with a unique key field.
    /// </summary>
    public struct ItemIdentifier
    {
        /// <summary>
        /// The integer unique key of the item if available.
        /// </summary>
        public int? ItemId;

        /// <summary>
        /// The GUID unique key of the item if available.
        /// </summary>
        public Guid? ItemGuid;

        /// <summary>
        /// The catch-all, freeform key of the item if available such as compound keys or some other type. 
        /// </summary>
        public string ItemInfo;

        /// <summary>
        /// Initialized a new instance of the ItemIdentifier structure.
        /// </summary>
        public ItemIdentifier(int? itemId, Guid? itemGuid, string itemInfo)
        {
            this.ItemId = itemId;
            this.ItemGuid = itemGuid;
            this.ItemInfo = itemInfo;
        }

        /// <summary>
        /// Returns a set of inclusive or exclusive properties that can be used to identify the unique key of an item such as a database entity with a unique key field.
        /// </summary>
        /// <param name="item">The object to be identified.</param>
        /// <param name="fieldName">The identifying field or property. NOTE: Compound keys can be specified by using commas.</param>
        public static ItemIdentifier FromObject(object item, string fieldName)
        {
            return FromObject(item, null, fieldName);
        }

        /// <summary>
        /// Returns a set of inclusive or exclusive properties that can be used to identify the unique key of an item such as a database entity with a unique key field.
        /// </summary>
        /// <param name="item">The object to be identified.</param>
        /// <param name="itemType">The type of the item being identified. If null, the type will be retrieved using reflection.</param>
        /// <param name="fieldName">The identifying field or property. NOTE: Compound keys can be specified by using commas.</param>
        public static ItemIdentifier FromObject(object item, Type itemType, string fieldName)
        {
            ItemIdentifier result = new ItemIdentifier();

            if (itemType == null)
                itemType = item.GetType();

            if (!fieldName.Contains(","))
            {
                PropertyInfo propertyInfo = itemType.GetProperty(fieldName);
                Type propertyType = propertyInfo.PropertyType;

                if (propertyType == typeof(int) || propertyType == typeof(short) || propertyType == typeof(byte))
                {
                    result.ItemId = (int)propertyInfo.GetValue(item);
                }
                else if (propertyType == typeof(Guid))
                {
                    result.ItemGuid = (Guid)propertyInfo.GetValue(item);
                }
                else
                {
                    result.ItemInfo = propertyInfo.GetValue(item).ToString();
                }
            }
            else
            {
                string[] keyNames = fieldName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();

                for (int index = 0; index < keyNames.Length; index++)
                {
                    PropertyInfo propertyInfo = itemType.GetProperty(keyNames[index].Trim());
                    Type propertyType = propertyInfo.PropertyType;

                    if (propertyType == typeof(int) || propertyType == typeof(short) || propertyType == typeof(byte))
                    {
                        sb.Append((int)propertyInfo.GetValue(item));
                    }
                    else if (propertyType == typeof(Guid))
                    {
                        sb.Append(((Guid)propertyInfo.GetValue(item)).ToString("N"));
                    }
                    else
                    {
                        sb.Append(propertyInfo.GetValue(item).ToString());
                    }

                    if (index < keyNames.Length - 1)
                        sb.Append(',');
                }

                result.ItemInfo = sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// Returns a string representation of the ItemIdentifier.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (this.ItemId.HasValue)
                sb.Append(this.ItemId.Value);
            else
                sb.Append('*');

            sb.Append(',');

            if (this.ItemGuid.HasValue)
                sb.Append(this.ItemId.Value.ToString("N"));
            else
                sb.Append('*');

            sb.Append(',');

            if (!string.IsNullOrWhiteSpace(this.ItemInfo))
                sb.Append(this.ItemInfo);
            else
                sb.Append('*');

            return sb.ToString();
        }
    }
}
