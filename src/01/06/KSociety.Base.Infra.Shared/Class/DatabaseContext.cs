namespace KSociety.Base.Infra.Shared.Class
{
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

            this.Logger = LoggerFactory.CreateLogger<DatabaseContext>();
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> option)
            : base(option)
        {

            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            this.Logger = LoggerFactory.CreateLogger<DatabaseContext>();
        }

        public DatabaseContext(ILoggerFactory loggerFactory, IDatabaseConfiguration configuration, IMediator mediator)
        {
            LoggerFactory = loggerFactory;
            this._configuration = configuration;
            this._mediator = mediator;
            this.Logger = LoggerFactory.CreateLogger<DatabaseContext>();
            this.Logger.LogTrace("DatabaseContext: {0}", this._configuration.ToString());
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
            if (this._configuration is null || String.IsNullOrEmpty(this._configuration.ConnectionString))
            {
                //string path = Assembly.GetExecutingAssembly().Location;
                //Configuration config = ConfigurationManager.OpenExeConfiguration(path);

                //optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=Master.CtrDb;AttachDbFilename=C:\DB\CtrDB.mdf;Integrated Security=True;Connect Timeout=30;");
            }
            else
            {
                if (this._configuration.Logging)
                {
                    optionsBuilder.UseLoggerFactory(LoggerFactory)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                }

                switch (this._configuration.DatabaseEngine)
                {
                    case DatabaseEngine.Sqlserver:
                        if (String.IsNullOrEmpty(this._configuration.MigrationsAssembly))
                        {
                            optionsBuilder
                                .UseSqlServer(this._configuration.ConnectionString);
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(this._configuration.LazyLoading)
                                .UseSqlServer(this._configuration.ConnectionString,
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(this._configuration.MigrationsAssembly);
                                        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                    });

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, SqlServerGenerator>();
                        }

                        break;

                    case DatabaseEngine.Sqlite:
                        if (String.IsNullOrEmpty(this._configuration.MigrationsAssembly))
                        {
                            optionsBuilder
                                .UseSqlite(this._configuration.ConnectionString);
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(this._configuration.LazyLoading)
                                .UseSqlite(this._configuration.ConnectionString,
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(this._configuration.MigrationsAssembly);
                                    });

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, SqliteGenerator>();
                        }

                        break;

                    case DatabaseEngine.Npgsql:
                        if (String.IsNullOrEmpty(this._configuration.MigrationsAssembly))
                        {
                            optionsBuilder
                                .UseNpgsql(this._configuration.ConnectionString);
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(this._configuration.LazyLoading)
                                .UseNpgsql(this._configuration.ConnectionString,
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(this._configuration.MigrationsAssembly);
                                        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                    });

                            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, NpgsqlGenerator>();
                        }

                        break;

                    case DatabaseEngine.Mysql:

                        if (String.IsNullOrEmpty(this._configuration.MigrationsAssembly))
                        {
                            optionsBuilder.UseMySql(this._configuration.ConnectionString,
                                ServerVersion.AutoDetect(this._configuration.ConnectionString));
                        }
                        else
                        {
                            optionsBuilder
                                .UseLazyLoadingProxies(this._configuration.LazyLoading)
                                .UseMySql(this._configuration.ConnectionString,
                                    ServerVersion.AutoDetect(this._configuration.ConnectionString),
                                    sql =>
                                    {
                                        sql.MigrationsAssembly(this._configuration.MigrationsAssembly);
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
            return this.Database.GetDbConnection().ConnectionString;
        }

        public ValueTask<string?> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            var result = this.Database.GetDbConnection().ConnectionString;
            return new ValueTask<string?>(result);
        }

        public bool? Exists()
        {
            bool? exists = null;
            try
            {
                var databaseCreator = this.Database.GetService<IDatabaseCreator>();

                if (databaseCreator is not null)
                {
                    if (databaseCreator is RelationalDatabaseCreator relationalDatabaseCreator)
                    {
                        exists = relationalDatabaseCreator.Exists();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "Exists: ");
            }

            return exists.HasValue && exists.Value;
        }

        public async ValueTask<bool?> ExistsAsync(CancellationToken cancellationToken = default)
        {
            bool? exists = null;

            try
            {
                var databaseCreator = this.Database.GetService<IDatabaseCreator>();

                if (databaseCreator is not null)
                {
                    if (databaseCreator is RelationalDatabaseCreator relationalDatabaseCreator)
                    {
                        exists = await relationalDatabaseCreator.ExistsAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "ExistsAsync: ");
            }

            return exists.HasValue && exists.Value;
        }

        public virtual bool? EnsureCreated()
        {
            this.Logger?.LogTrace("EnsureCreated");
            var output = false;
            try
            {
                output = this.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EnsureCreated: ");
            }

            return output;
        }

        public virtual async ValueTask<bool?> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("EnsureCreatedAsync");
            var output = false;
            try
            {
                output = await this.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EnsureCreatedAsync: ");
            }

            return output;
        }

        public virtual bool? EnsureDeleted()
        {
            this.Logger?.LogTrace("EnsureDeleted");
            var output = false;
            try
            {
                output = this.Database.EnsureDeleted();
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EnsureDeleted: ");
            }

            return output;
        }

        public virtual async ValueTask<bool?> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("EnsureDeletedAsync");
            var output = false;
            try
            {
                output = await this.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EnsureDeletedAsync: ");
            }

            return output;
        }

        public void Migrate(string? targetMigration = null)
        {
            this.Logger?.LogTrace("Migrate {0}", targetMigration);
            var migrator = this.Database.GetInfrastructure().GetService<IMigrator>();
            migrator?.Migrate(targetMigration);
        }

        public async ValueTask MigrateAsync(string? targetMigration = null,
            CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("MigrateAsync {0}", targetMigration);
            var migrator = this.Database.GetInfrastructure().GetService<IMigrator>();
            await migrator.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
        }

        public string? CreateScript()
        {
            var migrator = this.Database.GetInfrastructure().GetService<IMigrator>();
            return migrator?.GenerateScript();
        }

        public virtual void BeginTransaction()
        {
            this._transaction = this.Database.BeginTransaction();
        }

        public virtual async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            this._transaction = await this.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }


        public int? Commit()
        {
            //var output = -1;
            try
            {
                if (this._configuration.Logging)
                {
                    var entries = this.ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        this.Logger?.LogDebug("CommitAsync entry: {0} {1}", entry.Entity.GetType().FullName, entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                this._mediator?.DispatchDomainEvents(this);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = this.SaveChanges();
                //_transaction.Commit();
                this.Logger?.LogTrace("Commit: {0}.{1} Result: {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, result);
                return result;
                //output = result;
            }
            catch (DbUpdateConcurrencyException duce)
            {
                this.Logger?.LogWarning(duce, "Commit DbUpdateConcurrencyException: ");
                return -3;
                //output = -3;
            }
            catch (DbUpdateException due)
            {
                this.Logger?.LogWarning(due, "Commit DbUpdateException: ");
                return -2;
                //output = -2;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "Commit Exception: ");
                return -1;
                //output = -1;
            }
            //finally
            //{
            //    _transaction.Dispose();
            //}

            //return output;
        }

        public async ValueTask<int?> CommitAsync(CancellationToken cancellationToken = default)
        {
            //var output = -1;
            try
            {
                if (this._configuration.Logging)
                {
                    var entries = this.ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        this.Logger?.LogDebug("CommitAsync entry: {0} {1}", entry.Entity.GetType().FullName, entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 

                if (this._mediator != null)
                {
                    await this._mediator.DispatchDomainEventsAsync(this, cancellationToken).ConfigureAwait(false);
                }

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                //await _transaction.CommitAsync(cancellationToken);
                this.Logger?.LogTrace("CommitAsync: {0}.{1} Result: {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, result);
                //output = result;
                return result;
            }
            catch (DbUpdateConcurrencyException duce)
            {
                //duce.
                this.Logger?.LogWarning(duce, "CommitAsync DbUpdateConcurrencyException: ");
                //Logger?.LogWarning("CommitAsync: Rows affected: {0}", duce.Entries.Count);
                //output = -3;
                return -3;
            }
            catch (DbUpdateException due)
            {
                this.Logger?.LogWarning(due, "CommitAsync DbUpdateException: ");
                //output = -2;
                return -2;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "CommitAsync Exception: ");
                //output = -1;
                return -1;
            }

            //finally
            //{
            //    await _transaction.DisposeAsync();
            //}
            //return output;
        }

        public void CommitTransaction()
        {
            try
            {
                this._transaction?.Commit();
            }
            finally
            {
                this._transaction?.Dispose();
            }
        }

        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (this._transaction == null) {return;}
            try
            {
                await this._transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await this._transaction.DisposeAsync().ConfigureAwait(false);
            }
        }

        public virtual void Rollback()
        {
            this._transaction?.Rollback();
            this._transaction?.Dispose();
        }

        public virtual async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (this._transaction == null) {return;}
            await this._transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await this._transaction.DisposeAsync().ConfigureAwait(false);
        }
    }
}
