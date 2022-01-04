using System;
using Autofac;
using KSociety.Base.Infra.Shared.Class.Identity;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.AspNetCore.Identity;

namespace KSociety.Base.Srv.Host.Shared.Bindings.Identity;

public class UnitOfWork<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : Module
    where TContext : DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TUserToken : IdentityUserToken<TKey>
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Infra.Shared.Class.Identity.UnitOfWork<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>().As<IDatabaseUnitOfWork>();
    }
}