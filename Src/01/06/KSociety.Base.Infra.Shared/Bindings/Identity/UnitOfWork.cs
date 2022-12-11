using Autofac;
using KSociety.Base.Infra.Abstraction.Interface;
using KSociety.Base.Infra.Shared.Class.Identity;
using Microsoft.AspNetCore.Identity;
using System;

namespace KSociety.Base.Infra.Shared.Bindings.Identity
{
    public class UnitOfWork<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
        TUserToken> : Module
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
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<
                    Infra.Shared.Class.Identity.UnitOfWork<TContext, TUser, TRole, TKey, TUserClaim, TUserRole,
                        TUserLogin, TRoleClaim, TUserToken>>().As<IDatabaseUnitOfWork>();
        }
    }
}