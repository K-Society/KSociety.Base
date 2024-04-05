// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ.Helper
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using EventBus.Abstractions;
    using EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;

    public interface ISubscriber
    {
        IRabbitMqPersistentConnection PersistentConnection { get; }
        ConcurrentDictionary<string, IEventBus> EventBus { get; }

        ValueTask SubscribeClientServer<
            TIntegrationRpcClientHandler, TIntegrationRpcServerHandler,
            TIntegrationEventRpc, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string routingKey, string replyRoutingKey,
            TIntegrationRpcClientHandler integrationRpcClientHandler,
            TIntegrationRpcServerHandler integrationRpcServerHandler, bool asyncMode = true)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationRpcServerHandler :
            IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new();

        ValueTask SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string replyRoutingKey, TIntegrationRpcClientHandler integrationRpcClientHandler, bool asyncMode = true)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationEventReply : IIntegrationEventReply, new();

        ValueTask SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEventRpc, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationRpcServerHandler integrationRpcServerHandler, bool asyncMode = true)
            where TIntegrationRpcServerHandler :
            IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new();

        ValueTask SubscribeTyped<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationEventHandler integrationEventHandler, bool asyncMode = true)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void SubscribeTyped<TIntegrationEvent>(string eventBusName, string queueName = null)
            where TIntegrationEvent : IIntegrationEvent, new();

        void SubscribeInvoke<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            TIntegrationEventHandler integrationEventHandler)
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent, new();
    }
}
