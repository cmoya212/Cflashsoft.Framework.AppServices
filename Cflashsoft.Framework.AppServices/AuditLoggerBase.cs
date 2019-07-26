using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cflashsoft.Framework.Types;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Represents a class that can log audit actions to a store.
    /// </summary>
    public abstract class AuditLoggerBase
    {
        private AppContextBase _appContext = null;

        /// <summary>
        /// Shared application services context.
        /// </summary>
        public AppContextBase AppContext
        {
            get
            {
                return _appContext;
            }
            protected set
            {
                _appContext = value;
            }
        }

        /// <summary>
        /// Inititializes a new instance of the AuditLogger class.
        /// </summary>
        public AuditLoggerBase()
        {

        }

        /// <summary>
        /// Inititializes a new instance of the AuditLogger class.
        /// </summary>
        public AuditLoggerBase(AppContextBase appContext)
        {
            _appContext = appContext;
        }

        /// <summary>
        /// Log an action in the store.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userId">Integer id, if any, of the user performing the action.</param>
        /// <param name="userGuid">Guid id, if any, of the user performing the action.</param>
        /// <param name="userInfo">App defined id info (such as compound database primary key) identifying the user performing the action.</param>
        /// <param name="itemType">App defined id for the table or set of information being audited.</param>
        /// <param name="itemId">Integer id, if any, of the item being audited.</param>
        /// <param name="itemGuid">Guid id, if any, of the item being audited.</param>
        /// <param name="itemInfo">App defined id info (such as compound database primary key) identifying the item being audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        public void LogAction(AppAuditAction action, int applicationId, int? userId, Guid? userGuid, string userInfo, int? itemType, int? itemId, Guid? itemGuid, string itemInfo, string notes)
        {
            LogAction((short)action, applicationId, userId, userGuid, userInfo, itemType, itemId, itemGuid, itemInfo, notes);
        }

        /// <summary>
        /// Log an action in the store.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userId">Integer id, if any, of the user performing the action.</param>
        /// <param name="userGuid">Guid id, if any, of the user performing the action.</param>
        /// <param name="userInfo">App defined id info (such as compound database primary key) identifying the user performing the action.</param>
        /// <param name="itemType">App defined id for the table or set of information being audited.</param>
        /// <param name="itemId">Integer id, if any, of the item being audited.</param>
        /// <param name="itemGuid">Guid id, if any, of the item being audited.</param>
        /// <param name="itemInfo">App defined id info (such as compound database primary key) identifying the item being audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        public abstract void LogAction(short action, int applicationId, int? userId, Guid? userGuid, string userInfo, int? itemType, int? itemId, Guid? itemGuid, string itemInfo, string notes);

        /// <summary>
        /// Log multiple actions in the store.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userIdentifier">Identifying properties of the user.</param>
        /// <param name="itemsInfo">A dictionary that contains information about the items to be audited.</param>
        /// <param name="items">The items to be audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        /// <param name="queueActions">Queue the logging in a background thread if possible. Note: this option can only be specified if this audit logger does not belong to an instance of AppContext.</param>
        public void LogActions(AppAuditAction action, int applicationId, ItemIdentifier userIdentifier, IDictionary<Type, AppAuditItemInfo> itemsInfo, IEnumerable<object> items, string notes, bool queueActions = false)
        {
            LogActions((short)action, applicationId, userIdentifier, itemsInfo, items, notes, queueActions);
        }

        /// <summary>
        /// Log multiple actions in the store.
        /// </summary>
        /// <param name="action">Identifies the action being performed (such as add, modift, or delete).</param>
        /// <param name="applicationId">The id of the application performing the action.</param>
        /// <param name="userIdentifier">Identifying properties of the user.</param>
        /// <param name="itemsInfo">A dictionary that contains information about the items to be audited.</param>
        /// <param name="items">The items to be audited.</param>
        /// <param name="notes">Additional information for the action being audited.</param>
        /// <param name="queueActions">Queue the logging in a background thread if possible. Note: this option can only be specified if this audit logger does not belong to an instance of AppContext.</param>
        public void LogActions(short action, int applicationId, ItemIdentifier userIdentifier, IDictionary<Type, AppAuditItemInfo> itemsInfo, IEnumerable<object> items, string notes, bool queueActions = false)
        {
            if (queueActions)
            {
                if (this.AppContext != null)
                    throw new InvalidOperationException("When queueActions is specified, AppContext cannot be defined for this instance of the audit logger.");
                //if (!object.ReferenceEquals(this, AppContextBase.AuditLogger))
                //    throw new InvalidOperationException("When queueActions is specified this audit logger must be the AppContextBase static audit logger.");
            }

            foreach (object item in items)
            {
                LogAction(action, applicationId, userIdentifier, itemsInfo, item, notes, queueActions);
            }
        }

        private void LogAction(short action, int applicationId, ItemIdentifier userIdentifier, IDictionary<Type, AppAuditItemInfo> itemsInfo, object item, string notes, bool queueAction)
        {
            Type itemType = item.GetType();

            if (itemsInfo.TryGetValue(itemType, out AppAuditItemInfo itemInfo))
            {
                ItemIdentifier itemIdentifier = ItemIdentifier.FromObject(item, itemType, itemInfo.FieldName);

                if (queueAction)
                    AppContextBase.QueueBackgroundAction(() => AppContextBase.AuditLogger.LogAction(action, applicationId, userIdentifier.ItemId, userIdentifier.ItemGuid, userIdentifier.ItemInfo, itemInfo.ItemType, itemIdentifier.ItemId, itemIdentifier.ItemGuid, itemIdentifier.ItemInfo, notes));
                else
                    LogAction(action, applicationId, userIdentifier.ItemId, userIdentifier.ItemGuid, userIdentifier.ItemInfo, itemInfo.ItemType, itemIdentifier.ItemId, itemIdentifier.ItemGuid, itemIdentifier.ItemInfo, notes);
            }
        }
    }
}
