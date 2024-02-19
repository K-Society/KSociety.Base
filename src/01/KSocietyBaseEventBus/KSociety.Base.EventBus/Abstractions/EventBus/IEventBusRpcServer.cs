// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEventBusRpcServer : IEventBusBase
    {
        void InitializeServer<TIntegrationEvent, TIntegrationEventReply>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new();

        IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply> GetIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>()
            where TIntegrationEvent : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new();

        ValueTask SubscribeRpcServer<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>;

        void UnsubscribeRpcServer<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>;
    }
}
