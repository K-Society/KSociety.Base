// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior
{
    using Autofac;
    using KSociety.Base.App.Shared;
    using Contract;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;
    using System.Threading.Tasks;

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
            this.LoggerFactory = loggerFactory;
            this.ComponentContext = componentContext;
            this.CommandHandlerAsync = commandHandlerAsync;
        }

        public virtual async ValueTask<TExportRes?> ExportDataAsync(TExportReq exportReq, CallContext context = default)
        {
            return await this.CommandHandlerAsync
                .ExecuteWithResponseAsync<TExportReq, TExportRes>(this.LoggerFactory, this.ComponentContext, exportReq,
                    context.CancellationToken)
                .ConfigureAwait(false);
        }
    }
}
