namespace KSociety.Base.Srv.Behavior
{
    using System.Threading.Tasks;
    using Autofac;
    using KSociety.Base.App.Shared;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;
    using Shared.Interface;

    public class CommandAsync<
        TAddReq, TAddRes,
        TUpdateReq, TUpdateRes,
        TCopyReq, TCopyRes,
        TModifyFieldReq, TModifyFieldRes,
        TRemoveReq, TRemoveRes> : Contract.ICommandAsync<
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
            this.LoggerFactory = loggerFactory;
            this.ComponentContext = componentContext;
            this.CommandHandlerAsync = commandHandlerAsync;
        }

        public virtual async ValueTask<TAddRes> AddAsync(TAddReq request, CallContext context = default)
        {
            return await this.CommandHandlerAsync
                .ExecuteWithResponseAsync<TAddReq, TAddRes>(this.LoggerFactory, this.ComponentContext, request,
                    context.CancellationToken).ConfigureAwait(false);
        }

        public virtual async ValueTask<TUpdateRes> UpdateAsync(TUpdateReq request, CallContext context = default)
        {
            return await this.CommandHandlerAsync
                .ExecuteWithResponseAsync<TUpdateReq, TUpdateRes>(this.LoggerFactory, this.ComponentContext, request,
                    context.CancellationToken).ConfigureAwait(false);
        }

        public virtual async ValueTask<TCopyRes> CopyAsync(TCopyReq request, CallContext context = default)
        {
            return await this.CommandHandlerAsync
                .ExecuteWithResponseAsync<TCopyReq, TCopyRes>(this.LoggerFactory, this.ComponentContext, request,
                    context.CancellationToken).ConfigureAwait(false);
        }

        public virtual async ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq request,
            CallContext context = default)
        {
            return await this.CommandHandlerAsync
                .ExecuteWithResponseAsync<TModifyFieldReq, TModifyFieldRes>(this.LoggerFactory, this.ComponentContext, request,
                    context.CancellationToken).ConfigureAwait(false);
        }

        public virtual async ValueTask<TRemoveRes> RemoveAsync(TRemoveReq request, CallContext context = default)
        {
            return await this.CommandHandlerAsync
                .ExecuteWithResponseAsync<TRemoveReq, TRemoveRes>(this.LoggerFactory, this.ComponentContext, request,
                    context.CancellationToken).ConfigureAwait(false);
        }
    }
}
