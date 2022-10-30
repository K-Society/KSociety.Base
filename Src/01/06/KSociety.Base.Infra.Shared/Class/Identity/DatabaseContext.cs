using KSociety.Base.Infra.Shared.Class.SqlGenerator;
using System;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace KSociety.Base.Infra.Shared.Class.Identity;

public class DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> 
    : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IDatabaseUnitOfWork
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>, new()
    where TUserRole : IdentityUserRole<TKey>, new()
    where TUserLogin : IdentityUserLogin<TKey>, new()
    where TRoleClaim : IdentityRoleClaim<TKey>, new()
    where TUserToken : IdentityUserToken<TKey>, new()
{
    protected readonly ILogger<DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>> Logger;

    protected static ILoggerFactory LoggerFactory;

    private IDbContextTransaction _transaction;
    private bool _debug = false;
    private readonly IDatabaseConfiguration _configuration;

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

        Logger = LoggerFactory.CreateLogger<DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
    }

    public DatabaseContext(ILoggerFactory loggerFactory, IDatabaseConfiguration configuration)
    {
        LoggerFactory = loggerFactory;
        _configuration = configuration;
        //_mediator = mediator;
        Logger = LoggerFactory.CreateLogger<DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Logger?.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        //string path = Assembly.GetExecutingAssembly().Location;

        //Console.WriteLine(path);
        //Configuration config = ConfigurationManager.OpenExeConfiguration(path);
        //Console.WriteLine("OpenExeConfiguration: " + config.FilePath);


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

                if (string.IsNullOrEmpty(_configuration.MigrationsAssembly))
                {
                    optionsBuilder
                        .UseLoggerFactory(LoggerFactory)
                        .EnableSensitiveDataLogging()
                        .UseSqlServer(_configuration.ConnectionString);
                }
                else
                {
                    optionsBuilder
                        .UseLoggerFactory(LoggerFactory)
                        .EnableSensitiveDataLogging()
                        .UseSqlServer(_configuration.ConnectionString, sql => sql.MigrationsAssembly(_configuration.MigrationsAssembly));
                }
            }
            else
            {
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
        var output = false;
        try
        {
            output = Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
        }

        return output;
    }

    public virtual async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        var output = false;
        try
        {
            output = await Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
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
            Logger.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
        }

        return output;
    }

    public virtual async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
    {
        var output = false;
        try
        {
            output = await Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
        }

        return output;
    }

    public void Migrate(string targetMigration = null)
    {
        var migrator = Database.GetInfrastructure().GetService<IMigrator>();
        migrator.Migrate(targetMigration);
    }

    public async ValueTask MigrateAsync(string targetMigration = null, CancellationToken cancellationToken = default)
    {
        var migrator = Database.GetInfrastructure().GetService<IMigrator>();
        await migrator.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);

        //Database.Mi
    }

    public string CreateScript()
    {
        //var migratorAssembly = Database.GetInfrastructure().GetService<IMigrationsAssembly>();
        //migratorAssembly.Assembly.FullName
        var migrator = Database.GetInfrastructure().GetService<IMigrator>();
        return migrator.GenerateScript();
    }

    public virtual void BeginTransaction()
    {
        _transaction = Database.BeginTransaction();
    }

    public virtual async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
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
            //_mediator.DispatchDomainEvents(this);

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

    public async ValueTask<int> CommitAsync(CancellationToken cancellationToken = default)
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
            //await _mediator.DispatchDomainEventsAsync(this, cancellationToken);

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

    public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
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

    public virtual async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        await _transaction.DisposeAsync().ConfigureAwait(false);
    }
}