using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Contract;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace KSociety.Base.Srv.Behavior
{
    public class Import<TImportReq, TImportRes> : IImport<TImportReq, TImportRes>
        where TImportReq : class, IRequest, new()
        where TImportRes : class, IResponse, new()
    {
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly IComponentContext ComponentContext;
        protected readonly ICommandHandler CommandHandler;

        public Import(
            ILoggerFactory loggerFactory,
            IComponentContext componentContext,
            ICommandHandler commandHandler
        )
        {
            LoggerFactory = loggerFactory;
            ComponentContext = componentContext;
            CommandHandler = commandHandler;
        }

        public virtual TImportRes ImportData(TImportReq importReq, CallContext context = default)
        {
            return CommandHandler.ExecuteWithResponse<TImportReq, TImportRes>(LoggerFactory, ComponentContext,
                importReq);
        }
    }
}
