using KSociety.Base.Infra.Shared.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace KSociety.Base.Infra.Shared.Class
{
    public class ContextFactory<TContext> : IContextFactory<TContext> where TContext : DatabaseContext
    {
        //public virtual TContext CreateDbContext(string[] args)
        public virtual TContext CreateDbContext(string databaseEngine, string connectionString, string migrationsAssembly)
        {
            //DatabaseEngine dbEngine = DatabaseEngine.Sqlserver;

            //if (args.Length > 0)
            //{
            //    switch (args[0])
            //    {
            //        case "Sqlserver":
            //            dbEngine = DatabaseEngine.Sqlserver;
            //            break;
            //        case "Sqlite":
            //            dbEngine = DatabaseEngine.Sqlite;
            //            break;
            //        case "Npgsql":
            //            dbEngine = DatabaseEngine.Npgsql;
            //            break;
            //    }
            //}

            //var optionBuilder = new DbContextOptionsBuilder<ComContext>();

            //switch (dbEngine)
            //{
            //    case DatabaseEngine.Sqlserver:
            //        optionBuilder
            //            .UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=KSociety.Com;AttachDbFilename=C:\DB\ComDb.mdf;Integrated Security=True;Connect Timeout=30;", sql => sql.MigrationsAssembly("KSociety.Com.Infra.Migration.SqlServer"));

            //        break;

            //    case DatabaseEngine.Sqlite:
            //        optionBuilder
            //            .UseSqlite(@"Data Source=C:\DB\ComDb.db;", sql => sql.MigrationsAssembly("KSociety.Com.Infra.Migration.Sqlite"));

            //        break;

            //    case DatabaseEngine.Npgsql:
            //        break;
            //}

            //return new ComContext(optionBuilder.Options);

            TContext output = null;
            var dbEngine = DatabaseEngine.Sqlserver;
            //var migrationsAssembly = string.Empty;
            //var connectionString = string.Empty;

            //if (args.Length < 2) return output;

            //if (args.Length > 0)
            //{
                //switch (args[0])
                switch (databaseEngine)
            {
                    case "Sqlserver":
                        dbEngine = DatabaseEngine.Sqlserver;
                        break;
                    case "Sqlite":
                        dbEngine = DatabaseEngine.Sqlite;
                        break;
                    case "Npgsql":
                        dbEngine = DatabaseEngine.Npgsql;
                        break;
                    case "MySql":
                        dbEngine = DatabaseEngine.Mysql;
                        break;
                }
            //}

            //if (args.Length >= 2)
            //{
            //    connectionString = args[1];
            //}

            //if (args.Length >= 2)
            //{
            //    migrationsAssembly = args[1];
            //}

            var optionBuilder = new DbContextOptionsBuilder<TContext>();

            switch (dbEngine)
            {
                case DatabaseEngine.Sqlserver:
                    optionBuilder
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
                        .UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                    break;

                case DatabaseEngine.Sqlite:
                    optionBuilder
                        .UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                    break;

                case DatabaseEngine.Npgsql:
                    break;

                case DatabaseEngine.Mysql:
                    optionBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            //try
            //{

            //}
            //catch(Exception ex)
            //{

            //}

            output = (TContext)Activator.CreateInstance(typeof(TContext), optionBuilder.Options);

            return output; //new TContext(optionBuilder.Options);
        }
    }
}
