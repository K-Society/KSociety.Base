﻿using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.App.Shared.Dto.Res.Control;
using KSociety.Base.App.Shared.ReqHdlr;
using KSociety.Base.Infra.Shared.Class.Identity;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.Infra.Shared.Interface.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;

namespace KSociety.Base.Srv.Host.Shared.Bindings.Identity;

public class DatabaseControl<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : Module 
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
        builder.RegisterType<GetConnectionStringReqHdlr>().As(typeof(IRequestHandlerWithResponse<ConnectionString>));
        builder.RegisterType<EnsureCreatedReqHdlr>().As(typeof(IRequestHandlerWithResponse<EnsureCreated>)).As(
            typeof(IRequestHandlerWithResponseAsync<EnsureCreated>));
        builder.RegisterType<EnsureDeletedReqHdlr>().As(typeof(IRequestHandlerWithResponse<EnsureDeleted>)).As(
            typeof(IRequestHandlerWithResponseAsync<EnsureDeleted>));
        builder.RegisterType<MigrationReqHdlr>().Named<IRequestHandler>("MigrationReqHdlr")
            .Named<IRequestHandlerAsync>("MigrationReqHdlr");

        builder.RegisterType<DatabaseFactory<ILoggerFactory, IDatabaseConfiguration, TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>()
            .As<IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>().InstancePerLifetimeScope();
    }
}