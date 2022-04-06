using System.Threading.Tasks;
using Autofac;
using KSociety.Base.App.Shared;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;
using KSociety.Base.Srv.Shared.Interface;

namespace KSociety.Base.Srv.Behavior;
public class CommandAsync<
    TAddReq, TAddRes,
    TUpdateReq, TUpdateRes,
    TCopyReq, TCopyRes,
    TModifyFieldReq, TModifyFieldRes,
    TRemoveReq, TRemoveRes> : KSociety.Base.Srv.Contract.ICommandAsync<
    TAddReq, TAddRes,
    TUpdateReq, TUpdateRes,
    TCopyReq, TCopyRes,
    TModifyFieldReq, TModifyFieldRes,
    TRemoveReq, TRemoveRes>
    where TAddReq : class, IRequest, new()
    where TAddRes : class, IResponse, new()
    where TUpdateReq : class, IRequest, new()
    where TUpdateRes : class, IResponse, new()
    where TCopyReq : class, IRequest, new()
    where TCopyRes : class, IResponse, new()
    where TModifyFieldReq : class, IRequest, new()
    where TModifyFieldRes : class, IResponse, new()
    where TRemoveReq : class, IRequest, new()
    where TRemoveRes : class, IResponse, new()
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly IComponentContext ComponentContext;
    protected readonly ICommandHandlerAsync CommandHandlerAsync;

    public CommandAsync(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandlerAsync commandHandlerAsync
    )
    {
        LoggerFactory = loggerFactory;
        ComponentContext = componentContext;
        CommandHandlerAsync = commandHandlerAsync;
    }

    public virtual async ValueTask<TAddRes> AddAsync(TAddReq request, CallContext context = default)
    {
        return await CommandHandlerAsync
            .ExecuteWithResponseAsync<TAddReq, TAddRes>(LoggerFactory, ComponentContext, request,
                context.CancellationToken).ConfigureAwait(false);
    }

    public virtual async ValueTask<TUpdateRes> UpdateAsync(TUpdateReq request, CallContext context = default)
    {
        return await CommandHandlerAsync.ExecuteWithResponseAsync<TUpdateReq, TUpdateRes>(LoggerFactory, ComponentContext, request, context.CancellationToken).ConfigureAwait(false);
    }

    public virtual async ValueTask<TCopyRes> CopyAsync(TCopyReq request, CallContext context = default)
    {
        return await CommandHandlerAsync.ExecuteWithResponseAsync<TCopyReq, TCopyRes>(LoggerFactory, ComponentContext, request, context.CancellationToken).ConfigureAwait(false);
    }

    public virtual async ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq request, CallContext context = default)
    {
        return await CommandHandlerAsync.ExecuteWithResponseAsync<TModifyFieldReq, TModifyFieldRes>(LoggerFactory, ComponentContext, request, context.CancellationToken).ConfigureAwait(false);
    }

    public virtual async ValueTask<TRemoveRes> RemoveAsync(TRemoveReq request, CallContext context = default)
    {
        return await CommandHandlerAsync.ExecuteWithResponseAsync<TRemoveReq, TRemoveRes>(LoggerFactory, ComponentContext, request, context.CancellationToken).ConfigureAwait(false);
    }
}
