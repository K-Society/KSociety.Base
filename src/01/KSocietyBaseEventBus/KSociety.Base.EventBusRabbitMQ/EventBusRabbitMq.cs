// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using EventBus;
    using EventBus.Abstractions;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;
    using InfraSub.Shared.Class;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Polly;
    using ProtoBuf;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class EventBusRabbitMq : DisposableObject, IEventBus
    {
        protected readonly bool Debug;
        protected readonly IRabbitMqPersistentConnection? PersistentConnection;
        protected readonly ILogger<EventBusRabbitMq>? Logger;
        protected readonly IEventBusSubscriptionsManager? SubsManager;
        protected readonly IEventBusParameters? EventBusParameters;

        public IIntegrationGeneralHandler? EventHandler { get; }

        protected AsyncLazy<IModel>? ConsumerChannel;
        protected string? QueueName;

        #region [Constructor]

        private EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null)
        {
            if (eventBusParameters.Debug != null)
            {
                this.Debug = eventBusParameters.Debug.Value;
            }

            this.EventBusParameters = eventBusParameters;
            this.PersistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            this.SubsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            this.QueueName = queueName;
            this.SubsManager.OnEventRemoved += this.SubsManager_OnEventRemoved;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
            this.Logger = loggerFactory.CreateLogger<EventBusRabbitMq>();
            this.EventHandler = eventHandler;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
            this.Logger = loggerFactory.CreateLogger<EventBusRabbitMq>();
            this.EventHandler = null;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null, ILogger<EventBusRabbitMq>? logger = default) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
            //this.Logger ??= new NullLogger<EventBusRabbitMq>();
            if (this.Logger == null)
            {
                this.Logger = new NullLogger<EventBusRabbitMq>();
            }
            this.Logger = logger;

            this.EventHandler = eventHandler;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null, ILogger<EventBusRabbitMq>? logger = default) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
            //this.Logger ??= new NullLogger<EventBusRabbitMq>();
            if (this.Logger == null)
            {
                this.Logger = new NullLogger<EventBusRabbitMq>();
            }
            this.Logger = logger;

            this.EventHandler = null;
        }

        #endregion

        public virtual void Initialize(CancellationToken cancel = default)
        {
            //this.Logger?.LogTrace("EventBusRabbitMq Initialize.");
            this.ConsumerChannel =
                new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
        }

        private async void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using (var channel = this.PersistentConnection.CreateModel())
            {
                if (channel != null)
                {
                    if (!String.IsNullOrEmpty(this.QueueName) &&
                        !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters?.ExchangeName))
                    {
                        channel.QueueUnbind(this.QueueName,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                            eventName);

                        if (!this.SubsManager.IsEmpty)
                        {
                            return;
                        }

                        this.QueueName = String.Empty;
                        if (this.ConsumerChannel != null)
                        {
                            (await this.ConsumerChannel)?.Close();
                        }
                    }
                }
            }
        }

        public virtual async ValueTask Publish(IIntegrationEvent @event)
        {
            try
            {
                if (!this.PersistentConnection.IsConnected)
                {
                    await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                }

                var policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .Or<Exception>()
                    .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        this.Logger?.LogWarning(ex, "Publish: ");
                    });

                using (var channel = this.PersistentConnection.CreateModel())
                {
                    if (channel != null)
                    {
                        var routingKey = @event.RoutingKey;
                        if (this.EventBusParameters.ExchangeDeclareParameters != null)
                        {
                            channel.ExchangeDeclare(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

                            using (var ms = new MemoryStream())
                            {
                                Serializer.Serialize(ms, @event);
                                var body = ms.ToArray();

                                policy.Execute(() =>
                                {
                                    var properties = channel.CreateBasicProperties();
                                    properties.DeliveryMode = 1; //2 = persistent, write on disk

                                    channel.BasicPublish(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                        routingKey, true, properties,
                                        body);
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EventBusRabbitMq Publish: ");
            }
        }

        protected virtual void QueueInitialize(IModel channel)
        {
            try
            {
                if (this.EventBusParameters != null)
                {
                    channel?.ExchangeDeclare(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);
                    //var args = new Dictionary<string, object>
                    //{
                    //    { "x-dead-letter-exchange", EventBusParameters.ExchangeDeclareParameters.ExchangeName }
                    //    //{"x-dead-letter-routing-key", "some-routing-key" }
                    //};
                    channel?.QueueDeclare(this.QueueName, this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger?.LogError(rex, "EventBusRabbitMq RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EventBusRabbitMq QueueInitialize: ");
            }
        }

        #region [Subscribe]

        public async ValueTask Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {

            var eventName = this.SubsManager.GetEventKey<T>();
            await this.DoInternalSubscription(eventName).ConfigureAwait(false);
            this.SubsManager.AddSubscription<T, TH>();
            await this.StartBasicConsume().ConfigureAwait(false);
        }

        protected async ValueTask DoInternalSubscription(string eventName)
        {
            var containsKey = this.SubsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey)
            {
                return;
            }

            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using (var channel = this.PersistentConnection.CreateModel())
            {
                if (channel != null)
                {
                    this.QueueInitialize(channel);

                    if (!String.IsNullOrEmpty(this.QueueName) &&
                        !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters?.ExchangeName))
                    {
                        channel.QueueBind(this.QueueName,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
                    }
                }
            }
        }

        #endregion

        #region [Unsubscribe]

        public void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            this.SubsManager.RemoveSubscription<T, TH>();
        }

        #endregion

        protected override void DisposeManagedResources()
        {
            this.ConsumerChannel?.Value.Dispose();
            this.SubsManager?.Clear();
        }

        protected virtual async ValueTask<bool> StartBasicConsume()
        {
            //this.Logger?.LogTrace("EventBusRabbitMq Starting RabbitMQ basic consume.");
            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger?.LogWarning("ConsumerChannel is null!");
                    return false;
                }

                if (this.ConsumerChannel?.Value != null)
                {
                    var consumer = new AsyncEventingBasicConsumer(await this.ConsumerChannel);

                    consumer.Received += this.ConsumerReceivedAsync;

                    (await this.ConsumerChannel).BasicConsume(
                        queue: this.QueueName,
                        autoAck: false,
                        consumer: consumer);

                    //this.Logger?.LogInformation("EventBusRabbitMq StartBasicConsume done. Queue name: {0}, autoAck: {1}", this.QueueName, true);

                    return true;
                }

                this.Logger?.LogError("StartBasicConsume can't call on ConsumerChannel is null");
                
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "StartBasicConsume: ");
            }

            return false;
        }

        protected virtual void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                this.ProcessEvent(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogWarning(ex, "ConsumerReceived: {0}", eventName);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                this.ConsumerChannel?.Value.Result?.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
            }
        }

        protected virtual async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                await this.ProcessEvent(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogWarning(ex, "ConsumerReceived: {0}", eventName);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                if (this.ConsumerChannel != null)
                {
                    (await this.ConsumerChannel)?.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
            }
        }

        protected virtual async ValueTask<IModel> CreateConsumerChannelAsync(CancellationToken cancel = default)
        {
            //this.Logger?.LogTrace("CreateConsumerChannelAsync queue name: {0}", this.QueueName);

            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            var channel = this.PersistentConnection.CreateModel();
            if (channel != null)
            {
                this.QueueInitialize(channel);

                channel.BasicQos(0, 1, false);

                channel.CallbackException += async (sender, ea) =>
                {
                    if (!String.IsNullOrEmpty(this.QueueName) &&
                        !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters?.ExchangeName))
                    {
                        this.Logger?.LogError(ea.Exception, "CallbackException ExchangeName: {0} - QueueName: {1}",
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, this.QueueName);
                    }

                    this.ConsumerChannel?.Value.Dispose();
                    this.ConsumerChannel = new AsyncLazy<IModel>(async () =>
                        await this.CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
                    await this.StartBasicConsume().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        protected virtual async ValueTask ProcessEvent(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            if (this.SubsManager.HasSubscriptionsForEvent(routingKey))
            {

                var subscriptions = this.SubsManager.GetHandlersForEvent(routingKey);
                foreach (var subscription in subscriptions)
                {

                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Dynamic:
                            break;

                        case SubscriptionManagerType.Typed:
                            try
                            {
                                if (this.EventHandler is null)
                                {

                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger?.LogError("ProcessEvent Typed: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType =
                                            typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                                        await ((ValueTask)concreteType.GetMethod("Handle")
                                                .Invoke(this.EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger?.LogError(ex, "ProcessEvent Typed: ");
                            }

                            break;

                        case SubscriptionManagerType.Queue:
                            try
                            {
                                if (this.EventHandler is null)
                                {

                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger?.LogError("ProcessQueue: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType =
                                            typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);
                                        await ((ValueTask<bool>)concreteType.GetMethod("Enqueue")
                                                .Invoke(this.EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger?.LogError(ex, "ProcessQueue: ");
                            }

                            break;

                        case SubscriptionManagerType.Rpc:
                            try
                            {
                                if (this.EventHandler is null)
                                {

                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger?.LogError("ProcessEvent: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    var eventResultType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        this.Logger?.LogError("ProcessEvent: eventResultType is null! {0}", routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                        var concreteType =
                                            typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType,
                                                eventResultType);
                                        await ((ValueTask)concreteType.GetMethod("HandleReply")
                                                .Invoke(this.EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger?.LogError(ex, "ProcessEvent Rpc: ");
                            }

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        } //ProcessEvent.
    }
}
