using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Audit logger implementation that simply echoes to the console.
    /// </summary>
    public class TraceAuditLogger : AuditLoggerBase
    {
        /// <summary>
        /// Log an action to the console.
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
        public override void LogAction(short action, int applicationId, int? userId, Guid? userGuid, string userInfo, int? itemType, int? itemId, Guid? itemGuid, string itemInfo, string notes)
        {
            TraceWrite(action, applicationId, userId, userGuid, userInfo, itemType, itemId, itemGuid, itemInfo, notes);
        }

        /// <summary>
        /// Static method to log an action to the console.
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
        public static void TraceWrite(short action, int applicationId, int? userId, Guid? userGuid, string userInfo, int? itemType, int? itemId, Guid? itemGuid, string itemInfo, string notes)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Action: ");
            sb.Append(action.ToString());
            sb.Append("App: ");
            sb.Append(applicationId.ToString());
            sb.Append(",UserId: ");
            sb.Append(userId.HasValue ? userId.Value.ToString() : "*");
            sb.Append(".");
            sb.Append(userGuid.HasValue ? userGuid.Value.ToString("N") : "*");
            sb.Append(".");
            sb.Append(string.IsNullOrWhiteSpace(userInfo) ? "*" : userInfo);
            sb.Append(",ItemType: ");
            sb.Append(itemType.ToString());
            sb.Append(",ItemId: ");
            sb.Append(itemId.HasValue ? itemId.Value.ToString() : "*");
            sb.Append(".");
            sb.Append(itemGuid.HasValue ? itemGuid.Value.ToString("N") : "*");
            sb.Append(".");
            sb.Append(string.IsNullOrWhiteSpace(itemInfo) ? "*" : itemInfo);
            if (!string.IsNullOrWhiteSpace(notes))
            {
                sb.Append(",Notes: \"");
                sb.Append(notes);
                sb.Append("\"");
            }

            Trace.WriteLine(sb.ToString(), "Audit");
        }
    }
}
