// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Identity.Class
{
    using System;
    using KSociety.Base.Infra.Abstraction.Interface;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Shared.Class;

    public class DatabaseFactory<TLoggerFactory, TConfiguration, TContext, TUser, TRole, TKey, TUserClaim, TUserRole,
            TUserLogin, TRoleClaim, TUserToken>
        : DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>, Interface.IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TLoggerFactory : ILoggerFactory
        where TConfiguration : IDatabaseConfiguration
        where TContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        private IUserStore<TUser> _userStore;
        private IRoleStore<TRole> _roleStore;

        public DatabaseFactory(TLoggerFactory loggerFactory, TConfiguration configuration)
            : base(loggerFactory, configuration)
        {

        }

        public IUserStore<TUser> GetUserStore()
        {
            if (this._userStore != null)
            {
                return this._userStore;
            }

            this._userStore = this.CreateUserStore();

            return this._userStore;
        }

        public IRoleStore<TRole> GetRoleStore()
        {
            if (this._roleStore != null)
            {
                return this._roleStore;
            }

            this._roleStore = this.CreateRoleStore();

            return this._roleStore;
        }

        private IUserStore<TUser> CreateUserStore()
        {
            IUserStore<TUser> output = null;
            try
            {
                output =
                    (UserStore<TUser, TRole, TContext, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>)
                    Activator.CreateInstance(
                        typeof(UserStore<TUser, TRole, TContext, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken,
                            TRoleClaim>), this.Get(), null);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateUserStore: ");
            }

            return output;
        }

        private IRoleStore<TRole> CreateRoleStore()
        {
            IRoleStore<TRole> output = null;
            try
            {
                output = (RoleStore<TRole, TContext, TKey, TUserRole, TRoleClaim>)Activator.CreateInstance(
                    typeof(RoleStore<TRole, TContext, TKey, TUserRole, TRoleClaim>), this.Get(), null);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateRoleStore: ");
            }

            return output;
        }
    }
}
