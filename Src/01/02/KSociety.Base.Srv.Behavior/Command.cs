using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace KSociety.Base.Srv.Behavior;
public class Command<
    TAddReq, TAddRes,
    TUpdateReq, TUpdateRes,
    TCopyReq, TCopyRes,
    TModifyFieldReq, TModifyFieldRes,
    TRemoveReq, TRemoveRes> : KSociety.Base.Srv.Contract.ICommand<
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
    private readonly ICommandHandler _commandHandler;

    public Command(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandler commandHandler
    )
    {
        _loggerFactory = loggerFactory;
        _componentContext = componentContext;
        _commandHandler = commandHandler;
    }

    public virtual TAddRes Add(TAddReq request, CallContext context = default)
    {
        return _commandHandler.ExecuteWithResponse<TAddReq, TAddRes>(_loggerFactory, _componentContext, request);
    }

    public virtual TUpdateRes Update(TUpdateReq request, CallContext context = default)
    {
        return _commandHandler.ExecuteWithResponse<TUpdateReq, TUpdateRes>(_loggerFactory, _componentContext, request);
    }

    public virtual TCopyRes Copy(TCopyReq request, CallContext context = default)
    {
        return _commandHandler.ExecuteWithResponse<TCopyReq, TCopyRes>(_loggerFactory, _componentContext, request);
    }

    //public App.Dto.Res.Export.Common.Tag Export(App.Dto.Req.Export.Common.Tag request, CallContext context = default)
    //{
    //    return _commandHandler.ExecuteWithResponse<App.Dto.Req.Export.Common.Tag, App.Dto.Res.Export.Common.Tag>(_loggerFactory, _componentContext, request);
    //}

    //public App.Dto.Res.Import.Common.Tag Import(App.Dto.Req.Import.Common.Tag request, CallContext context = default)
    //{
    //    return _commandHandler.ExecuteWithResponse<App.Dto.Req.Import.Common.Tag, App.Dto.Res.Import.Common.Tag>(_loggerFactory, _componentContext, request);
    //}

    public virtual TModifyFieldRes ModifyField(TModifyFieldReq request, CallContext context = default)
    {
        return _commandHandler.ExecuteWithResponse<TModifyFieldReq, TModifyFieldRes>(_loggerFactory, _componentContext, request);
    }

    public virtual TRemoveRes Remove(TRemoveReq request, CallContext context = default)
    {
        return _commandHandler.ExecuteWithResponse<TRemoveReq, TRemoveRes>(_loggerFactory, _componentContext, request);
    }
}
