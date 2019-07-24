using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// An implementation of the generic standard Dictionary class with a more lenient Get on values that returns a default value if the key is not found rather than an exception.
    /// </summary>
    public class AppOptions : OptionsDictionary<string, object>
    {

        /// <summary>
        /// Inititializes a new instance of the AppOptions class.
        /// </summary>
        public AppOptions()
            :base()
        {

        }

        /// <summary>
        /// Inititializes a new instance of the AppOptions class.
        /// </summary>
        public AppOptions(IEnumerable<KeyValuePair<string, object>> options)
            : base(options.ToDictionary(x => x.Key, x => x.Value))
        {

        }
    }
}
