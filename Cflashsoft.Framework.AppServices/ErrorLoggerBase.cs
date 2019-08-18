using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Represents a class that can log errors to a store.
    /// </summary>
    public abstract class ErrorLoggerBase
    {
        /// <summary>
        /// Inititializes a new instance of the ErrorLogger class.
        /// </summary>
        public ErrorLoggerBase()
        {

        }

        /// <summary>
        /// Log an error in the store.
        /// </summary>
        /// <param name="exception">The underlying exception of the error.</param>
        /// <param name="info">Optional additional info to record in the store.</param>
        /// <param name="code">Optional code to record in the store.</param>
        /// <param name="applicationId">The id of the application that caused the error.</param>
        /// <param name="userId">The unique id of the user that caused the error.</param>
        /// <param name="userGuid">The unique id of the user that caused the error.</param>
        /// <param name="userInfo">The unique id of the user that caused the error.</param>
        /// <returns>Return value should be a unique number identifying the log entry.</returns>
        public abstract int LogError(Exception exception, string info, short code, int applicationId, int? userId, Guid? userGuid, string userInfo);
    }
}
