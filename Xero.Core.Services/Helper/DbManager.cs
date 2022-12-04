using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xero.Core.Services.Helper
{
    public static class DbManager
    {
        public static string DbName;
        public static string ConnString;

        public static string GetDbConnectionString(string dbName, string connString)
        {
            return DbConnectionManager.GetConnectionString(dbName, connString);
        }
    }
}
