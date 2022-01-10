using System;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.Infra.Shared.Interface.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Class.Identity;

public class DatabaseFactory<TLoggerFactory, TConfiguration, TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    : DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>, IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    where TLoggerFactory : ILoggerFactory
    where TConfiguration : IDatabaseConfiguration
    where TContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
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

    public DatabaseFactory(TLoggerFactory loggerFactory, TConfiguration configuration)
        :base(loggerFactory, configuration)
    {

    }

    public IUserStore<TUser> GetUserStore()
    {
        if (_userStore is not null) return _userStore;
        _userStore = CreateUserStore();

        return _userStore;
    }

    private IUserStore<TUser> CreateUserStore()
    {
        IUserStore<TUser> output = null;
        try
        {
            output = (UserStore<TUser, TRole, TContext, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>)Activator.CreateInstance(typeof(UserStore<TUser, TRole, TContext, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>), Get(), null);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateUserStore: ");
        }

        return output;
    }
}