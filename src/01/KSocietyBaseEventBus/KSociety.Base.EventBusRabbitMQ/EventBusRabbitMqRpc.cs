// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using EventBus;
    using EventBus.Abstractions;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;
    using InfraSub.Shared.Class;
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

    public sealed class EventBusRabbitMqRpc : EventBusRabbitMq, IEventBusRpc
    {
        private AsyncLazy<IModel>? _consumerChannelReply;
        private string? _queueNameReply;

        private readonly string _correlationId;

        #region [Constructor]

        public EventBusRabbitMqRpc(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {
            this._correlationId = Guid.NewGuid().ToString();
        }

        public EventBusRabbitMqRpc(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null, ILogger<EventBusRabbitMq>? logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {
            this._correlationId = Guid.NewGuid().ToString();
        }

        #endregion

        public override void Initialize(CancellationToken cancel = default)
        {
            //this.Logger?.LogTrace("EventBusRabbitMqRpc Initialize.");
            this.SubsManager.OnEventReplyRemoved += this.SubsManager_OnEventReplyRemoved;
            this.ConsumerChannel =
                new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
            this._queueNameReply = this.QueueName + "_Reply";
            this._consumerChannelReply =
                new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelReplyAsync(cancel).ConfigureAwait(false));
        }

        public IIntegrationRpcHandler<T, TR>? GetIntegrationRpcHandler<T, TR>()
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
        {
            if (this.EventHandler is IIntegrationRpcHandler<T, TR> queue)
            {
                return queue;
            }

            return null;
        }

        private async void SubsManager_OnEventReplyRemoved(object sender, string eventName)
        {
            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using (var channel = this.PersistentConnection.CreateModel())
            {
                if (!String.IsNullOrEmpty(this._queueNameReply) &&
                    !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters?.ExchangeName))
                {
                    channel.QueueUnbind(this._queueNameReply,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                        eventName);
                }
            }

            if (!this.SubsManager.IsReplyEmpty)
            {
                return;
            }

            this._queueNameReply = String.Empty;
            if (this._consumerChannelReply != null)
            {
                (await this._consumerChannelReply)?.Close();
            }
        }

        public override async ValueTask Publish(IIntegrationEvent @event)
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
                    this.Logger?.LogWarning(ex, "Publish:");
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
                                properties.CorrelationId = this._correlationId;
                                properties.ReplyTo = this._queueNameReply;
                                channel.BasicPublish(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                    routingKey,
                                    true,
                                    properties, body);
                            });
                        }
                    }
                }
            }
        }

        protected override void QueueInitialize(IModel channel)
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
                    //};

                    channel?.QueueDeclare(this.QueueName, this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger?.LogError(rex, "EventBusRabbitMqRpc RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EventBusRabbitMqRpc QueueInitialize: ");
            }
        }

        #region [Subscribe]

        public async ValueTask SubscribeRpc<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>
        {
            var eventName = this.SubsManager.GetEventKey<T>();
            var eventNameResult = this.SubsManager.GetEventReplyKey<TR>();
            //this.Logger?.LogDebug("SubscribeRpc: eventName: {0}.{1} eventNameResult: {2}.{3}", eventName, routingKey, eventNameResult, routingKey);
            await this.DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey);
            this.SubsManager.AddSubscriptionRpc<T, TR, TH>(eventName + "." + routingKey, eventNameResult + "." + routingKey);
            await this.StartBasicConsume().ConfigureAwait(false);
            await this.StartBasicConsumeReply().ConfigureAwait(false);
        }

        private async ValueTask DoInternalSubscriptionRpc(string eventName, string eventNameResult)
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
                        !String.IsNullOrEmpty(this._queueNameReply) &&
                        !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters?.ExchangeName))
                    {
                        channel.QueueBind(this.QueueName,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
                        channel.QueueBind(this._queueNameReply,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                            eventNameResult);
                    }
                }
            }
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeRpc<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationRpcHandler<T, TR>
            where TR : IIntegrationEventReply
        {
            this.SubsManager.RemoveSubscriptionRpc<T, TR, TH>(routingKey);
        }

        #endregion

        protected override void DisposeManagedResources()
        {
            this._consumerChannelReply?.Value.Dispose();
            this.ConsumerChannel?.Value.Dispose();
            this.SubsManager?.Clear();
            this.SubsManager?.ClearReply();
        }

        protected override async ValueTask<bool> StartBasicConsume()
        {
            //this.Logger?.LogTrace("Starting RabbitMQ basic consume");
            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger?.LogWarning("ConsumerChannel is null");
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

                    return true;
                }

                this.Logger?.LogError("StartBasicConsume can't call on ConsumerChannel == null");
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "StartBasicConsume: ");
            }

            return false;
        }

        private async ValueTask StartBasicConsumeReply()
        {
            //this.Logger?.LogTrace("Starting RabbitMQ basic consume reply");
            try
            {
                if (this._consumerChannelReply is null)
                {
                    this.Logger?.LogWarning("ConsumerChannelReply is null");
                    return;
                }

                if (this._consumerChannelReply?.Value != null)
                {
                    var consumer = new AsyncEventingBasicConsumer(await this._consumerChannelReply);

                    consumer.Received += this.ConsumerReceivedReply;

                    (await this._consumerChannelReply).BasicConsume(
                        queue: this._queueNameReply,
                        autoAck: false,
                        consumer: consumer);
                }
                else
                {
                    this.Logger?.LogError("StartBasicConsumeReply can't call on _consumerChannelReply is null");
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "StartBasicConsumeReply: ");
            }
        }

        protected override async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                try
                {
                    var props = eventArgs.BasicProperties;
                    if (this.ConsumerChannel != null)
                    {
                        var replyProps = (await this.ConsumerChannel)?.CreateBasicProperties();
                        if (replyProps != null)
                        {
                            replyProps.CorrelationId = props.CorrelationId;

                            var response = await this.ProcessEventRpc(eventArgs.RoutingKey, eventName, eventArgs.Body)
                                .ConfigureAwait(false);

                            if (response != null)
                            {
                                var ms = new MemoryStream();
                                Serializer.Serialize<IIntegrationEventReply>(ms, response);
                                var body = ms.ToArray();
                                if (!String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters
                                        ?.ExchangeName))
                                {
                                    (await this.ConsumerChannel)?.BasicPublish(
                                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                        (string)response.RoutingKey, replyProps, body);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Logger?.LogError(ex, "CreateConsumerChannel RPC Received: ");
                }
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

        private async Task ConsumerReceivedReply(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                if (!eventArgs.BasicProperties.CorrelationId.Equals(this._correlationId))
                {
                    return;
                }

                await this.ProcessEventReply(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger?.LogWarning(ex, "ConsumerReceivedReply: {0}", eventName);
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

        protected override async ValueTask<IModel> CreateConsumerChannelAsync(CancellationToken cancel = default)
        {
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
                    this.Logger?.LogError(ea.Exception, "CallbackException: ");
                    this.ConsumerChannel?.Value.Dispose();
                    this.ConsumerChannel = new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
                    await this.StartBasicConsume().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        private async ValueTask<IModel> CreateConsumerChannelReplyAsync(CancellationToken cancel = default)
        {
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
                    this.Logger?.LogError(ea.Exception, "CallbackException Rpc: ");
                    this._consumerChannelReply?.Value.Dispose();
                    this._consumerChannelReply =
                        new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelReplyAsync(cancel).ConfigureAwait(false));
                    await this.StartBasicConsumeReply().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        private async ValueTask ProcessEventReply(string routingKey, string eventName, ReadOnlyMemory<byte> message,
            CancellationToken cancel = default)
        {
            if (this.SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = this.SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    this.Logger?.LogError("ProcessEventReply subscriptions no items! " + routingKey);
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
                                    this.Logger?.LogError("ProcessEventReply _eventHandler is null!");
                                }
                                else
                                {

                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventReply: eventType is null! " + routingKey);
                                        return;
                                    }

                                    var eventResultType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventReply: eventResultType is null! " + routingKey);
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
                                this.Logger?.LogError(ex, "ProcessEventReply: ");
                            }

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                this.Logger?.LogError("ProcessEventReply HasSubscriptionsForEventReply {0} No Subscriptions!", routingKey);
            }
        }

        private async ValueTask<dynamic> ProcessEventRpc(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            dynamic output = null;

            if (this.SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                var subscriptions = this.SubsManager.GetHandlersForEvent(routingKey);

                if (!subscriptions.Any())
                {
                    this.Logger?.LogError("ProcessEventRpc subscriptions no items! {0}", routingKey);
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
                                    this.Logger?.LogError("ProcessEventRpc _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventRpc: eventType is null! {0}", routingKey);
                                        return null;
                                    }

                                    var eventReplyType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventRpc: eventReplyType is null! {0}", routingKey);
                                        return null;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType =
                                            typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType,
                                                eventReplyType);

                                        output = await ((dynamic)concreteType.GetMethod("HandleRpcAsync")
                                                .Invoke(this.EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);

                                        if (output is null)
                                        {
                                            this.Logger?.LogError("ProcessEventRpc output is null!");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger?.LogError(ex, "ProcessEventRpc: ");
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
                this.Logger?.LogError("ProcessEventRpc HasSubscriptionsForEvent {0} No Subscriptions!", routingKey);
            }

            return output;
        } //ProcessEventRpc.
    }
}
