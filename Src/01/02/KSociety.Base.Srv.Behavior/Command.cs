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
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly IComponentContext ComponentContext;
    protected readonly ICommandHandler CommandHandler;

    public Command(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandler commandHandler
    )
    {
        LoggerFactory = loggerFactory;
        ComponentContext = componentContext;
        CommandHandler = commandHandler;
    }

    public virtual TAddRes Add(TAddReq request, CallContext context = default)
    {
        return CommandHandler.ExecuteWithResponse<TAddReq, TAddRes>(LoggerFactory, ComponentContext, request);
    }

    public virtual TUpdateRes Update(TUpdateReq request, CallContext context = default)
    {
        return CommandHandler.ExecuteWithResponse<TUpdateReq, TUpdateRes>(LoggerFactory, ComponentContext, request);
    }

    public virtual TCopyRes Copy(TCopyReq request, CallContext context = default)
    {
        return CommandHandler.ExecuteWithResponse<TCopyReq, TCopyRes>(LoggerFactory, ComponentContext, request);
    }

    public virtual TModifyFieldRes ModifyField(TModifyFieldReq request, CallContext context = default)
    {
        return CommandHandler.ExecuteWithResponse<TModifyFieldReq, TModifyFieldRes>(LoggerFactory, ComponentContext, request);
    }

    public virtual TRemoveRes Remove(TRemoveReq request, CallContext context = default)
    {
        return CommandHandler.ExecuteWithResponse<TRemoveReq, TRemoveRes>(LoggerFactory, ComponentContext, request);
    }
}
