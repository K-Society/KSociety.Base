// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Abstractions;
    using Abstractions.Handler;
    using Microsoft.Extensions.Logging;

    ///<inheritdoc cref="IIntegrationQueueHandler{TIntegrationEventReply}"/>
    public class IntegrationRpcClientHandler<TIntegrationEventReply>
        : IntegrationGeneralHandler, IIntegrationRpcClientHandler<TIntegrationEventReply>
        where TIntegrationEventReply : IIntegrationEventReply
    {
        protected readonly ILogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>? Logger;

        #region [Constructors]

        public IntegrationRpcClientHandler(ILoggerFactory? loggerFactory = default)
            : base(loggerFactory)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>();
        }

        public IntegrationRpcClientHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default)
            : base(loggerFactory, componentContext)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>();
        }

        public IntegrationRpcClientHandler(ILogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>? logger = default, IComponentContext? componentContext = default)
            : base(componentContext)
        {
            this.Logger = logger;
        }

        #endregion

        public virtual void HandleReply(TIntegrationEventReply @integrationEventReply,
            CancellationToken cancel = default)
        {
            this.Logger?.LogWarning("IntegrationRpcHandler HandleRpcAsync: {0}, routing key: {1}", "NotImplemented!",
                @integrationEventReply.RoutingKey);
        }

        public virtual async ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply,
            CancellationToken cancel = default)
        {
            this.Logger?.LogWarning("IntegrationRpcHandler HandleRpcAsync: {0}, routing key: {1}", "NotImplemented!",
                @integrationEventReply.RoutingKey);
        }
    }
}
