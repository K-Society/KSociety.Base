// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Identity.Class
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using KSociety.Base.Infra.Abstraction.Interface;
    using KSociety.Base.InfraSub.Shared.Class;
    using Microsoft.AspNetCore.Identity;

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
        private Interface.IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken>? _dbFactory;

        private TContext? _context;
        private IUserStore<TUser>? _userStore;
        private IRoleStore<TRole>? _roleStore;

        public UnitOfWork(Interface.IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>?
                dbFactory)
        {
            this._dbFactory = dbFactory;
        }

        public TContext? Context => this._context ??= this._dbFactory?.Get();
        public IUserStore<TUser>? UserStore => this._userStore ??= this._dbFactory?.GetUserStore();
        public IRoleStore<TRole>? RoleStore => this._roleStore ??= this._dbFactory?.GetRoleStore();

        public string? GetConnectionString()
        {
            return this.Context?.GetConnectionString();
        }

        public async ValueTask<string?> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                return await this.Context.GetConnectionStringAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            return null;
        }

        public bool? Exists()
        {
            return this.Context?.Exists();
        }

        public async ValueTask<bool?> ExistsAsync(CancellationToken cancellationToken = default)
        {
            if(this.Context != null)
            {
                return await this.Context.ExistsAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        public bool? EnsureCreated()
        {
            return this.Context?.EnsureCreated();
        }

        public async ValueTask<bool?> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                await this.Context.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        public bool? EnsureDeleted()
        {
            return this.Context?.EnsureDeleted();
        }

        public async ValueTask<bool?> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                return await this.Context.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        public void Migrate(string? targetMigration = null)
        {
            this.Context?.Migrate(targetMigration);
        }

        public async ValueTask MigrateAsync(string? targetMigration = null,
            CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                await this.Context.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
            }
        }

        public string? CreateScript()
        {
            return this.Context?.CreateScript();
        }

        public void BeginTransaction()
        {
            this.Context?.BeginTransaction();
        }

        public async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                await this.Context.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public int? Commit()
        {
            return this.Context?.Commit();
        }

        public async ValueTask<int?> CommitAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                return await this.Context.CommitAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        public void CommitTransaction()
        {
            this.Context?.CommitTransaction();
        }

        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                await this.Context.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public void Rollback()
        {
            this.Context?.Rollback();
        }

        public async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null)
            {
                await this.Context.RollbackAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        protected override void DisposeManagedResources()
        {
            if (this._dbFactory != null)
            {
                this._dbFactory.Dispose();
                this._dbFactory = null;
            }

            if (this._context == null)
            {
                return;
            }

            this._context.Dispose();
            this._context = null;
        }
    }
}
