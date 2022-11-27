using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KSociety.Base.EventBusRabbitMQ.Helper
{
    public class Subscriber
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IEventBusParameters _eventBusParameters;
        public IRabbitMqPersistentConnection PersistentConnection { get; }
        public Dictionary<string, IEventBus> EventBus { get; } = new();

        #region [Constructor]

        public Subscriber(
            ILoggerFactory loggerFactory,
            IConnectionFactory connectionFactory,
            IEventBusParameters eventBusParameters)
        {
            _loggerFactory = loggerFactory;
            _eventBusParameters = eventBusParameters;

            PersistentConnection = new DefaultRabbitMqPersistentConnection(connectionFactory, _loggerFactory);
        }

        public Subscriber(
            ILoggerFactory loggerFactory,
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusParameters eventBusParameters)
        {
            _loggerFactory = loggerFactory;
            _eventBusParameters = eventBusParameters;

            PersistentConnection = persistentConnection;
        }

        #endregion

        public async ValueTask SubscribeClientServer<
            TIntegrationRpcClientHandler, TIntegrationRpcServerHandler,
            TIntegrationEvent, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string routingKey, string replyRoutingKey,
            TIntegrationRpcClientHandler integrationRpcClientHandler,
            TIntegrationRpcServerHandler integrationRpcServerHandler
        )
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
            where TIntegrationEvent : IIntegrationEventRpc
            where TIntegrationEventReply : IIntegrationEventReply
        {
            await SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
                eventBusName, queueName,
                replyRoutingKey, integrationRpcClientHandler);

            await SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
                eventBusName, queueName,
                routingKey, integrationRpcServerHandler);
        }

        public async ValueTask SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string replyRoutingKey, TIntegrationRpcClientHandler integrationRpcClientHandler)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (EventBus.ContainsKey(eventBusName + "_Client")) return;

            EventBus.Add(eventBusName + "_Client",
                new EventBusRabbitMqRpcClient(PersistentConnection, _loggerFactory, integrationRpcClientHandler,
                    null, _eventBusParameters, queueName));

            ((IEventBusRpcClient)EventBus[eventBusName + "_Client"]).Initialize();

            await ((IEventBusRpcClient)EventBus[eventBusName + "_Client"])
                .SubscribeRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(replyRoutingKey);
        }

        public async ValueTask SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationRpcServerHandler integrationRpcServerHandler)
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
            where TIntegrationEvent : IIntegrationEventRpc
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (EventBus.ContainsKey(eventBusName + "_Server")) return;

            EventBus.Add(eventBusName + "_Server",
                new EventBusRabbitMqRpcServer(PersistentConnection, _loggerFactory, integrationRpcServerHandler,
                    null, _eventBusParameters, queueName));

            ((IEventBusRpcServer)EventBus[eventBusName + "_Server"]).Initialize();

            await ((IEventBusRpcServer)EventBus[eventBusName + "_Server"])
                .SubscribeRpcServer<TIntegrationEvent, TIntegrationEventReply,
                    TIntegrationRpcServerHandler>(routingKey);
        }

        public async ValueTask SubscribeTyped<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationEventHandler integrationEventHandler)
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent
        {
            if (EventBus.ContainsKey(eventBusName)) return;

            EventBus.Add(eventBusName,
                new EventBusRabbitMqTyped(PersistentConnection, _loggerFactory, integrationEventHandler,
                    null, _eventBusParameters, queueName));

            ((IEventBusTyped)EventBus[eventBusName]).Initialize();

            await ((IEventBusTyped)EventBus[eventBusName])
                .Subscribe<TIntegrationEvent, TIntegrationEventHandler>(routingKey);
        }

        public async ValueTask SubscribeInvoke<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            TIntegrationEventHandler integrationEventHandler
        )
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent
        {
            if (EventBus.ContainsKey(eventBusName)) return;

            EventBus.Add(eventBusName,
                new EventBusRabbitMqQueue(PersistentConnection, _loggerFactory, integrationEventHandler,
                    null, _eventBusParameters, queueName));

            ((IEventBusQueue)EventBus[eventBusName]).Initialize();
        }
    }
}