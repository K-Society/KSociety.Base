// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior
{
    using Autofac;
    using KSociety.Base.App.Shared;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;

    public class Command<
        TAddReq, TAddRes,
        TUpdateReq, TUpdateRes,
        TCopyReq, TCopyRes,
        TModifyFieldReq, TModifyFieldRes,
        TRemoveReq, TRemoveRes> : Contract.ICommand<
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
            this.LoggerFactory = loggerFactory;
            this.ComponentContext = componentContext;
            this.CommandHandler = commandHandler;
        }

        public virtual TAddRes Add(TAddReq request, CallContext context = default)
        {
            return this.CommandHandler.ExecuteWithResponse<TAddReq, TAddRes>(this.LoggerFactory, this.ComponentContext, request);
        }

        public virtual TUpdateRes Update(TUpdateReq request, CallContext context = default)
        {
            return this.CommandHandler.ExecuteWithResponse<TUpdateReq, TUpdateRes>(this.LoggerFactory, this.ComponentContext, request);
        }

        public virtual TCopyRes Copy(TCopyReq request, CallContext context = default)
        {
            return this.CommandHandler.ExecuteWithResponse<TCopyReq, TCopyRes>(this.LoggerFactory, this.ComponentContext, request);
        }

        public virtual TModifyFieldRes ModifyField(TModifyFieldReq request, CallContext context = default)
        {
            return this.CommandHandler.ExecuteWithResponse<TModifyFieldReq, TModifyFieldRes>(this.LoggerFactory, this.ComponentContext,
                request);
        }

        public virtual TRemoveRes Remove(TRemoveReq request, CallContext context = default)
        {
            return this.CommandHandler.ExecuteWithResponse<TRemoveReq, TRemoveRes>(this.LoggerFactory, this.ComponentContext, request);
        }
    }
}
