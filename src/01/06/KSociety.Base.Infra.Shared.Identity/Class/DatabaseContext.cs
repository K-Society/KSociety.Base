// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Identity.Class
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using KSociety.Base.Infra.Abstraction.Interface;
    using KSociety.Base.Infra.Shared.Class.SqlGenerator;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>,
            IDatabaseUnitOfWork
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        protected readonly
            ILogger<DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>?
            Logger;

        protected static ILoggerFactory? LoggerFactory;

        private IDbContextTransaction? _transaction;
        private bool _debug;
        private readonly IDatabaseConfiguration? _configuration;

        public DatabaseContext(DbContextOptions option)
            : base(option)
        {
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole(configure =>
                {
                    configure.LogToStandardErrorThreshold = LogLevel.Trace;
                });
            });

            this.Logger = LoggerFactory
                .CreateLogger<DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
                    TUserToken>>();
        }

        public DatabaseContext(ILoggerFactory loggerFactory, IDatabaseConfiguration configuration)
        {
            LoggerFactory = loggerFactory;
            this._configuration = configuration;
            //_mediator = mediator;
            this.Logger = LoggerFactory
                .CreateLogger<DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
                    TUserToken>>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Logger?.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            //string path = Assembly.GetExecutingAssembly().Location;

            //Console.WriteLine(path);
            //Configuration config = ConfigurationManager.OpenExeConfiguration(path);
            //Console.WriteLine("OpenExeConfiguration: " + config.FilePath);


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
                    this._debug = true;

                    if (String.IsNullOrEmpty(this._configuration.MigrationsAssembly))
                    {
                        optionsBuilder
                            .UseLoggerFactory(LoggerFactory)
                            .EnableSensitiveDataLogging()
                            .UseSqlServer(this._configuration.ConnectionString);
                    }
                    else
                    {
                        optionsBuilder
                            .UseLoggerFactory(LoggerFactory)
                            .EnableSensitiveDataLogging()
                            .UseSqlServer(this._configuration.ConnectionString,
                                sql =>
                                {
                                    sql.MigrationsAssembly(this._configuration.MigrationsAssembly);
                                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                });
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(this._configuration.MigrationsAssembly))
                    {
                        optionsBuilder
                            .UseSqlServer(this._configuration.ConnectionString);
                    }
                    else
                    {
                        optionsBuilder
                            .UseSqlServer(this._configuration.ConnectionString,
                                sql =>
                                {
                                    sql.MigrationsAssembly(this._configuration.MigrationsAssembly);
                                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                });
                    }
                }
            }

            //ToDo
            optionsBuilder.ReplaceService<IMigrationsSqlGenerator, SqlServerGenerator>();

            //No!
            //try
            //{
            //    Database.EnsureCreated();
            //}
            //catch (Exception ex)
            //{
            //    Logger?.LogError("EnsureCreated: " + ex.Message + " - " + ex.StackTrace);
            //}
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

        //public bool? Exists()
        //{
        //    var exists =
        //        (Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator)?.Exists();

        //    return exists.HasValue && exists.Value;
        //}

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
            var output = false;
            try
            {
                output = this.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual async ValueTask<bool?> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            var output = false;
            try
            {
                output = await this.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual bool? EnsureDeleted()
        {
            var output = false;
            try
            {
                output = this.Database.EnsureDeleted();
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public virtual async ValueTask<bool?> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            var output = false;
            try
            {
                output = await this.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return output;
        }

        public void Migrate(string? targetMigration = null)
        {
            var migrator = this.Database.GetInfrastructure().GetService<IMigrator>();
            migrator?.Migrate(targetMigration);
        }

        public async ValueTask MigrateAsync(string? targetMigration = null,
            CancellationToken cancellationToken = default)
        {
            var migrator = this.Database.GetInfrastructure().GetService<IMigrator>();
            //await migrator.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);

            //Database.Mi

            if (migrator is not null)
            {
                await migrator.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
            }
        }

        public string? CreateScript()
        {
            //var migratorAssembly = Database.GetInfrastructure().GetService<IMigrationsAssembly>();
            //migratorAssembly.Assembly.FullName
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
            //var output1 = -1;
            try
            {
                if (this._debug)
                {
                    var entries = this.ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        this.Logger?.LogDebug("CommitAsync entry: " + entry.Entity.GetType().FullName + " " + entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                //_mediator.DispatchDomainEvents(this);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = this.SaveChanges();
                //_transaction.Commit();
                this.Logger?.LogTrace("Commit: " + this.GetType().FullName + "." +
                                      System.Reflection.MethodBase.GetCurrentMethod()?.Name + " Result: " + result);
                //output1 = result;
                return result;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError("Commit: " + ex.Source + " - " + ex.Message + " - " + ex.InnerException);
                //output1 = -1;
                return -1;
            }
            //finally
            //{
            //    _transaction.Dispose();
            //}
        }

        public async ValueTask<int?> CommitAsync(CancellationToken cancellationToken = default)
        {
            //var output = -1;
            try
            {
                if (this._debug)
                {
                    var entries = this.ChangeTracker.Entries();

                    foreach (var entry in entries)
                    {
                        this.Logger?.LogDebug("CommitAsync entry: " + entry.Entity.GetType().FullName + " " + entry.State);
                    }
                }

                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                //await _mediator.DispatchDomainEventsAsync(this, cancellationToken);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                var result = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                //await _transaction.CommitAsync(cancellationToken);
                this.Logger?.LogTrace("CommitAsync: " + this.GetType().FullName + "." +
                                      System.Reflection.MethodBase.GetCurrentMethod()?.Name + " Result: " + result);
                //output = result;
                return result;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError("CommitAsync: " + ex.Source + " - " + ex.Message + " - " + ex.InnerException);
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
            if (this._transaction == null) { return; }
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
            if (this._transaction == null) { return; }
            await this._transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await this._transaction.DisposeAsync().ConfigureAwait(false);
        }
    }
}
