// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ.Helper
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EventBus;
    using EventBus.Abstractions;
    using EventBus.Abstractions.Handler;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using RabbitMQ.Client;

    public class Subscriber
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<EventBusRabbitMq> _loggerEventBusRabbitMq;
        private readonly IEventBusParameters _eventBusParameters;
        public IRabbitMqPersistentConnection PersistentConnection { get; }
        public ConcurrentDictionary<string, IEventBus> EventBus { get; } = new ConcurrentDictionary<string, IEventBus>();

        #region [Constructor]

        public Subscriber(
            ILoggerFactory loggerFactory,
            IConnectionFactory connectionFactory,
            IEventBusParameters eventBusParameters)
        {
            this._loggerFactory = loggerFactory;
            this._eventBusParameters = eventBusParameters;

            this.PersistentConnection = new DefaultRabbitMqPersistentConnection(connectionFactory, this._loggerFactory);
        }

        public Subscriber(
            IConnectionFactory connectionFactory,
            IEventBusParameters eventBusParameters,
            ILogger<EventBusRabbitMq> loggerEventBusRabbitMq = default,
            ILogger<DefaultRabbitMqPersistentConnection> loggerDefaultRabbitMqPersistentConnection = default)
        {
            this._eventBusParameters = eventBusParameters;

            if (loggerEventBusRabbitMq == null)
            {
                loggerEventBusRabbitMq = new NullLogger<EventBusRabbitMq>();
            }

            this._loggerEventBusRabbitMq = loggerEventBusRabbitMq;

            if (loggerDefaultRabbitMqPersistentConnection == null)
            {
                loggerDefaultRabbitMqPersistentConnection = new NullLogger<DefaultRabbitMqPersistentConnection>();
            }

            this.PersistentConnection = new DefaultRabbitMqPersistentConnection(connectionFactory, loggerDefaultRabbitMqPersistentConnection);
        }

        public Subscriber(
            ILoggerFactory loggerFactory,
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusParameters eventBusParameters)
        {
            this._loggerFactory = loggerFactory;
            this._eventBusParameters = eventBusParameters;

            this.PersistentConnection = persistentConnection;
        }

        public Subscriber(
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusParameters eventBusParameters,
            ILogger<EventBusRabbitMq> loggerEventBusRabbitMq = default)
        {
            this._eventBusParameters = eventBusParameters;

            this.PersistentConnection = persistentConnection;

            if (loggerEventBusRabbitMq == null)
            {
                loggerEventBusRabbitMq = new NullLogger<EventBusRabbitMq>();
            }
            this._loggerEventBusRabbitMq = loggerEventBusRabbitMq;
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
            await this.SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
                eventBusName, queueName,
                replyRoutingKey, integrationRpcClientHandler).ConfigureAwait(false);

            await this.SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
                eventBusName, queueName,
                routingKey, integrationRpcServerHandler).ConfigureAwait(false);
        }

        public async ValueTask SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string replyRoutingKey, TIntegrationRpcClientHandler integrationRpcClientHandler)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (this.EventBus.ContainsKey(eventBusName + "_Client"))
            {
                return;
            }

            IEventBus eventBus = null;

            if (this._loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqRpcClient(this.PersistentConnection, this._loggerFactory,
                    integrationRpcClientHandler,
                    null, this._eventBusParameters, queueName);
            }else if (this._loggerFactory == null && this._loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqRpcClient(this.PersistentConnection,
                    integrationRpcClientHandler,
                    null, this._eventBusParameters, queueName, this._loggerEventBusRabbitMq);
            }

            if (eventBus == null)
            {
                return;
            }

            if (this.EventBus.TryAdd(eventBusName + "_Client", eventBus))
            {
                ((IEventBusRpcClient)this.EventBus[eventBusName + "_Client"]).Initialize();

                await ((IEventBusRpcClient)this.EventBus[eventBusName + "_Client"])
                    .SubscribeRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(replyRoutingKey)
                    .ConfigureAwait(false);
            }
        }

        public async ValueTask SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEvent, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationRpcServerHandler integrationRpcServerHandler)
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
            where TIntegrationEvent : IIntegrationEventRpc
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (this.EventBus.ContainsKey(eventBusName + "_Server"))
            {
                return;
            }

            IEventBus eventBus = null;

            if (this._loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqRpcServer(this.PersistentConnection, this._loggerFactory,
                    integrationRpcServerHandler,
                    null, this._eventBusParameters, queueName);
            }
            else if (this._loggerFactory == null && this._loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqRpcServer(this.PersistentConnection,
                    integrationRpcServerHandler,
                    null, this._eventBusParameters, queueName, this._loggerEventBusRabbitMq);
            }

            if (eventBus == null)
            {
                return;
            }

            if (this.EventBus.TryAdd(eventBusName + "_Server", eventBus))
            {
                ((IEventBusRpcServer)this.EventBus[eventBusName + "_Server"]).Initialize();

                await ((IEventBusRpcServer)this.EventBus[eventBusName + "_Server"])
                    .SubscribeRpcServer<TIntegrationEvent, TIntegrationEventReply,
                        TIntegrationRpcServerHandler>(routingKey).ConfigureAwait(false);
            }
        }

        public async ValueTask SubscribeTyped<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationEventHandler integrationEventHandler)
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent
        {
            if (this.EventBus.ContainsKey(eventBusName))
            {
                return;
            }

            IEventBus eventBus = null;

            if (this._loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqTyped(this.PersistentConnection, this._loggerFactory, integrationEventHandler,
                    null, this._eventBusParameters, queueName);
            }
            else if (this._loggerFactory == null && this._loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqTyped(this.PersistentConnection, integrationEventHandler,
                    null, this._eventBusParameters, queueName, this._loggerEventBusRabbitMq);
            }

            if (eventBus == null)
            {
                return;
            }

            if (this.EventBus.TryAdd(eventBusName, eventBus))
            {
                ((IEventBusTyped)this.EventBus[eventBusName]).Initialize();

                await ((IEventBusTyped)this.EventBus[eventBusName])
                    .Subscribe<TIntegrationEvent, TIntegrationEventHandler>(routingKey).ConfigureAwait(false);
            }
        }

        public void SubscribeTyped(string eventBusName, string queueName = null)
        {
            if (this.EventBus.ContainsKey(eventBusName))
            {
                return;
            }

            IEventBus eventBus = null;

            if (this._loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqTyped(this.PersistentConnection, this._loggerFactory,
                    null, this._eventBusParameters, queueName);
            }
            else if (this._loggerFactory == null && this._loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqTyped(this.PersistentConnection,
                    null, this._eventBusParameters, queueName, this._loggerEventBusRabbitMq);
            }

            if (eventBus == null)
            {
                return;
            }

            if (this.EventBus.TryAdd(eventBusName, eventBus))
            {
                ((IEventBusTyped)this.EventBus[eventBusName]).Initialize();
            }
        }

        public void SubscribeInvoke<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            TIntegrationEventHandler integrationEventHandler
        )
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent
        {
            if (this.EventBus.ContainsKey(eventBusName))
            {
                return;
            }

            IEventBus eventBus = null;

            if (this._loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqQueue(this.PersistentConnection, this._loggerFactory, integrationEventHandler,
                    null, this._eventBusParameters, queueName);
            }
            else if (this._loggerFactory == null && this._loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqQueue(this.PersistentConnection, integrationEventHandler,
                    null, this._eventBusParameters, queueName, this._loggerEventBusRabbitMq);
            }

            if (eventBus == null)
            {
                return;
            }

            if (this.EventBus.TryAdd(eventBusName, eventBus))
            {
                ((IEventBusQueue)this.EventBus[eventBusName]).Initialize();
            }
        }
    }
}
