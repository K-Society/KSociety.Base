using System;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.Infra.Shared.Interface.Identity;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.AspNetCore.Identity;

namespace KSociety.Base.Infra.Shared.Class.Identity
{
    public class UnitOfWork<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : DisposableObject, IDatabaseUnitOfWork
        where TContext : DbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> _dbFactory;
        private TContext _context;

        public UnitOfWork(IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public TContext Context => _context ??= _dbFactory.Get();

        public string GetConnectionString()
        {
            return Context.GetConnectionString();
        }

        public ValueTask<string> GetConnectionStringAsync()
        {
            return Context.GetConnectionStringAsync();
        }

        public ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken)
        {
            return Context.GetConnectionStringAsync(cancellationToken);
        }

        public bool Exists()
        {
            return Context.Exists();
        }

        public bool EnsureCreated()
        {
            return Context.EnsureCreated();
        }

        public async ValueTask<bool> EnsureCreatedAsync()
        {
            return await Context.EnsureCreatedAsync().ConfigureAwait(false);
        }

        public async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken)
        {
            return await Context.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool EnsureDeleted()
        {
            return Context.EnsureDeleted();
        }

        public async ValueTask<bool> EnsureDeletedAsync()
        {
            return await Context.EnsureDeletedAsync().ConfigureAwait(false);
        }

        public async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken)
        {
            return await Context.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Migrate()
        {
            Context.Migrate();
        }

        public async ValueTask MigrateAsync()
        {
            await Context.MigrateAsync().ConfigureAwait(false);
        }

        public async ValueTask MigrateAsync(CancellationToken cancellationToken)
        {
            await Context.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        public void BeginTransaction()
        {
            Context.BeginTransaction();
        }

        public async ValueTask BeginTransactionAsync()
        {
            await Context.BeginTransactionAsync().ConfigureAwait(false);
        }

        public async ValueTask BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await Context.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public int Commit()
        {
            return Context.Commit();
        }

        public async ValueTask<int> CommitAsync()
        {
            return await Context.CommitAsync().ConfigureAwait(false);
        }

        public async ValueTask<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await Context.CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        public void CommitTransaction()
        {
            Context.CommitTransaction();
        }

        public async ValueTask CommitTransactionAsync()
        {
            await Context.CommitTransactionAsync().ConfigureAwait(false);
        }

        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await Context.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Rollback()
        {
            Context.Rollback();
        }

        public async ValueTask RollbackAsync()
        {
            await Context.RollbackAsync().ConfigureAwait(false);
        }

        public async ValueTask RollbackAsync(CancellationToken cancellationToken)
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
