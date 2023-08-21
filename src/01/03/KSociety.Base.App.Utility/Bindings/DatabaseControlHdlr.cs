namespace KSociety.Base.App.Utility.Bindings
{
    using Autofac;
    using Shared;
    using Dto.Res.Control;
    using ReqHdlr;

    /// <summary>
    /// 
    /// </summary>
    public class DatabaseControlHdlr : Module
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
        }
    }
}
