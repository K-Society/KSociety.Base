using KSociety.Base.Infra.Shared.Class.SqlGenerator;
using System;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Infra.Shared.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Class
{
    public class DatabaseContext : DbContext, IDatabaseUnitOfWork
    {
        public static readonly string DefaultSchema = "ksociety";

        protected readonly ILogger<DatabaseContext> Logger;

        protected readonly ILoggerFactory LoggerFactory;

        private IDbContextTransaction _transaction;
        private bool _debug = false;

        private readonly IDatabaseConfiguration _configuration;
        private readonly IMediator _mediator;

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

        public DatabaseContext(ILoggerFactory loggerFactory, IDatabaseConfiguration configuration, IMediator mediator)
        {
            LoggerFactory = loggerFactory;
            _configuration = configuration;
            _mediator = mediator;
            Logger = LoggerFactory.CreateLogger<DatabaseContext>();
        }

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
                    _debug = true;

                    optionsBuilder.UseLoggerFactory(LoggerFactory)
                        .EnableSensitiveDataLogging();
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
                                .UseSqlServer(_configuration.ConnectionString,
                                    sql => sql.MigrationsAssembly(_configuration.MigrationsAssembly));

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
                                .UseSqlite(_configuration.ConnectionString,
                                    sql => sql.MigrationsAssembly(_configuration.MigrationsAssembly));

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
                                .UseNpgsql(_configuration.ConnectionString,
                                    sql => sql.MigrationsAssembly(_configuration.MigrationsAssembly));

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, NpgsqlGenerator>();
                        }
                        break;

                    case DatabaseEngine.Mysql:
                        if (string.IsNullOrEmpty(_configuration.MigrationsAssembly))
                        {
                            optionsBuilder.UseMySql(_configuration.ConnectionString, ServerVersion.AutoDetect(_configuration.ConnectionString));
                        }
                        else
                        {
                            optionsBuilder.UseMySql(_configuration.ConnectionString, ServerVersion.AutoDetect(_configuration.ConnectionString),
                                sql => sql.MigrationsAssembly(_configuration.MigrationsAssembly));

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

        public ValueTask<string> GetConnectionStringAsync()
        {
            var result = Database.GetDbConnection().ConnectionString;
            return new ValueTask<string>(result);
        }

        public ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken)
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
            var output = false;
            try
            {
                output = Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual async ValueTask<bool> EnsureCreatedAsync()
        {
            var output = false;
            try
            {
                output = await Database.EnsureCreatedAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken)
        {
            var output = false;
            try
            {
                output = await Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual bool EnsureDeleted()
        {
            var output = false;
            try
            {
                output = Database.EnsureDeleted();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual async ValueTask<bool> EnsureDeletedAsync()
        {
            var output = false;
            try
            {
                output = await Database.EnsureDeletedAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken)
        {
            var output = false;
            try
            {
                output = await Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public void Migrate()
        {
            Database.Migrate();
        }

        public async ValueTask MigrateAsync()
        {
            await Database.MigrateAsync().ConfigureAwait(false);
        }

        public async ValueTask MigrateAsync(CancellationToken cancellationToken)
        {
            await Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void BeginTransaction()
        {
            _transaction = Database.BeginTransaction();
        }

        public virtual async ValueTask BeginTransactionAsync()
        {
            _transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);
        }

        public virtual async ValueTask BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }


        public int Commit()
        {
            var output = -1;
            try
            {
                if (_debug)
                {
                    var entries = ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        Logger?.LogDebug("CommitAsync entry: " + entry.Entity.GetType().FullName + " " + entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                _mediator.DispatchDomainEvents(this);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = SaveChanges();
                //_transaction.Commit();
                Logger?.LogTrace("Commit: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " Result: " + result);
                output = result;
            }
            catch (Exception ex)
            {
                Logger?.LogError("Commit: " + ex.Source + " - " + ex.Message + " - " + ex.InnerException);
                output = -1;
            }
            //finally
            //{
            //    _transaction.Dispose();
            //}

            return output;
        }

        public async ValueTask<int> CommitAsync()
        {
            var output = -1;
            try
            {
                if (_debug)
                {
                    var entries = ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        Logger?.LogDebug("CommitAsync entry: " + entry.Entity.GetType().FullName + " " + entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                await _mediator.DispatchDomainEventsAsync(this).ConfigureAwait(false);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = await SaveChangesAsync().ConfigureAwait(false);
                //await _transaction.CommitAsync(cancellationToken);
                Logger?.LogTrace("CommitAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " Result: " + result);
                output = result;
            }
            catch (Exception ex)
            {
                Logger?.LogError("CommitAsync: " + ex.Source + " - " + ex.Message + " - " + ex.InnerException);
                output = -1;
            }
            //finally
            //{
            //    await _transaction.DisposeAsync();
            //}
            return output;
        }

        public async ValueTask<int> CommitAsync(CancellationToken cancellationToken)
        {
            var output = -1;
            try
            {
                if (_debug)
                {
                    var entries = ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        Logger?.LogDebug("CommitAsync entry: " + entry.Entity.GetType().FullName + " " + entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                await _mediator.DispatchDomainEventsAsync(this, cancellationToken).ConfigureAwait(false);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                //await _transaction.CommitAsync(cancellationToken);
                Logger?.LogTrace("CommitAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " Result: " + result);
                output = result;
            }
            catch (Exception ex)
            {
                Logger?.LogError("CommitAsync: " + ex.Source + " - " + ex.Message + " - " + ex.InnerException);
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
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
            }
        }

        public async ValueTask CommitTransactionAsync()
        {
            try
            {
                await _transaction.CommitAsync().ConfigureAwait(false);
            }
            finally
            {
                await _transaction.DisposeAsync().ConfigureAwait(false);
            }
        }

        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken)
        {
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
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public virtual async ValueTask RollbackAsync()
        {
            await _transaction.RollbackAsync().ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
        }

        public virtual async ValueTask RollbackAsync(CancellationToken cancellationToken)
        {
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
        }
    }
}
