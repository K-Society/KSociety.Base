using System;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.Infra.Shared.Interface.Identity;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.AspNetCore.Identity;

namespace KSociety.Base.Infra.Shared.Class.Identity
{
    public class KbIdentityUnitOfWork<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : DisposableObject, IDbUnitOfWork
        where TContext : KbIdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        private IIdentityDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> _dbFactory;
        private TContext _context;

        public KbIdentityUnitOfWork(IIdentityDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public TContext Context => _context ??= _dbFactory.Get();

        public string GetConnectionString()
        {
            return Context.GetConnectionString();
        }

        public ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
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

        public async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return await Context.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool EnsureDeleted()
        {
            return Context.EnsureDeleted();
        }

        public async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await Context.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Migrate()
        {
            Context.Migrate();
        }

        public async ValueTask MigrateAsync(CancellationToken cancellationToken = default)
        {
            await Context.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        public void BeginTransaction()
        {
            Context.BeginTransaction();
        }

        public async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await Context.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public int Commit()
        {
            return Context.Commit();
        }

        public async ValueTask<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await Context.CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        public void CommitTransaction()
        {
            Context.CommitTransaction();
        }

        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await Context.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Rollback()
        {
            Context.Rollback();
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
