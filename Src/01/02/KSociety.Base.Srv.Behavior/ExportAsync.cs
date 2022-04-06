using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Contract;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Behavior;
public class ExportAsync<TExportReq, TExportRes> : IExportAsync<TExportReq, TExportRes>
    where TExportReq : class, IRequest, new()
    where TExportRes : class, IResponse, new()
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly IComponentContext ComponentContext;
    protected readonly ICommandHandlerAsync CommandHandlerAsync;

    public ExportAsync(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandlerAsync commandHandlerAsync
    )
    {
        LoggerFactory = loggerFactory;
        ComponentContext = componentContext;
        CommandHandlerAsync = commandHandlerAsync;
    }

    public virtual async ValueTask<TExportRes> ExportDataAsync(TExportReq importReq, CallContext context = default)
    {
        return await CommandHandlerAsync
            .ExecuteWithResponseAsync<TExportReq, TExportRes>(LoggerFactory, ComponentContext, importReq, context.CancellationToken)
            .ConfigureAwait(false);
    }
}
