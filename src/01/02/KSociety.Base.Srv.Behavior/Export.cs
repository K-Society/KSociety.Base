// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior
{
    using Contract;
    using Autofac;
    using KSociety.Base.App.Shared;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;

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
            this.LoggerFactory = loggerFactory;
            this.ComponentContext = componentContext;
            this.CommandHandler = commandHandler;
        }

        public virtual TExportRes? ExportData(TExportReq exportReq, CallContext context = default)
        {
            return this.CommandHandler.ExecuteWithResponse<TExportReq, TExportRes>(this.LoggerFactory, this.ComponentContext,
                exportReq);
        }
    }
}
