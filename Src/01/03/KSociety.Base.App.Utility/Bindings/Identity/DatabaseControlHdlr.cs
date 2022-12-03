using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.App.Utility.Dto.Res.Control;
using KSociety.Base.App.Utility.ReqHdlr;

namespace KSociety.Base.App.Utility.Bindings.Identity
{
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