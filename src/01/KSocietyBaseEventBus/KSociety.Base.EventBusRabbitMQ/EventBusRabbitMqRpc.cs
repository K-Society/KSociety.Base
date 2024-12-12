// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using EventBus;
    using EventBus.Abstractions;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;
    using Microsoft.Extensions.Logging;
    using Polly;
    using ProtoBuf;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Helper;

    public sealed class EventBusRabbitMqRpc : EventBusRabbitMq, IEventBusRpc
    {
        private AsyncLazy<IChannel> _consumerChannelReply;
        private string _queueNameReply;

        private readonly string _correlationId;

        #region [Constructor]

        public EventBusRabbitMqRpc(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {
            this._correlationId = Guid.NewGuid().ToString();
        }

        public EventBusRabbitMqRpc(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {
            this._correlationId = Guid.NewGuid().ToString();
        }

        #endregion

        public bool Initialize<TIntegrationEvent, TIntegrationEventReply>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            //this.Logger.LogTrace("EventBusRabbitMqRpc Initialize.");
            if (this.SubsManager != null)
            {
                this.SubsManager.OnEventReplyRemoved += this.SubsManager_OnEventReplyRemoved;
            }

            this.ConsumerChannel =
                new AsyncLazy<IChannel>(async () => await this.CreateConsumerChannelServerAsync<TIntegrationEvent, TIntegrationEventReply>(cancel).ConfigureAwait(false));
            this._queueNameReply = this.QueueName + "_Reply";
            this._consumerChannelReply =
                new AsyncLazy<IChannel>(async () => await this.CreateConsumerChannelReplyAsync<TIntegrationEventReply>(cancel).ConfigureAwait(false));

            if (this.ConsumerChannel != null && this._consumerChannelReply != null)
            {
                return true;
            }

            return false;
        }

        public IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply> GetIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            if (this.EventHandler is IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply> queue)
            {
                return queue;
            }

            return null;
        }

        private async void SubsManager_OnEventReplyRemoved(object sender, string eventName)
        {
            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpc SubsManager_OnEventReplyRemoved: {0}!", "no connection");
                    return;
                }
            }

            using (var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false))
            {
                if (!String.IsNullOrEmpty(this._queueNameReply) &&
                    !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                {
                    await channel.QueueUnbindAsync(this._queueNameReply,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                        eventName).ConfigureAwait(false);
                }
            }

            if (!this.SubsManager.IsReplyEmpty)
            {
                return;
            }

            this._queueNameReply = String.Empty;
            if (this._consumerChannelReply != null)
            {
                await (await this._consumerChannelReply).CloseAsync().ConfigureAwait(false);
            }
        }

        public override async ValueTask Publish(IIntegrationEvent @event)
        {
            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpc Publish: {0}!", "no connection");
                    return;
                }
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .Or<Exception>()
                .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    this.Logger.LogWarning(ex, "Publish:");
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
                                //properties.DeliveryMode = 1; //2 = persistent, write on disk
                                //properties.CorrelationId = this._correlationId;
                                //properties.ReplyTo = this._queueNameReply;
                                var props = new BasicProperties
                                {
                                    DeliveryMode = DeliveryModes.Transient,
                                    CorrelationId = this._correlationId,
                                    ReplyTo = this._queueNameReply
                                };

                                await channel.BasicPublishAsync(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                    routingKey,
                                    true,
                                    props, body).ConfigureAwait(false);
                            }).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        protected override async ValueTask<(QueueDeclareOk, QueueDeclareOk)> QueueInitialize(IChannel channel)
        {
            try
            {
                if (this.EventBusParameters != null)
                {
                    await channel.ExchangeDeclareAsync(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete).ConfigureAwait(false);

                    //var args = new Dictionary<string, object>
                    //{
                    //    { "x-dead-letter-exchange", EventBusParameters.ExchangeDeclareParameters.ExchangeName }
                    //};

                    var resultQueue = await channel.QueueDeclareAsync(this.QueueName,
                        this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null).ConfigureAwait(false);

                    var resultQueueReply =  await channel.QueueDeclareAsync(this._queueNameReply,
                        this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null).ConfigureAwait(false);

                    return (resultQueue, resultQueueReply);
                }

                return (null, null);
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger.LogError(rex, "EventBusRabbitMqRpc RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMqRpc QueueInitialize: ");
            }

            return (null, null);
        }

        #region [Subscribe]

        public async ValueTask<bool> SubscribeRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        {
            var eventName = this.SubsManager.GetEventKey<TIntegrationEvent>();
            var eventNameResult = this.SubsManager.GetEventReplyKey<TIntegrationEventReply>();
            //this.Logger.LogDebug("SubscribeRpc: eventName: {0}.{1} eventNameResult: {2}.{3}", eventName, routingKey, eventNameResult, routingKey);
            var internalSubscriptionResult = await this.DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey)
                .ConfigureAwait(false);
            if (internalSubscriptionResult)
            {
                this.SubsManager.AddSubscriptionRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(eventName + "." + routingKey, eventNameResult + "." + routingKey);
                await this.StartBasicConsumeServer<TIntegrationEvent, TIntegrationEventReply>().ConfigureAwait(false);
                await this.StartBasicConsumeReplyAsync<TIntegrationEventReply>().ConfigureAwait(false);

                return true;
            }

            return false;
        }

        private async ValueTask<bool> DoInternalSubscriptionRpc(string eventName, string eventNameResult)
        {
            try
            {
                //var containsKey = this.SubsManager.HasSubscriptionsForEvent(eventName);
                if (this.SubsManager.HasSubscriptionsForEvent(eventName))
                {
                    return false;
                }

                if (!this.PersistentConnection.IsConnected)
                {
                    var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                    if (!connectionResult)
                    {
                        this.Logger.LogWarning("EventBusRabbitMqRpc DoInternalSubscriptionRpc: {0}!", "no connection");
                        return false;
                    }
                }

                using (var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false))
                {
                    if (channel != null)
                    {
                        var result = await this.QueueInitialize(channel).ConfigureAwait(false);

                        if (result.Item1 != null && result.Item2 != null)
                        {
                            if (!String.IsNullOrEmpty(this.QueueName) &&
                                !String.IsNullOrEmpty(this._queueNameReply) &&
                                !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                            {
                                await channel.QueueBindAsync(this.QueueName,
                                    this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName).ConfigureAwait(false);
                                await channel.QueueBindAsync(this._queueNameReply,
                                    this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                    eventNameResult).ConfigureAwait(false);

                                return true;
                            }
                        }
                    }
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger.LogError(rex, "EventBusRabbitMqRpc RabbitMQClientException DoInternalSubscriptionRpc: ");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMqRpc DoInternalSubscriptionRpc: ");
            }

            return false;
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        {
            this.SubsManager.RemoveSubscriptionRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(routingKey);
        }

        #endregion

        #region [Dispose]

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._consumerChannelReply.Value.Dispose();
                this.ConsumerChannel.Value.Dispose();
                this.SubsManager.Clear();
                this.SubsManager.ClearReply();
            }

            base.Dispose(disposing);
        }

        #endregion

        public async ValueTask<uint> QueueReplyPurge(CancellationToken cancellationToken = default)
        {
            try
            {
                if (this._consumerChannelReply is null)
                {
                    this.Logger.LogWarning("ConsumerChannelReply is null!");
                    return 0;
                }

                if (this._consumerChannelReply.Value != null && !String.IsNullOrEmpty(this._queueNameReply))
                {
                    var result = await (await this._consumerChannelReply).QueuePurgeAsync(this._queueNameReply, cancellationToken);

                    return result;
                }

                this.Logger.LogError("QueueReplyPurge can't call on ConsumerChannelReply is null or queue name is null.");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "QueueReplyPurge: ");
            }

            return 0;
        }

        protected async ValueTask<bool> StartBasicConsumeServer<TIntegrationEvent, TIntegrationEventReply>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            //this.Logger.LogTrace("Starting RabbitMQ basic consume");
            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpc ConsumerChannel is null!");
                    return false;
                }

                if (this.ConsumerChannel.Value != null)
                {
                    var consumer = new AsyncEventingBasicConsumer(await this.ConsumerChannel);

                    consumer.ReceivedAsync += this.ConsumerReceivedServerAsync<TIntegrationEvent, TIntegrationEventReply>;

                    await (await this.ConsumerChannel).BasicConsumeAsync(
                        queue: this.QueueName,
                        autoAck: false,
                        consumer: consumer).ConfigureAwait(false);

                    return true;
                }

                this.Logger.LogError("StartBasicConsume can't call on ConsumerChannel == null");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "StartBasicConsume: ");
            }

            return false;
        }

        private async ValueTask StartBasicConsumeReplyAsync<TIntegrationEventReply>()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            //this.Logger.LogTrace("Starting RabbitMQ basic consume reply");
            try
            {
                if (this._consumerChannelReply is null)
                {
                    this.Logger.LogWarning("ConsumerChannelReply is null");
                    return;
                }

                if (this._consumerChannelReply.Value != null)
                {
                    var consumer = new AsyncEventingBasicConsumer(await this._consumerChannelReply);

                    consumer.ReceivedAsync += this.ConsumerReceivedReply<TIntegrationEventReply>;

                    await (await this._consumerChannelReply).BasicConsumeAsync(
                        queue: this._queueNameReply,
                        autoAck: false,
                        consumer: consumer).ConfigureAwait(false);
                }
                else
                {
                    this.Logger.LogError("StartBasicConsumeReply can't call on _consumerChannelReply is null");
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "StartBasicConsumeReply: ");
            }
        }

        protected async Task ConsumerReceivedServerAsync<TIntegrationEvent, TIntegrationEventReply>(object sender, BasicDeliverEventArgs eventArgs)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                try
                {
                    var props = eventArgs.BasicProperties;
                    if (this.ConsumerChannel != null)
                    {
                        //var replyProps = (await this.ConsumerChannel).CreateBasicProperties();
                        //if (replyProps != null)
                        //{

                        var replyProps = new BasicProperties
                        {
                            DeliveryMode = DeliveryModes.Transient,
                            CorrelationId = props.CorrelationId
                        };

                        //replyProps.CorrelationId = props.CorrelationId;

                        var response = await this.ProcessEventRpcAsync<TIntegrationEvent, TIntegrationEventReply>(eventArgs.RoutingKey, eventName, eventArgs.Body)
                            .ConfigureAwait(false);

                        if (response != null)
                        {
                            var ms = new MemoryStream();
                            Serializer.Serialize(ms, response);
                            var body = ms.ToArray();
                            if (!String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters
                                    .ExchangeName))
                            {
                                await (await this.ConsumerChannel).BasicPublishAsync(
                                    this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                    response.RoutingKey, true, replyProps, body).ConfigureAwait(false);
                            }
                        }
                        //}
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "CreateConsumerChannel RPC Received: ");
                }
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

        private async Task ConsumerReceivedReply<TIntegrationEventReply>(object sender, BasicDeliverEventArgs eventArgs)
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                if (!eventArgs.BasicProperties.CorrelationId.Equals(this._correlationId))
                {
                    return;
                }

                await this.ProcessEventReplyAsync<TIntegrationEventReply>(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "ConsumerReceivedReply: {0}", eventName);
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

        protected async ValueTask<IChannel> CreateConsumerChannelServerAsync<TIntegrationEvent, TIntegrationEventReply>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
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
                    this.Logger.LogError(ea.Exception, "CallbackException: ");
                    this.ConsumerChannel.Value.Dispose();
                    this.ConsumerChannel = new AsyncLazy<IChannel>(async () => await this.CreateConsumerChannelServerAsync<TIntegrationEvent, TIntegrationEventReply>(cancel).ConfigureAwait(false));
                    await this.StartBasicConsumeServer<TIntegrationEvent, TIntegrationEventReply>().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        private async ValueTask<IChannel> CreateConsumerChannelReplyAsync<TIntegrationEventReply>(CancellationToken cancel = default)
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if(!connectionResult) { return null; }
            }

            var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false);
            if (channel != null)
            {
                //this.QueueInitialize(channel);
                await channel.BasicQosAsync(0, 1, false).ConfigureAwait(false);

                channel.CallbackExceptionAsync += async (sender, ea) =>
                {
                    this.Logger.LogError(ea.Exception, "CallbackException Rpc: ");
                    this._consumerChannelReply.Value.Dispose();
                    this._consumerChannelReply =
                        new AsyncLazy<IChannel>(async () => await this.CreateConsumerChannelReplyAsync<TIntegrationEventReply>(cancel).ConfigureAwait(false));
                    await this.StartBasicConsumeReplyAsync<TIntegrationEventReply>().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        private async ValueTask ProcessEventReplyAsync<TIntegrationEventReply>(string routingKey, string eventName, ReadOnlyMemory<byte> message,
            CancellationToken cancel = default)
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            if (this.SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = this.SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    this.Logger.LogError("ProcessEventReply subscriptions no items! " + routingKey);
                }

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Rpc:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger.LogError("ProcessEventReply _eventHandler is null!");
                                }
                                else
                                {

                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger.LogError("ProcessEventReply: eventType is null! " + routingKey);
                                        return;
                                    }

                                    var eventResultType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        this.Logger.LogError("ProcessEventReply: eventResultType is null! " + routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        //var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEventReply>(ms);
                                        var concreteType =
                                            typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType, eventResultType);
                                        await ((ValueTask)concreteType.GetMethod("HandleReply")
                                                .Invoke(this.EventHandler, new[] {(object)integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, "ProcessEventReply: ");
                            }

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                this.Logger.LogError("ProcessEventReply HasSubscriptionsForEventReply {0} No Subscriptions!", routingKey);
            }
        }

        //private async ValueTask<dynamic> ProcessEventRpcAsync(string routingKey, string eventName,
        //    ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        private async ValueTask<TIntegrationEventReply> ProcessEventRpcAsync<TIntegrationEvent, TIntegrationEventReply>(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            //dynamic output = null;
            TIntegrationEventReply output = default;

            if (this.SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                var subscriptions = this.SubsManager.GetHandlersForEvent(routingKey);

                if (!subscriptions.Any())
                {
                    this.Logger.LogError("ProcessEventRpc subscriptions no items! {0}", routingKey);
                }

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Rpc:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger.LogError("ProcessEventRpc _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger.LogError("ProcessEventRpc: eventType is null! {0}", routingKey);
                                        return default;
                                    }

                                    var eventReplyType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        this.Logger.LogError("ProcessEventRpc: eventReplyType is null! {0}", routingKey);
                                        return default;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        //var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEvent>(ms);
                                        var concreteType =
                                            typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType, eventReplyType);

                                        //output = await ((dynamic)concreteType.GetMethod("HandleRpcAsync")
                                        //        .Invoke(this.EventHandler, new[] {(object)integrationEvent, cancel}))
                                        //    .ConfigureAwait(false);

                                        var generic = concreteType.GetMethod("HandleRpcAsync");

                                        var result = (ValueTask<TIntegrationEventReply>)generic.Invoke(this.EventHandler,
                                            new[] { (object)integrationEvent, cancel });

                                        output = await result.ConfigureAwait(false);

                                        //if (output is null)
                                        //{
                                        //    this.Logger.LogError("ProcessEventRpc output is null!");
                                        //}
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, "ProcessEventRpcAsync: ");
                            }

                            break;

                        case SubscriptionManagerType.Dynamic:
                            break;
                        case SubscriptionManagerType.Typed:
                            break;
                        case SubscriptionManagerType.Queue:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                this.Logger.LogError("ProcessEventRpc HasSubscriptionsForEvent {0} No Subscriptions!", routingKey);
            }

            return output;
        } //ProcessEventRpc.
    }
}
