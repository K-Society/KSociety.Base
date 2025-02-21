// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ.Helper
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using EventBus;
    using EventBus.Abstractions;
    using EventBus.Abstractions.Handler;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using RabbitMQ.Client;

    public class Subscriber : ISubscriber
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<EventBusRabbitMq> _loggerEventBusRabbitMq;
        private readonly IEventBusParameters _eventBusParameters;
        private readonly bool _purgeQueue;
        public IRabbitMqPersistentConnection PersistentConnection { get; }
        public ConcurrentDictionary<string, IEventBus> EventBus { get; }

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

        #region [Constructor]

        public Subscriber(
            ILoggerFactory loggerFactory,
            IConnectionFactory connectionFactory,
            IEventBusParameters eventBusParameters, int eventBusNumber, bool purgeQueue = false)
        {
            this._loggerFactory = loggerFactory;
            this._eventBusParameters = eventBusParameters;
            this.PersistentConnection = new DefaultRabbitMqPersistentConnection(connectionFactory, this._loggerFactory);

            this.EventBus = new ConcurrentDictionary<string, IEventBus>(1, eventBusNumber);
            this._purgeQueue = purgeQueue;
        }

        public Subscriber(
            IConnectionFactory connectionFactory,
            IEventBusParameters eventBusParameters,
            int eventBusNumber,
            ILogger<EventBusRabbitMq> loggerEventBusRabbitMq = default,
            ILogger<DefaultRabbitMqPersistentConnection> loggerDefaultRabbitMqPersistentConnection = default,
            bool purgeQueue = false)
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

            this.EventBus = new ConcurrentDictionary<string, IEventBus>(1, eventBusNumber);

            this._purgeQueue = purgeQueue;
        }

        public Subscriber(
            ILoggerFactory loggerFactory,
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusParameters eventBusParameters, int eventBusNumber,
            bool purgeQueue = false)
        {
            this._loggerFactory = loggerFactory;
            this._eventBusParameters = eventBusParameters;
            this.PersistentConnection = persistentConnection;

            this.EventBus = new ConcurrentDictionary<string, IEventBus>(1, eventBusNumber);

            this._purgeQueue = purgeQueue;
        }

        public Subscriber(
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusParameters eventBusParameters, int eventBusNumber,
            ILogger<EventBusRabbitMq> loggerEventBusRabbitMq = default,
            bool purgeQueue = false)
        {
            this._eventBusParameters = eventBusParameters;
            this.PersistentConnection = persistentConnection;

            if (loggerEventBusRabbitMq == null)
            {
                loggerEventBusRabbitMq = new NullLogger<EventBusRabbitMq>();
            }
            this._loggerEventBusRabbitMq = loggerEventBusRabbitMq;
            this.EventBus = new ConcurrentDictionary<string, IEventBus>(1, eventBusNumber);

            this._purgeQueue = purgeQueue;
        }

        #endregion

        public async ValueTask<bool> SubscribeClientServer<
            TIntegrationRpcClientHandler, TIntegrationRpcServerHandler,
            TIntegrationEventRpc, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string routingKey, string replyRoutingKey,
            TIntegrationRpcClientHandler integrationRpcClientHandler,
            TIntegrationRpcServerHandler integrationRpcServerHandler
        )
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            var clientResult = await this.SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
                eventBusName, queueName,
                replyRoutingKey, integrationRpcClientHandler).ConfigureAwait(false);

            var serverResult = await this.SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEventRpc, TIntegrationEventReply>(
                eventBusName, queueName,
                routingKey, integrationRpcServerHandler).ConfigureAwait(false);

            return clientResult && serverResult;
        }

        public async ValueTask<bool> SubscribeClient<TIntegrationRpcClientHandler, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string replyRoutingKey, TIntegrationRpcClientHandler integrationRpcClientHandler)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            if (this.EventBus.ContainsKey(eventBusName + "_Client"))
            {
                return false;
            }

            IEventBus eventBus = null;

            if (this._loggerFactory != null)
            {
                eventBus = new EventBusRabbitMqRpcClient<TIntegrationEventReply>(this.PersistentConnection, this._loggerFactory,
                    integrationRpcClientHandler,
                    null, this._eventBusParameters, queueName);
            }else if (this._loggerFactory == null && this._loggerEventBusRabbitMq != null)
            {
                eventBus = new EventBusRabbitMqRpcClient<TIntegrationEventReply>(this.PersistentConnection,
                    integrationRpcClientHandler,
                    null, this._eventBusParameters, queueName, this._loggerEventBusRabbitMq);
            }

            if (eventBus == null)
            {
                return false;
            }

            if (this.EventBus.TryAdd(eventBusName + "_Client", eventBus))
            {
                ((IEventBusRpcClient<TIntegrationEventReply>)this.EventBus[eventBusName + "_Client"]).Initialize();

                var result = await ((IEventBusRpcClient<TIntegrationEventReply>)this.EventBus[eventBusName + "_Client"])
                    .SubscribeRpcClient<TIntegrationRpcClientHandler>(replyRoutingKey)
                    .ConfigureAwait(false);

                //if (result && this._purgeQueue)
                //{
                //    await this.QueuesPurge().ConfigureAwait(false);
                //}

                return result;
            }

            return false;
        }

        public async ValueTask<bool> SubscribeServer<TIntegrationRpcServerHandler, TIntegrationEventRpc, TIntegrationEventReply>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationRpcServerHandler integrationRpcServerHandler)
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            if (this.EventBus.ContainsKey(eventBusName + "_Server"))
            {
                return false;
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
                return false;
            }

            if (this.EventBus.TryAdd(eventBusName + "_Server", eventBus))
            {
                ((IEventBusRpcServer)this.EventBus[eventBusName + "_Server"]).InitializeServer<TIntegrationEventRpc, TIntegrationEventReply>();//.Initialize();

                var result = await ((IEventBusRpcServer)this.EventBus[eventBusName + "_Server"])
                    .SubscribeRpcServer<TIntegrationEventRpc, TIntegrationEventReply,
                        TIntegrationRpcServerHandler>(routingKey).ConfigureAwait(false);

                //if (result && this._purgeQueue)
                //{
                //    await this.QueuesPurge().ConfigureAwait(false);
                //}

                return result;
            }

            return false;
        }

        public async ValueTask<bool> SubscribeTyped<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            string routingKey, TIntegrationEventHandler integrationEventHandler)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            if (this.EventBus.ContainsKey(eventBusName))
            {
                return false;
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
                return false;
            }

            if (this.EventBus.TryAdd(eventBusName, eventBus))
            {
                ((IEventBusTyped)this.EventBus[eventBusName]).Initialize<TIntegrationEvent>();

                var result = await ((IEventBusTyped)this.EventBus[eventBusName])
                    .Subscribe<TIntegrationEvent, TIntegrationEventHandler>(routingKey).ConfigureAwait(false);

                //if (result && this._purgeQueue)
                //{
                //    await this.QueuesPurge().ConfigureAwait(false);
                //}
                return result;
            }

            return false;
        }

        public async ValueTask<bool> SubscribeTyped<TIntegrationEvent>(string eventBusName, string queueName = null)
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            if (this.EventBus.ContainsKey(eventBusName))
            {
                return false;
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
                return false;
            }

            if (this.EventBus.TryAdd(eventBusName, eventBus))
            {
                var result = ((IEventBusTyped)this.EventBus[eventBusName]).Initialize<TIntegrationEvent>();

                //if (result && this._purgeQueue)
                //{
                //    await this.QueuesPurge().ConfigureAwait(false);
                //}
                await Task.Delay(1).ConfigureAwait(false);
                return result;
            }

            return false;
        }

        public async ValueTask<bool> SubscribeInvoke<TIntegrationEventHandler, TIntegrationEvent>(
            string eventBusName, string queueName,
            TIntegrationEventHandler integrationEventHandler
        )
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            if (this.EventBus.ContainsKey(eventBusName))
            {
                return false;
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
                return false;
            }

            if (this.EventBus.TryAdd(eventBusName, eventBus))
            {
                var result = ((IEventBusQueue)this.EventBus[eventBusName]).Initialize<TIntegrationEvent>();

                //if (result && this._purgeQueue)
                //{
                //    await this.QueuesPurge().ConfigureAwait(false);
                //}
                await Task.Delay(1).ConfigureAwait(false);
                return result;
            }
            return false;
        }

        public async ValueTask QueuesPurge(CancellationToken cancellationToken = default)
        {
            await this._connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (this.EventBus.Count > 0)
                {
                    foreach (var eventBus in this.EventBus)
                    {
                        var queuePurge = await eventBus.Value.QueuePurge(cancellationToken).ConfigureAwait(false);

                        var queueReplyPurge = await eventBus.Value.QueueReplyPurge(cancellationToken).ConfigureAwait(false);

                        this._loggerEventBusRabbitMq?.LogTrace("QueuesPurge eventBus: {0}, queuePurge: {1}, queueReplyPurge: {2}", eventBus.Key, queuePurge, queueReplyPurge);
                    }
                }
            }catch(Exception ex)
            {
                this._loggerEventBusRabbitMq?.LogError(ex, "QueuesPurge: ");
            }
            finally
            {
                this._connectionLock.Release();
            }
        }
    }
}
