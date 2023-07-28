using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KSociety.Base.EventBusRabbitMQ.Helper
{
    public class Subscriber
    {
        private readonly ILoggerFactory? _loggerFactory;
        private readonly ILogger<EventBusRabbitMq>? _loggerEventBusRabbitMq;
        private readonly IEventBusParameters _eventBusParameters;
        public IRabbitMqPersistentConnection PersistentConnection { get; }
        public ConcurrentDictionary<string, IEventBus> EventBus { get; } = new ConcurrentDictionary<string, IEventBus>();

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
            IConnectionFactory connectionFactory,
            IEventBusParameters eventBusParameters,
            ILogger<EventBusRabbitMq>? loggerEventBusRabbitMq = default,
            ILogger<DefaultRabbitMqPersistentConnection>? loggerDefaultRabbitMqPersistentConnection = default)
        {
            _eventBusParameters = eventBusParameters;

            loggerEventBusRabbitMq ??= new NullLogger<EventBusRabbitMq>();
            _loggerEventBusRabbitMq = loggerEventBusRabbitMq;

            loggerDefaultRabbitMqPersistentConnection ??= new NullLogger<DefaultRabbitMqPersistentConnection>();

            PersistentConnection = new DefaultRabbitMqPersistentConnection(connectionFactory, loggerDefaultRabbitMqPersistentConnection);
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

        public Subscriber(
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusParameters eventBusParameters,
            ILogger<EventBusRabbitMq>? loggerEventBusRabbitMq = default)
        {
            _eventBusParameters = eventBusParameters;

            PersistentConnection = persistentConnection;

            loggerEventBusRabbitMq ??= new NullLogger<EventBusRabbitMq>();
            _loggerEventBusRabbitMq = loggerEventBusRabbitMq;
        }

        #endregion

        public async ValueTask SubscribeClientServer<
            TIntegrationRpcClientHandler, TIntegrationRpcServerHandler,
            TIntegrationEvent, TIntegrationEventReply>(
            string eventBusName, string? queueName,
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
                replyRoutingKey, integrationRpcClientHandler).ConfigureAwait(false);

            await SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
                eventBusName, queueName,
                routingKey, integrationRpcServerHandler).ConfigureAwait(false);
        }

        public async ValueTask SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
            string eventBusName, string? queueName,
            string replyRoutingKey, TIntegrationRpcClientHandler integrationRpcClientHandler)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (EventBus.ContainsKey(eventBusName + "_Client")) return;

            IEventBus? eventBus = null;

            if (_loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqRpcClient(PersistentConnection, _loggerFactory,
                    integrationRpcClientHandler,
                    null, _eventBusParameters, queueName);
            }else if (_loggerFactory == null && _loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqRpcClient(PersistentConnection,
                    integrationRpcClientHandler,
                    null, _eventBusParameters, queueName, _loggerEventBusRabbitMq);
            }

            if (eventBus == null) return;

            if (EventBus.TryAdd(eventBusName + "_Client", eventBus))
            {
                ((IEventBusRpcClient) EventBus[eventBusName + "_Client"]).Initialize();

                await ((IEventBusRpcClient) EventBus[eventBusName + "_Client"])
                    .SubscribeRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(replyRoutingKey)
                    .ConfigureAwait(false);
            }
        }

        public async ValueTask SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
            string eventBusName, string? queueName,
            string routingKey, TIntegrationRpcServerHandler integrationRpcServerHandler)
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
            where TIntegrationEvent : IIntegrationEventRpc
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (EventBus.ContainsKey(eventBusName + "_Server")) return;

            IEventBus? eventBus = null;

            if (_loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqRpcServer(PersistentConnection, _loggerFactory,
                    integrationRpcServerHandler,
                    null, _eventBusParameters, queueName);
            }
            else if (_loggerFactory == null && _loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqRpcServer(PersistentConnection,
                    integrationRpcServerHandler,
                    null, _eventBusParameters, queueName, _loggerEventBusRabbitMq);
            }

            if (eventBus == null) return;

            if (EventBus.TryAdd(eventBusName + "_Server", eventBus))
            {
                ((IEventBusRpcServer) EventBus[eventBusName + "_Server"]).Initialize();

                await ((IEventBusRpcServer) EventBus[eventBusName + "_Server"])
                    .SubscribeRpcServer<TIntegrationEvent, TIntegrationEventReply,
                        TIntegrationRpcServerHandler>(routingKey).ConfigureAwait(false);
            }
        }

        public async ValueTask SubscribeTyped<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string? queueName,
            string routingKey, TIntegrationEventHandler integrationEventHandler)
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent
        {
            if (EventBus.ContainsKey(eventBusName)) return;

            IEventBus? eventBus = null;

            if (_loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqTyped(PersistentConnection, _loggerFactory, integrationEventHandler,
                    null, _eventBusParameters, queueName);
            }
            else if (_loggerFactory == null && _loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqTyped(PersistentConnection, integrationEventHandler,
                    null, _eventBusParameters, queueName, _loggerEventBusRabbitMq);
            }

            if (eventBus == null) return;

            if (EventBus.TryAdd(eventBusName, eventBus))
            {
                ((IEventBusTyped) EventBus[eventBusName]).Initialize();

                await ((IEventBusTyped) EventBus[eventBusName])
                    .Subscribe<TIntegrationEvent, TIntegrationEventHandler>(routingKey).ConfigureAwait(false);
            }
        }

        public void SubscribeTyped(string eventBusName, string? queueName = null)
        {
            if (EventBus.ContainsKey(eventBusName)) return;

            IEventBus? eventBus = null;

            if (_loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqTyped(PersistentConnection, _loggerFactory,
                    null, _eventBusParameters, queueName);
            }
            else if (_loggerFactory == null && _loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqTyped(PersistentConnection,
                    null, _eventBusParameters, queueName, _loggerEventBusRabbitMq);
            }

            if (eventBus == null) return;

            if (EventBus.TryAdd(eventBusName, eventBus))
            {
                ((IEventBusTyped) EventBus[eventBusName]).Initialize();
            }
        }

        public void SubscribeInvoke<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string? queueName,
            TIntegrationEventHandler integrationEventHandler
        )
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent
        {
            if (EventBus.ContainsKey(eventBusName)) return;

            IEventBus? eventBus = null;

            if (_loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqQueue(PersistentConnection, _loggerFactory, integrationEventHandler,
                    null, _eventBusParameters, queueName);
            }
            else if (_loggerFactory == null && _loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqQueue(PersistentConnection, integrationEventHandler,
                    null, _eventBusParameters, queueName, _loggerEventBusRabbitMq);
            }

            if (eventBus == null) return;

            if (EventBus.TryAdd(eventBusName, eventBus))
            {
                ((IEventBusQueue) EventBus[eventBusName]).Initialize();
            }
        }
    }
}