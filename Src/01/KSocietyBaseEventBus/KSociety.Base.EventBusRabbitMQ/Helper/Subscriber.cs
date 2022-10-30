using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Threading;

namespace KSociety.Base.EventBusRabbitMQ.Helper;

public class Subscriber
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IEventBusParameters _eventBusParameters;
    public IRabbitMqPersistentConnection PersistentConnection { get; }
    public Dictionary<string, IEventBus> EventBus { get; } = new();

    public Subscriber(
        ILoggerFactory loggerFactory,
        IConnectionFactory connectionFactory,
        IEventBusParameters eventBusParameters)
    {
        _loggerFactory = loggerFactory;
        _eventBusParameters = eventBusParameters;

        PersistentConnection = new DefaultRabbitMqPersistentConnection(connectionFactory, _loggerFactory);
    }
    public void SubscribeClientServer<
        TIntegrationRpcClientHandler, TIntegrationRpcServerHandler,
        TIntegrationEvent, TIntegrationEventReply>(
        string eventBusName, string queueName,
        string routingKey, string replyRoutingKey,
        TIntegrationRpcClientHandler integrationRpcClientHandler, TIntegrationRpcServerHandler integrationRpcServerHandler
    )
        where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
            eventBusName, queueName,
            replyRoutingKey, integrationRpcClientHandler);

        SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
            eventBusName, queueName,
            routingKey, integrationRpcServerHandler);
    }

    public void SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
        string eventBusName, string queueName,
        string replyRoutingKey, TIntegrationRpcClientHandler integrationRpcClientHandler)
        where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        where TIntegrationEventReply : IIntegrationEventReply
    {
        EventBus.Add(eventBusName + "_Client",
            new EventBusRabbitMqRpcClient(PersistentConnection, _loggerFactory, integrationRpcClientHandler,
                null, _eventBusParameters, queueName));

        ((IEventBusRpcClient)EventBus[eventBusName + "_Client"])
            .SubscribeRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(replyRoutingKey);
    }

    public void SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
        string eventBusName, string queueName,
        string routingKey, TIntegrationRpcServerHandler integrationRpcServerHandler)
        where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        EventBus.Add(eventBusName + "_Server",
            new EventBusRabbitMqRpcServer(PersistentConnection, _loggerFactory, integrationRpcServerHandler,
                null, _eventBusParameters, queueName));

        ((IEventBusRpcServer)EventBus[eventBusName + "_Server"])
            .SubscribeRpcServer<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcServerHandler>(routingKey);
    }
}