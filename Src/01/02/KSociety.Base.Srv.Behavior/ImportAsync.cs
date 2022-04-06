using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Contract;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Behavior;
public class ImportAsync<TImportReq, TImportRes> : IImportAsync<TImportReq, TImportRes>
    where TImportReq : class, IRequest, new()
    where TImportRes : class, IResponse, new()
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly IComponentContext ComponentContext;
    protected readonly ICommandHandlerAsync CommandHandlerAsync;

    public ImportAsync(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandlerAsync commandHandlerAsync
    )
    {
        LoggerFactory = loggerFactory;
        ComponentContext = componentContext;
        CommandHandlerAsync = commandHandlerAsync;
    }

    public virtual async ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CallContext context = default)
    {
        return await CommandHandlerAsync
            .ExecuteWithResponseAsync<TImportReq, TImportRes>(LoggerFactory, ComponentContext, importReq, context.CancellationToken)
            .ConfigureAwait(false);
    }
}
