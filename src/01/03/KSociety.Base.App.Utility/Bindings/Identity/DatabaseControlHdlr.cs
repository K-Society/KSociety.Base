namespace KSociety.Base.App.Utility.Bindings.Identity
{
    using Autofac;
    using Shared;
    using Dto.Res.Control;
    using ReqHdlr;

    public class DatabaseControlHdlr : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GetConnectionStringReqHdlr>()
                .As(typeof(IRequestHandlerWithResponse<ConnectionString>));
            builder.RegisterType<EnsureCreatedReqHdlr>().As(typeof(IRequestHandlerWithResponse<EnsureCreated>)).As(
                typeof(IRequestHandlerWithResponseAsync<EnsureCreated>));
            builder.RegisterType<EnsureDeletedReqHdlr>().As(typeof(IRequestHandlerWithResponse<EnsureDeleted>)).As(
                typeof(IRequestHandlerWithResponseAsync<EnsureDeleted>));
            builder.RegisterType<MigrationReqHdlr>().Named<IRequestHandler>("MigrationReqHdlr")
                .Named<IRequestHandlerAsync>("MigrationReqHdlr");

            //builder
            //    .RegisterType<DatabaseFactory<ILoggerFactory, IDatabaseConfiguration, TContext, TUser, TRole, TKey,
            //        TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>()
            //    .As<IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            //        TUserToken>>().InstancePerLifetimeScope();
        }
    }
}
