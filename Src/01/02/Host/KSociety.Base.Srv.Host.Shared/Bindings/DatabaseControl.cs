using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.App.Utility.Dto.Res.Control;
using KSociety.Base.App.Utility.ReqHdlr;
using KSociety.Base.Infra.Abstraction.Interface;
using KSociety.Base.Infra.Shared.Class;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class DatabaseControl<TContext> : Module where TContext : DatabaseContext
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GetConnectionStringReqHdlr>()
                .As(typeof(IRequestHandlerWithResponse<ConnectionString>))
                .As(typeof(IRequestHandlerWithResponseAsync<ConnectionString>));

            builder.RegisterType<EnsureCreatedReqHdlr>()
                .As(typeof(IRequestHandlerWithResponse<EnsureCreated>))
                .As(typeof(IRequestHandlerWithResponseAsync<EnsureCreated>));

            builder.RegisterType<EnsureDeletedReqHdlr>()
                .As(typeof(IRequestHandlerWithResponse<EnsureDeleted>))
                .As(typeof(IRequestHandlerWithResponseAsync<EnsureDeleted>));

            builder.RegisterType<MigrationReqHdlr>()
                .Named<IRequestHandler>("MigrationReqHdlr")
                .Named<IRequestHandlerAsync>("MigrationReqHdlr");

            builder.RegisterType<DatabaseFactory<ILoggerFactory, IDatabaseConfiguration, TContext>>()
                .As<IDatabaseFactory<TContext>>().InstancePerLifetimeScope();
        }
    }
}