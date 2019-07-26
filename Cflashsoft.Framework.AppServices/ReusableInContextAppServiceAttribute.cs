using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cflashsoft.Framework.AppServices
{
    /// <summary>
    /// Specifies whether a new instance of a UoW service class is created by the CreateAppService&lt;T&gt; function or a previous instance in the app context is reused.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ReusableInContextAppServiceAttribute : Attribute
    {
    }
}
