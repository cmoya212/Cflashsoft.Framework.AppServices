using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cflashsoft.Framework.EntityAppServices
{
    //The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
    //for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. 
    //Make sure the provider assembly is available to the running application. 
    //See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.
    internal class EntityFrameworkSqlFix
    {
        public EntityFrameworkSqlFix()
        {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
