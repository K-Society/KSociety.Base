using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.App.Shared.Dto.Res.Control;
using KSociety.Base.App.Shared.ReqHdlr;
using KSociety.Base.Infra.Shared.Class;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    public class DatabaseControl<TContext> : Module where TContext : DatabaseContext
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

            builder.RegisterType<DatabaseFactory<ILoggerFactory, Infra.Shared.Class.DatabaseConfiguration, TContext>>()
                .As<IDatabaseFactory<TContext>>().InstancePerLifetimeScope();
        }
    }
}
