using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xero.Core.Data.Entities;
using Xero.Core.Services.Helper;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Data.SqlClient;

namespace Xero.Core.Services.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){
    
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DbManager.DbName != null)
            {
                var dbName = DbManager.DbName;
                var connString = DbManager.ConnString;

                var initString = DbManager.GetDbConnectionString(dbName, connString);
                initString = initString + @$"Database={dbName}";
                optionsBuilder.UseSqlServer(initString);
            }
        }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<ItemTypes> ItemTypes { get; set; }
    }
}
