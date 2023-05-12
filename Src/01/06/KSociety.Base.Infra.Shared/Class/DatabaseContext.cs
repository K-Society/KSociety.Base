using KSociety.Base.Infra.Abstraction.Class;
using KSociety.Base.Infra.Abstraction.Interface;
using KSociety.Base.Infra.Shared.Class.SqlGenerator;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Class
{
    ///<inheritdoc cref="IDatabaseUnitOfWork"/>
    public class DatabaseContext : DbContext, IDatabaseUnitOfWork
    {
        public const string DefaultSchema = "ksociety";

        protected readonly ILogger<DatabaseContext>? Logger;

        protected static ILoggerFactory? LoggerFactory;

        private IDbContextTransaction? _transaction;

        private readonly IDatabaseConfiguration? _configuration;
        private readonly IMediator? _mediator;

        #region [Constructor]

        public DatabaseContext()
        {

        }

        public DatabaseContext(DbContextOptions option)
            : base(option)
        {

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            Logger = LoggerFactory.CreateLogger<DatabaseContext>();
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> option)
            : base(option)
        {

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            Logger = LoggerFactory.CreateLogger<DatabaseContext>();
        }

        public DatabaseContext(ILoggerFactory loggerFactory, IDatabaseConfiguration configuration, IMediator mediator)
        {
            LoggerFactory = loggerFactory;
            _configuration = configuration;
            _mediator = mediator;
            Logger = LoggerFactory.CreateLogger<DatabaseContext>();
            Logger.LogTrace("DatabaseContext: {0}", _configuration.ToString());
        }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Logger?.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            //string path = Assembly.GetExecutingAssembly().Location;

            //Console.WriteLine(path);
            //Configuration config = ConfigurationManager.OpenExeConfiguration(path);
            //Console.WriteLine("OpenExeConfiguration: " + config.FilePath);
            //string conn = config.ConnectionStrings


            //Console.WriteLine("ConnectionString: " + _configuration.ConnectionString);
            if (_configuration is null || string.IsNullOrEmpty(_configuration.ConnectionString))
            {
                //string path = Assembly.GetExecutingAssembly().Location;
                //Configuration config = ConfigurationManager.OpenExeConfiguration(path);

                //optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=Master.CtrDb;AttachDbFilename=C:\DB\CtrDB.mdf;Integrated Security=True;Connect Timeout=30;");
            }
            else
            {
                if (_configuration.Logging)
                {
                    optionsBuilder.UseLoggerFactory(LoggerFactory)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                }

                switch (_configuration.DatabaseEngine)
                {
                    case DatabaseEngine.Sqlserver:
                        if (string.IsNullOrEmpty(_configuration.MigrationsAssembly))
                        {
                            optionsBuilder
                                .UseSqlServer(_configuration.ConnectionString);
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(_configuration.LazyLoading)
                                .UseSqlServer(_configuration.ConnectionString,
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(_configuration.MigrationsAssembly);
                                        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                    });

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, SqlServerGenerator>();
                        }

                        break;

                    case DatabaseEngine.Sqlite:
                        if (string.IsNullOrEmpty(_configuration.MigrationsAssembly))
                        {
                            optionsBuilder
                                .UseSqlite(_configuration.ConnectionString);
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(_configuration.LazyLoading)
                                .UseSqlite(_configuration.ConnectionString,
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(_configuration.MigrationsAssembly);
                                    });

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, SqliteGenerator>();
                        }

                        break;

                    case DatabaseEngine.Npgsql:
                        if (string.IsNullOrEmpty(_configuration.MigrationsAssembly))
                        {
                            optionsBuilder
                                .UseNpgsql(_configuration.ConnectionString);
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(_configuration.LazyLoading)
                                .UseNpgsql(_configuration.ConnectionString,
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(_configuration.MigrationsAssembly);
                                        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                    });

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, NpgsqlGenerator>();
                        }

                        break;

                    case DatabaseEngine.Mysql:

                        if (string.IsNullOrEmpty(_configuration.MigrationsAssembly))
                        {
                            optionsBuilder.UseMySql(_configuration.ConnectionString,
                                ServerVersion.AutoDetect(_configuration.ConnectionString));
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(_configuration.LazyLoading)
                                .UseMySql(_configuration.ConnectionString,
                                    ServerVersion.AutoDetect(_configuration.ConnectionString),
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(_configuration.MigrationsAssembly);
                                        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                    });

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, MySqlGenerator>();
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        public string GetConnectionString()
        {
            return Database.GetDbConnection().ConnectionString;
        }

        public ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            var result = Database.GetDbConnection().ConnectionString;
            return new ValueTask<string>(result);
        }

        public bool Exists()
        {
            var exists =
                (Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator)?.Exists();

            return exists.HasValue && exists.Value;
        }

        public virtual bool EnsureCreated()
        {
            Logger?.LogTrace("EnsureCreated");
            var output = false;
            try
            {
                output = Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "EnsureCreated: ");
            }

            return output;
        }

        public virtual async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            Logger?.LogTrace("EnsureCreatedAsync");
            var output = false;
            try
            {
                output = await Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "EnsureCreatedAsync: ");
            }

            return output;
        }

        public virtual bool EnsureDeleted()
        {
            Logger?.LogTrace("EnsureDeleted");
            var output = false;
            try
            {
                output = Database.EnsureDeleted();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "EnsureDeleted: ");
            }

            return output;
        }

        public virtual async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            Logger?.LogTrace("EnsureDeletedAsync");
            var output = false;
            try
            {
                output = await Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "EnsureDeletedAsync: ");
            }

            return output;
        }

        public void Migrate(string? targetMigration = null)
        {
            Logger?.LogTrace("Migrate {0}", targetMigration);
            var migrator = Database.GetInfrastructure().GetService<IMigrator>();
            migrator?.Migrate(targetMigration);
        }

        public async ValueTask MigrateAsync(string? targetMigration = null,
            CancellationToken cancellationToken = default)
        {
            Logger?.LogTrace("MigrateAsync {0}", targetMigration);
            var migrator = Database.GetInfrastructure().GetService<IMigrator>();
            await migrator.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
        }

        public string? CreateScript()
        {
            var migrator = Database.GetInfrastructure().GetService<IMigrator>();
            return migrator?.GenerateScript();
        }

        public virtual void BeginTransaction()
        {
            _transaction = Database.BeginTransaction();
        }

        public virtual async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }


        public int? Commit()
        {
            var output = -1;
            try
            {
                if (_configuration.Logging)
                {
                    var entries = ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        Logger?.LogDebug("CommitAsync entry: {0} {1}", entry.Entity.GetType().FullName, entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                _mediator?.DispatchDomainEvents(this);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = SaveChanges();
                //_transaction.Commit();
                Logger?.LogTrace("Commit: {0}.{1} Result: {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, result);
                output = result;
            }
            catch (DbUpdateConcurrencyException duce)
            {
                Logger?.LogWarning(duce, "Commit DbUpdateConcurrencyException: ");
                output = -3;
            }
            catch (DbUpdateException due)
            {
                Logger?.LogWarning(due, "Commit DbUpdateException: ");
                output = -2;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Commit Exception: ");
                output = -1;
            }
            //finally
            //{
            //    _transaction.Dispose();
            //}

            return output;
        }

        public async ValueTask<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            var output = -1;
            try
            {
                if (_configuration.Logging)
                {
                    var entries = ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        Logger?.LogDebug("CommitAsync entry: {0} {1}", entry.Entity.GetType().FullName, entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 

                if (_mediator != null)
                {
                    await _mediator.DispatchDomainEventsAsync(this, cancellationToken).ConfigureAwait(false);
                }

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                //await _transaction.CommitAsync(cancellationToken);
                Logger?.LogTrace("CommitAsync: {0}.{1} Result: {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, result);
                output = result;
            }
            catch (DbUpdateConcurrencyException duce)
            {
                //duce.
                Logger?.LogWarning(duce, "CommitAsync DbUpdateConcurrencyException: ");
                //Logger?.LogWarning("CommitAsync: Rows affected: {0}", duce.Entries.Count);
                output = -3;
            }
            catch (DbUpdateException due)
            {
                Logger?.LogWarning(due, "CommitAsync DbUpdateException: ");
                output = -2;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "CommitAsync Exception: ");
                output = -1;
            }

            //finally
            //{
            //    await _transaction.DisposeAsync();
            //}
            return output;
        }

        public void CommitTransaction()
        {
            try
            {
                _transaction?.Commit();
            }
            finally
            {
                _transaction?.Dispose();
            }
        }

        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null) return;
            try
            {
                await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
            }
        }

        public virtual void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }

        public virtual async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
        }
    }
}