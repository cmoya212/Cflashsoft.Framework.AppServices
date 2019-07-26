using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cflashsoft.Framework.Types
{
    /// <summary>
    /// Represents a container for a collection of items together with the count of items in the original store.
    /// </summary>
    public class PagedList<T>
    {
        /// <summary>
        /// Total number of items in the store.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Page represented by this instance of the class.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Limit of items requested from the store.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Collection of items.
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
