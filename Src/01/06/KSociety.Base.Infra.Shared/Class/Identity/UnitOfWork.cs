using KSociety.Base.Infra.Abstraction.Interface;
using KSociety.Base.Infra.Shared.Interface.Identity;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Class.Identity
{
    public class
        UnitOfWork<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken> : DisposableObject, IDatabaseUnitOfWork
        where TContext : DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        private IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken>? _dbFactory;

        private TContext? _context;
        private IUserStore<TUser>? _userStore;
        private IRoleStore<TRole>? _roleStore;

        public UnitOfWork(
            IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>?
                dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public TContext? Context => _context ??= _dbFactory?.Get();
        public IUserStore<TUser>? UserStore => _userStore ??= _dbFactory?.GetUserStore();
        public IRoleStore<TRole>? RoleStore => _roleStore ??= _dbFactory?.GetRoleStore();

        public string? GetConnectionString()
        {
            return Context?.GetConnectionString();
        }

        public async ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            return await (Context.GetConnectionStringAsync(cancellationToken)
                .ConfigureAwait(false));
        }

        public bool? Exists()
        {
            return Context?.Exists();
        }

        public bool? EnsureCreated()
        {
            return Context?.EnsureCreated();
        }

        public async ValueTask<bool?> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return await Context.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool? EnsureDeleted()
        {
            return Context?.EnsureDeleted();
        }

        public async ValueTask<bool?> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await Context.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Migrate(string? targetMigration = null)
        {
            Context?.Migrate(targetMigration);
        }

        public async ValueTask MigrateAsync(string? targetMigration = null,
            CancellationToken cancellationToken = default)
        {
            await Context.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
        }

        public string? CreateScript()
        {
            return Context?.CreateScript();
        }

        public void BeginTransaction()
        {
            Context?.BeginTransaction();
        }

        public async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await Context.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public int? Commit()
        {
            return Context?.Commit();
        }

        public async ValueTask<int?> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await Context.CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        public void CommitTransaction()
        {
            Context?.CommitTransaction();
        }

        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await Context.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Rollback()
        {
            Context?.Rollback();
        }

        public async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
        {
            await Context.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override void DisposeManagedResources()
        {
            if (_dbFactory != null)
            {
                _dbFactory.Dispose();
                _dbFactory = null;
            }

            if (_context == null) return;
            _context.Dispose();
            _context = null;
        }
    }
}