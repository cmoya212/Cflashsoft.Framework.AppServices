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
    /// Error logger implementation that simply echoes to the console.
    /// </summary>
    public class TraceErrorLogger : ErrorLoggerBase
    {
        /// <summary>
        /// Log an error to the console.
        /// </summary>
        /// <param name="exception">The underlying exception of the error.</param>
        /// <param name="info">Optional additional info to record in the store.</param>
        /// <param name="code">Optional code to record in the store.</param>
        /// <param name="applicationId">The id of the application that caused the error.</param>
        /// <param name="userId">The unique id of the user that caused the error.</param>
        /// <param name="userGuid">The unique id of the user that caused the error.</param>
        /// <param name="userInfo">The unique id of the user that caused the error.</param>
        /// <returns>Return value should be a unique number identifying the log entry.</returns>
        public override int LogError(Exception exception, string info, short code, int applicationId, int? userId, Guid? userGuid, string userInfo)
        {
            TraceWrite(exception, info, code, applicationId, userId, userGuid, userInfo);

            return 0;
        }

        /// <summary>
        /// Static method to write an error to the console.
        /// </summary>
        /// <param name="exception">The underlying exception of the error.</param>
        /// <param name="info">Optional additional info to record in the store.</param>
        /// <param name="code">Optional code to record in the store.</param>
        /// <param name="applicationId">The id of the application that caused the error.</param>
        /// <param name="userId">The unique id of the user that caused the error.</param>
        /// <param name="userGuid">The unique id of the user that caused the error.</param>
        /// <param name="userInfo">The unique id of the user that caused the error.</param>
        public static void TraceWrite(Exception exception, string info, short code, int applicationId, int? userId, Guid? userGuid, string userInfo)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Application Error,");
            sb.Append(code.ToString());
            sb.Append(",App:");
            sb.Append(applicationId.ToString());
            sb.Append(",UserId: ");
            if (userId.HasValue)
                sb.Append(userId.Value);
            else if (userGuid.HasValue)
                sb.Append(userGuid.Value.ToString("N"));
            else if (!string.IsNullOrWhiteSpace(userInfo))
                sb.Append(userInfo);
            else
                sb.Append("Unknown");
            sb.Append(",");
            if (exception != null)
                sb.AppendLine(exception.ToString());
            if (!string.IsNullOrWhiteSpace(info))
                sb.AppendLine(info);

            Trace.WriteLine(sb.ToString(), "Error");
        }
    }
}
