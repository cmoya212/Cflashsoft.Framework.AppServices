using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Represents the exception that is thrown when an error happens in app services.
    /// </summary>
    public class AppServicesException : Exception
    {
        /// <summary>
        /// Inititializes a new instance of the AppServicesException class.
        /// </summary>
        public AppServicesException()
            : base()
        {

        }

        /// <summary>
        /// Inititializes a new instance of the AppServicesException class with the specified string.
        /// </summary>
        public AppServicesException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Inititializes a new instance of the AppServicesException class with the specified string and reference to the exception that cause the error.
        /// </summary>
        public AppServicesException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
