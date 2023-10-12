// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior
{
    using Autofac;
    using KSociety.Base.App.Shared;
    using Contract;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;

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
            this.LoggerFactory = loggerFactory;
            this.ComponentContext = componentContext;
            this.CommandHandler = commandHandler;
        }

        public virtual TImportRes? ImportData(TImportReq importReq, CallContext context = default)
        {
            return this.CommandHandler.ExecuteWithResponse<TImportReq, TImportRes>(this.LoggerFactory, this.ComponentContext,
                importReq);
        }
    }
}
