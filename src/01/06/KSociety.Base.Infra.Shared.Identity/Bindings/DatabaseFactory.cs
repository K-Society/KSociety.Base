// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Identity.Bindings
{
    using System;
    using Autofac;
    using KSociety.Base.Infra.Abstraction.Interface;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class DatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
        TUserToken> : Module
        where TContext : Class.DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
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
                .RegisterType<Class.DatabaseFactory<ILoggerFactory, IDatabaseConfiguration, TContext, TUser, TRole, TKey,
                    TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>()
                .As<Interface.IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
                    TUserToken>>().InstancePerLifetimeScope();
        }
    }
}
