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
            this.LoggerFactory = loggerFactory;
            this.ComponentContext = componentContext;
            this.CommandHandlerAsync = commandHandlerAsync;
        }

        public virtual async ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CallContext context = default)
        {
            return await this.CommandHandlerAsync
                .ExecuteWithResponseAsync<TImportReq, TImportRes>(this.LoggerFactory, this.ComponentContext, importReq,
                    context.CancellationToken)
                .ConfigureAwait(false);
        }
    }
}
