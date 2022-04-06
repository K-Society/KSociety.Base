using KSociety.Base.Srv.Contract;
using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace KSociety.Base.Srv.Behavior;
public class Export<TExportReq, TExportRes> : IExport<TExportReq, TExportRes>
    where TExportReq : class, IRequest, new()
    where TExportRes : class, IResponse, new()
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly IComponentContext ComponentContext;
    protected readonly ICommandHandler CommandHandler;

    public Export(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandler commandHandler
    )
    {
        LoggerFactory = loggerFactory;
        ComponentContext = componentContext;
        CommandHandler = commandHandler;
    }

    public virtual TExportRes ExportData(TExportReq importReq, CallContext context = default)
    {
        return CommandHandler.ExecuteWithResponse<TExportReq, TExportRes>(LoggerFactory, ComponentContext, importReq);
    }
}
