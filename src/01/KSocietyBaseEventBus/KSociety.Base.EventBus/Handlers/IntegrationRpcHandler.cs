// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Autofac;
    using Abstractions;
    using Abstractions.Handler;
    using Microsoft.Extensions.Logging;

    ///<inheritdoc cref="IIntegrationRpcHandler{TIntegrationEvent, TIntegrationEventReply}"/>
    public class IntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        : IntegrationGeneralHandler, IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEvent
        where TIntegrationEventReply : IIntegrationEventReply
    {
        protected readonly ILogger<IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>>? Logger;

        public BufferBlock<TIntegrationEventReply> Queue { get; }
        public bool IsEmpty => this.Queue.Count == 0;

        #region [Constructors]

        public IntegrationRpcHandler(ILoggerFactory? loggerFactory = default)
            : base(loggerFactory)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>>();
            this.Queue = new BufferBlock<TIntegrationEventReply>();
        }

        public IntegrationRpcHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default)
            : base(loggerFactory, componentContext)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>>();
            this.Queue = new BufferBlock<TIntegrationEventReply>();
        }

        public IntegrationRpcHandler(ILogger<IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>>? logger = default, IComponentContext? componentContext = default)
            : base(componentContext)
        {
            this.Logger = logger;
            this.Queue = new BufferBlock<TIntegrationEventReply>();
        }

        #endregion

        public virtual TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel = default)
        {
            this.Logger?.LogWarning("IntegrationRpcHandler HandleRpc: NotImplemented!");
            throw new NotImplementedException();
        }

        public virtual ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event,
            CancellationToken cancel = default)
        {
            this.Logger?.LogWarning("IntegrationRpcHandler HandleRpcAsync: NotImplemented!");
            throw new NotImplementedException();
        }

        public virtual async ValueTask HandleReply(TIntegrationEventReply @integrationEventReply,
            CancellationToken cancel = default)
        {
            await this.Queue.SendAsync(@integrationEventReply, cancel).ConfigureAwait(false);
        }

        public virtual async ValueTask<bool> Enqueue(TIntegrationEventReply @integrationEventReply,
            CancellationToken cancel = default)
        {
            return await this.Queue.SendAsync(@integrationEventReply, cancel).ConfigureAwait(false);
        }

        public virtual async ValueTask<TIntegrationEventReply> Take(CancellationToken cancel = default)
        {
            return await this.Queue.ReceiveAsync(cancel).ConfigureAwait(false);
        }
    }
}
