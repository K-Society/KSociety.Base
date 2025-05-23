// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using EventBus;
    using EventBus.Abstractions;
    using EventBus.Abstractions.Handler;
    using Helper;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Polly;
    using ProtoBuf;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;

    public class EventBusRabbitMq : Disposable, IEventBus
    {
        protected readonly bool Debug;
        protected readonly IRabbitMqPersistentConnection PersistentConnection;
        protected readonly ILogger<EventBusRabbitMq> Logger;
        protected readonly IEventBusSubscriptionsManager SubsManager;
        protected readonly IEventBusParameters EventBusParameters;

        //protected readonly BasicProperties Properties;

        public IIntegrationGeneralHandler EventHandler { get; }

        protected AsyncLazy<IChannel> ConsumerChannel;
        protected string QueueName;

        #region [Constructor]

        private EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
        {
            //this.Properties = new BasicProperties
            //{
            //    DeliveryMode = DeliveryModes.Transient
            //};
            //if (eventBusParameters.Debug != null)
            //{
            //    this.Debug = eventBusParameters.Debug.Value;
            //}

            this.Debug = eventBusParameters.Debug;

            this.EventBusParameters = eventBusParameters;
            this.PersistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            this.SubsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            this.QueueName = queueName;
            this.SubsManager.OnEventRemoved += this.SubsManager_OnEventRemoved;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
            this.Logger = loggerFactory.CreateLogger<EventBusRabbitMq>();
            this.EventHandler = eventHandler;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
            this.Logger = loggerFactory.CreateLogger<EventBusRabbitMq>();
            this.EventHandler = null;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
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
            string queueName = null, ILogger<EventBusRabbitMq> logger = default) : this(persistentConnection, subsManager, eventBusParameters, queueName)
        {
            if (this.Logger == null)
            {
                this.Logger = new NullLogger<EventBusRabbitMq>();
            }
            this.Logger = logger;

            this.EventHandler = null;
        }

        #endregion

        public virtual bool Initialize<TIntegrationEvent>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
        {

            this.ConsumerChannel =
            new AsyncLazy<IChannel>(async () => await this.CreateConsumerChannelAsync<TIntegrationEvent>(cancel).ConfigureAwait(false));           

            if (this.ConsumerChannel != null)
            {
                return true;
            }

            return false;
        }

        private async void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (this.SubsManager is null)
            {
                return;
            }

            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
                    this.Logger.LogWarning("EventBusRabbitMq SubsManager_OnEventRemoved: {0}!", "no connection");
                    return;
                }
            }

            using (var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false))
            {
                if (channel != null)
                {
                    if (!String.IsNullOrEmpty(this.QueueName) &&
                        !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                    {
                        await channel.QueueUnbindAsync(this.QueueName,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                            eventName).ConfigureAwait(false);

                        if (!this.SubsManager.IsEmpty)
                        {
                            return;
                        }

                        this.QueueName = String.Empty;
                        if (this.ConsumerChannel != null)
                        {
                            await (await this.ConsumerChannel).CloseAsync().ConfigureAwait(false);
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
                    var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                    if (!connectionResult)
                    {
                        this.Logger.LogWarning("EventBusRabbitMq Publish: {0}!", "no connection");
                        return;
                    }
                }

                var policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .Or<Exception>()
                    .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        this.Logger.LogWarning(ex, "Publish: ");
                    });

                using (var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false))
                {
                    if (channel != null)
                    {
                        var routingKey = @event.RoutingKey;
                        if (this.EventBusParameters.ExchangeDeclareParameters != null)
                        {
                            await channel.ExchangeDeclareAsync(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete).ConfigureAwait(false);

                            using (var ms = new MemoryStream())
                            {
                                Serializer.Serialize(ms, @event);
                                var body = ms.ToArray();

                                await policy.Execute(async () =>
                                {
                                    //var properties = channel.CreateBasicProperties();
                                    //var properties = BasicProperties();
                                    //properties.DeliveryMode = 1; //2 = persistent, write on disk

                                    var props = new BasicProperties
                                    {
                                        DeliveryMode = DeliveryModes.Transient
                                    };

                                    await channel.BasicPublishAsync(
                                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                        routingKey, true, props,
                                        body).ConfigureAwait(false);
                                }).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMq Publish: ");
            }
        }

        protected virtual async ValueTask<(QueueDeclareOk, QueueDeclareOk)> QueueInitialize(IChannel channel)
        {
            try
            {
                if (this.EventBusParameters is null)
                {
                    return (null, null);
                }

                //if (this.EventBusParameters != null)
                //{
                await channel.ExchangeDeclareAsync(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                    this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                    this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                    this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete).ConfigureAwait(false);
                    //var args = new Dictionary<string, object>
                    //{
                    //    { "x-dead-letter-exchange", EventBusParameters.ExchangeDeclareParameters.ExchangeName }
                    //    //{"x-dead-letter-routing-key", "some-routing-key" }
                    //};
                    var result = await channel.QueueDeclareAsync(this.QueueName, this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null).ConfigureAwait(false);
                //}

                return (result, null);
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger.LogError(rex, "EventBusRabbitMq RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMq QueueInitialize: ");
            }

            return (null, null);
        }

        #region [Subscribe]

        public async ValueTask<bool> Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = this.SubsManager.GetEventKey<TIntegrationEvent>();
            var internalSubscriptionResult = await this.DoInternalSubscription(eventName).ConfigureAwait(false);

            if (internalSubscriptionResult)
            {
                this.SubsManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();

                return await this.StartBasicConsumeAsync<TIntegrationEvent>().ConfigureAwait(false);
            }

            return false;
        }

        protected async ValueTask<bool> DoInternalSubscription(string eventName)
        {
            //var containsKey = this.SubsManager.HasSubscriptionsForEvent(eventName);
            if (this.SubsManager.HasSubscriptionsForEvent(eventName))
            {
                return false;
            }

            if (this.PersistentConnection is null)
            {
                return false;
            }

            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
                    this.Logger.LogWarning("EventBusRabbitMq DoInternalSubscriptionRpc: {0}!", "no connection");
                    return false;
                }
            }

            using (var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false))
            {
                if (channel != null)
                {
                    var result = await this.QueueInitialize(channel).ConfigureAwait(false);

                    if (result.Item1 != null)
                    {
                        if (!String.IsNullOrEmpty(this.QueueName) &&
                            !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                        {
                            await channel.QueueBindAsync(this.QueueName,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName).ConfigureAwait(false);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        #region [Unsubscribe]

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            this.SubsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        #endregion

        #region [Dispose]

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ConsumerChannel.Value.Dispose();
                this.SubsManager.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion

        public virtual async ValueTask<uint> QueuePurge(CancellationToken cancellationToken = default)
        {
            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger.LogWarning("ConsumerChannel is null!");
                    return 0;
                }

                if (this.ConsumerChannel.Value != null && !String.IsNullOrEmpty(this.QueueName))
                {
                    var result = await (await this.ConsumerChannel).QueuePurgeAsync(this.QueueName, cancellationToken);

                    return result;
                }

                this.Logger.LogError("QueuePurge can't call on ConsumerChannel is null or queue name is null.");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "QueuePurge: ");
            }

            return 0;
        }

        public virtual async ValueTask<uint> QueueReplyPurge(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1).ConfigureAwait(false);
            return 0;
        }

        protected virtual async ValueTask<bool> StartBasicConsume<TIntegrationEvent>()
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            //this.Logger.LogTrace("EventBusRabbitMq Starting RabbitMQ basic consume.");
            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger.LogWarning("ConsumerChannel is null!");
                    return false;
                }

                if (this.ConsumerChannel.Value != null)
                {
                    var model = this.ConsumerChannel.Value.Result;
                    var consumer = new AsyncEventingBasicConsumer(model);

                    consumer.ReceivedAsync += this.ConsumerReceived<TIntegrationEvent>;

                    await model.BasicConsumeAsync(
                        queue: this.QueueName,
                        autoAck: false,
                        consumer: consumer).ConfigureAwait(false);

                    //this.Logger.LogInformation("EventBusRabbitMq StartBasicConsume done. Queue name: {0}, autoAck: {1}", this.QueueName, true);

                    return true;
                }

                this.Logger.LogError("StartBasicConsume can't call on ConsumerChannel is null");
                
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "StartBasicConsume: ");
            }

            return false;
        }

        protected virtual async ValueTask<bool> StartBasicConsumeAsync<TIntegrationEvent>()
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            //this.Logger.LogTrace("EventBusRabbitMq Starting RabbitMQ basic consume.");
            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger.LogWarning("ConsumerChannel is null!");
                    return false;
                }

                if (this.ConsumerChannel.Value != null)
                {
                    var asyncConsumer = new AsyncEventingBasicConsumer(await this.ConsumerChannel);

                    asyncConsumer.ReceivedAsync += this.ConsumerReceivedAsync<TIntegrationEvent>;

                    await (await this.ConsumerChannel).BasicConsumeAsync(
                        queue: this.QueueName,
                        autoAck: false,
                        consumer: asyncConsumer).ConfigureAwait(false);

                    //this.Logger.LogInformation("EventBusRabbitMq StartBasicConsume done. Queue name: {0}, autoAck: {1}", this.QueueName, true);

                    return true;
                }

                this.Logger.LogError("StartBasicConsume can't call on ConsumerChannel is null");

            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "StartBasicConsume: ");
            }

            return false;
        }

        protected virtual async Task ConsumerReceived<TIntegrationEvent>(object sender, BasicDeliverEventArgs eventArgs)
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                await this.ProcessEventAsync<TIntegrationEvent>(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "ConsumerReceived: {0}", eventName);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                await this.ConsumerChannel.Value.Result.BasicAckAsync(eventArgs.DeliveryTag, multiple: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
            }
        }

        protected virtual async Task ConsumerReceivedAsync<TIntegrationEvent>(object sender, BasicDeliverEventArgs eventArgs)
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                await this.ProcessEventAsync<TIntegrationEvent>(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "ConsumerReceived: {0}", eventName);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                if (this.ConsumerChannel != null)
                {
                    await (await this.ConsumerChannel).BasicAckAsync(eventArgs.DeliveryTag, multiple: false).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
            }
        }

        protected virtual async ValueTask<IChannel> CreateConsumerChannelAsync<TIntegrationEvent>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            //this.Logger.LogTrace("CreateConsumerChannelAsync queue name: {0}", this.QueueName);
            if (this.PersistentConnection is null)
            {
                return null;
            }

            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
                    this.Logger.LogWarning("EventBusRabbitMq CreateConsumerChannelAsync: {0}!", "no connection");
                    return null;
                }
            }

            var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false);
            if (channel != null)
            {
                //this.QueueInitialize(channel);

                await channel.BasicQosAsync(0, 1, false).ConfigureAwait(false);

                channel.CallbackExceptionAsync += async (sender, ea) =>
                {
                    if (!String.IsNullOrEmpty(this.QueueName) &&
                        !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                    {
                        this.Logger.LogError(ea.Exception, "CallbackException ExchangeName: {0} - QueueName: {1}",
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, this.QueueName);
                    }

                    this.ConsumerChannel.Value.Dispose();
                    this.ConsumerChannel = new AsyncLazy<IChannel>(async () =>
                        await this.CreateConsumerChannelAsync<TIntegrationEvent>(cancel).ConfigureAwait(false));
                    await this.StartBasicConsumeAsync<TIntegrationEvent>().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        //protected virtual async ValueTask ProcessEvent(string routingKey, string eventName,
        //    ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        protected virtual async ValueTask ProcessEventAsync<TIntegrationEvent>(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
        {
            if (this.SubsManager is null)
            {
                return;
            }
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
                                        this.Logger.LogError("ProcessEvent Typed: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEvent>(ms);
                                        var concreteType =
                                            typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                                        var generic = concreteType.GetMethod("Handle");

                                        var result = (ValueTask)generic.Invoke(this.EventHandler,
                                            new[] { (object)integrationEvent, cancel });

                                        await result.ConfigureAwait(false);

                                        //await ((ValueTask)concreteType.GetMethod("Handle")
                                        //        .Invoke(this.EventHandler, new[] {(object)integrationEvent, cancel}))
                                        //    .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, "ProcessEvent Typed: ");
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
                                        this.Logger.LogError("ProcessQueue: eventType is null! {0}", routingKey);
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
                                this.Logger.LogError(ex, "ProcessQueue: ");
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
                                        this.Logger.LogError("ProcessEvent: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    var eventResultType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        this.Logger.LogError("ProcessEvent: eventResultType is null! {0}", routingKey);
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
                                this.Logger.LogError(ex, "ProcessEvent Rpc: ");
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
