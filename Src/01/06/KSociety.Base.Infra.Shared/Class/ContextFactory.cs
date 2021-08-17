using KSociety.Base.Infra.Shared.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace KSociety.Base.Infra.Shared.Class
{
    public class ContextFactory<TContext> : IContextFactory<TContext> where TContext : DatabaseContext
    {
        public virtual TContext CreateDbContext(string[] args)
        {
            TContext output = null;
            var dbEngine = DatabaseEngine.Sqlserver;
            var migrationsAssembly = string.Empty;
            var connectionStringName = string.Empty;
            var connectionString = string.Empty;

            if (args.Length < 2) return output;

            if (args.Length > 0)
            {
                switch (args[0])
                //switch (databaseEngine)
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
            }

            if (args.Length >= 2)
            {
                connectionStringName = args[1];

                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
                connectionString = config.GetConnectionString(connectionStringName);
            }

            if (args.Length >= 3)
            {
                migrationsAssembly = args[2];
            }

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
