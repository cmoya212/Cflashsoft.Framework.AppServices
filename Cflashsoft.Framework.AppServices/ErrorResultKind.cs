using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Describes a type of error and its severity.
    /// </summary>
    public enum ErrorResultKind
    {
        /// <summary>
        /// Critical error or exception.
        /// </summary>
        Error,
        /// <summary>
        /// Error that can be safely ignored.
        /// </summary>
        Warning,
        /// <summary>
        /// Violation of business rules or constraints.
        /// </summary>
        Validation
    }
}
