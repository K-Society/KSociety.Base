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
    where TUserClaim : IdentityUserClaim<TKey>, new()
    where TUserRole : IdentityUserRole<TKey>, new()
    where TUserLogin : IdentityUserLogin<TKey>, new()
    where TRoleClaim : IdentityRoleClaim<TKey>, new()
    where TUserToken : IdentityUserToken<TKey>, new()
    where TContext : DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
{
}