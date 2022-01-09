using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;

namespace KSociety.Base.EventBusRabbitMQ.Helper;

public class Subscriber
{
    public void SubscribeClientServer<
        TIntegrationRpcClientHandler, TIntegrationRpcServerHandler,
        TIntegrationEvent, TIntegrationEventReply>(
        string eventBusName, string queueName,
        string routingKey, string replyRoutingKey,
        TIntegrationRpcClientHandler integrationRpcClientHandler, TIntegrationRpcServerHandler integrationRpcServerHandler,
        IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory, IEventBusParameters eventBusParameters, ref Dictionary<string, IEventBus> eventBusDictionary, CancellationToken cancellationToken = default
    )
        where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
            eventBusName, queueName,
            replyRoutingKey, integrationRpcClientHandler,
            persistentConnection, loggerFactory, eventBusParameters, ref eventBusDictionary,
            cancellationToken);

        SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
            eventBusName, queueName,
            routingKey, integrationRpcServerHandler,
            persistentConnection, loggerFactory, eventBusParameters, ref eventBusDictionary, cancellationToken);
    }
    public static void SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
        string eventBusName, string queueName,
        string replyRoutingKey, TIntegrationRpcClientHandler integrationRpcClientHandler, 
        IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory, IEventBusParameters eventBusParameters, ref Dictionary<string, IEventBus> eventBusDictionary, CancellationToken cancellationToken = default)
        where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        where TIntegrationEventReply : IIntegrationEventReply
    {
        eventBusDictionary.Add(eventBusName,
            new EventBusRabbitMqRpcClient(persistentConnection, loggerFactory, integrationRpcClientHandler,
                null, eventBusParameters, queueName, cancellationToken));

        ((IEventBusRpcClient)eventBusDictionary[eventBusName])
            .SubscribeRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(replyRoutingKey);
    }

    public static void SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
        string eventBusName, string queueName,
        string routingKey, TIntegrationRpcServerHandler integrationRpcServerHandler,
        IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory, IEventBusParameters eventBusParameters, ref Dictionary<string, IEventBus> eventBusDictionary, CancellationToken cancellationToken = default)
        where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        eventBusDictionary.Add(eventBusName,
            new EventBusRabbitMqRpcServer(persistentConnection, loggerFactory, integrationRpcServerHandler,
                null, eventBusParameters, queueName, CancellationToken.None));

        ((IEventBusRpcServer)eventBusDictionary[eventBusName])
            .SubscribeRpcServer<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcServerHandler>(routingKey);
    }
}