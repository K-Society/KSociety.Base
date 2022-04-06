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
    private readonly ILoggerFactory _loggerFactory;
    private readonly IComponentContext _componentContext;
    private readonly ICommandHandlerAsync _commandHandlerAsync;

    public CommandAsync(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandlerAsync commandHandlerAsync
    )
    {
        _loggerFactory = loggerFactory;
        _componentContext = componentContext;
        _commandHandlerAsync = commandHandlerAsync;
    }

    public virtual async ValueTask<TAddRes> AddAsync(TAddReq request, CallContext context = default)
    {
        return await _commandHandlerAsync.ExecuteWithResponseAsync<TAddReq, TAddRes>(_loggerFactory, _componentContext, request, context.CancellationToken);
    }

    public virtual async ValueTask<TUpdateRes> UpdateAsync(TUpdateReq request, CallContext context = default)
    {
        return await _commandHandlerAsync.ExecuteWithResponseAsync<TUpdateReq, TUpdateRes>(_loggerFactory, _componentContext, request, context.CancellationToken);
    }

    public virtual async ValueTask<TCopyRes> CopyAsync(TCopyReq request, CallContext context = default)
    {
        return await _commandHandlerAsync.ExecuteWithResponseAsync<TCopyReq, TCopyRes>(_loggerFactory, _componentContext, request, context.CancellationToken);
    }

    //public async ValueTask<App.Dto.Res.Export.Common.Tag> ExportTagAsync(App.Dto.Req.Export.Common.Tag request, CallContext context = default)
    //{
    //    return await _commandHandlerAsync.ExecuteWithResponseAsync<App.Dto.Req.Export.Common.Tag, App.Dto.Res.Export.Common.Tag>(_loggerFactory, _componentContext, request, context.CancellationToken);
    //}

    //public async ValueTask<App.Dto.Res.Import.Common.Tag> ImportTagAsync(App.Dto.Req.Import.Common.Tag request, CallContext context = default)
    //{
    //    return await _commandHandlerAsync.ExecuteWithResponseAsync<App.Dto.Req.Import.Common.Tag, App.Dto.Res.Import.Common.Tag>(_loggerFactory, _componentContext, request, context.CancellationToken);
    //}

    public virtual async ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq request, CallContext context = default)
    {
        return await _commandHandlerAsync.ExecuteWithResponseAsync<TModifyFieldReq, TModifyFieldRes>(_loggerFactory, _componentContext, request, context.CancellationToken);
    }

    public virtual async ValueTask<TRemoveRes> RemoveAsync(TRemoveReq request, CallContext context = default)
    {
        return await _commandHandlerAsync.ExecuteWithResponseAsync<TRemoveReq, TRemoveRes>(_loggerFactory, _componentContext, request, context.CancellationToken);
    }
}
