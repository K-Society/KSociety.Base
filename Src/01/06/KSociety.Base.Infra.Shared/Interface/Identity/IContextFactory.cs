using KSociety.Base.Infra.Shared.Class.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace KSociety.Base.Infra.Shared.Interface.Identity;

public interface IContextFactory<out TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    : IDesignTimeDbContextFactory<TContext>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TUserToken : IdentityUserToken<TKey>
    where TContext : DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
{
}