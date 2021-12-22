using KSociety.Base.Infra.Shared.Class.SqlGenerator;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace KSociety.Base.Infra.Shared.Class;

public class ContextFactory<TContext> : IContextFactory<TContext> where TContext : DatabaseContext
{
    private readonly ILogger<ContextFactory<TContext>> _logger;

    public ContextFactory()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        });
        _logger = loggerFactory.CreateLogger<ContextFactory<TContext>>();
    }

    public ContextFactory(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ContextFactory<TContext>>();
    }

    public virtual TContext CreateDbContext(string[] args)
    {
        _logger.LogTrace("ContextFactory CreateDbContext");
        TContext output = null;
        var dbEngine = DatabaseEngine.Sqlserver;
        var migrationsAssembly = string.Empty;
        var connectionString = string.Empty;

        if (args.Length < 2) return output;

        if (args.Length > 0)
        {
            switch (args[0])
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
                case "Mysql":
                    dbEngine = DatabaseEngine.Mysql;
                    break;
            }
        }

        if (args.Length >= 2)
        {
            string connectionStringName = args[1];

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = config.GetConnectionString(connectionStringName);
            _logger.LogTrace(@"ContextFactory CreateDbContext connectionString: {0}", connectionString);
        }

        if (args.Length >= 3)
        {
            migrationsAssembly = args[2];

            _logger.LogTrace(@"ContextFactory CreateDbContext migrationsAssembly: {0}", migrationsAssembly);
        }

        var optionBuilder = new DbContextOptionsBuilder<TContext>();

        switch (dbEngine)
        {
            case DatabaseEngine.Sqlserver:
                optionBuilder
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                optionBuilder.ReplaceService<IMigrationsSqlGenerator, SqlServerGenerator>();
                break;

            case DatabaseEngine.Sqlite:
                optionBuilder
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                optionBuilder.ReplaceService<IMigrationsSqlGenerator, SqliteGenerator>();
                break;

            case DatabaseEngine.Npgsql:
                optionBuilder
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));

                optionBuilder.ReplaceService<IMigrationsSqlGenerator, NpgsqlGenerator>();
                break;

            case DatabaseEngine.Mysql:
                optionBuilder
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), sql => sql.MigrationsAssembly(migrationsAssembly));

                optionBuilder.ReplaceService<IMigrationsSqlGenerator, MySqlGenerator>();
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