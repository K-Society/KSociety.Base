// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using EventBus;
    using EventBus.Abstractions;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;
    using InfraSub.Shared.Class;
    using Microsoft.Extensions.Logging;
    using ProtoBuf;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class EventBusRabbitMqRpcServer : EventBusRabbitMq, IEventBusRpcServer
    {
        private AsyncLazy<IModel?>? _consumerChannelReply;
        private string? _queueNameReply;

        #region [Constructor]

        public EventBusRabbitMqRpcServer(IRabbitMqPersistentConnection persistentConnection,
            ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager? subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {

        }

        public EventBusRabbitMqRpcServer(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager? subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null, ILogger<EventBusRabbitMq>? logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {

        }

        #endregion

        public override void Initialize(CancellationToken cancel = default)
        {
            //this.Logger?.LogTrace("EventBusRabbitMqRpcServer Initialize.");
            this.SubsManager.OnEventReplyRemoved += this.SubsManager_OnEventReplyRemoved;
            this.ConsumerChannel =
                new AsyncLazy<IModel?>(async () => await this.CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
            this._queueNameReply = this.QueueName + "_Reply";

            this._consumerChannelReply =
                new AsyncLazy<IModel?>(async () => await this.CreateConsumerChannelReplyAsync(cancel).ConfigureAwait(false));
        }

        public IIntegrationRpcServerHandler<T, TR>? GetIntegrationRpcServerHandler<T, TR>()
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
        {
            if (this.EventHandler is IIntegrationRpcServerHandler<T, TR> queue)
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
                if (channel != null)
                {
                    if (!String.IsNullOrEmpty(this.QueueName) &&
                        !String.IsNullOrEmpty(this._queueNameReply) &&
                        !String.IsNullOrEmpty(this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName))
                    {
                        channel.QueueUnbind(this.QueueName,
                            this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName,
                            eventName);

                        channel.QueueUnbind(this._queueNameReply,
                            this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName,
                            eventName);
                    }
                }
            }

            if (this.SubsManager is null)
            {
                return;
            }

            if (!this.SubsManager.IsReplyEmpty)
            {
                return;
            }

            this.QueueName = String.Empty;
            if (this.ConsumerChannel != null)
            {
                (await this.ConsumerChannel)?.Close();
            }

            //ToDo

            this._queueNameReply = String.Empty;
            if (this._consumerChannelReply != null)
            {
                (await this._consumerChannelReply)?.Close();
            }
        }

        protected override void QueueInitialize(IModel channel)
        {
            try
            {
                if (this.EventBusParameters != null)
                {


                    channel?.ExchangeDeclare(this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName,
                        this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeType,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

                    //var args = new Dictionary<string, object>
                    //{
                    //    { "x-dead-letter-exchange", EventBusParameters.ExchangeDeclareParameters.ExchangeName }
                    //};

                    channel?.QueueDeclare(this._queueNameReply,
                        this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger?.LogError(rex, "EventBusRabbitMqRpcServer RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EventBusRabbitMqRpcServer QueueInitialize: ");
            }
        }

        #region [Subscribe]

        public async ValueTask SubscribeRpcServer<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>
        {
            var eventName = this.SubsManager?.GetEventKey<T>();
            var eventNameResult = this.SubsManager?.GetEventReplyKey<TR>();
            //this.Logger?.LogTrace("SubscribeRpcServer routing key: {0}, event name: {1}, event name result: {2}", routingKey, eventName, eventNameResult);
            await this.DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey)
                .ConfigureAwait(false);
            this.SubsManager?.AddSubscriptionRpcServer<T, TR, TH>(eventName + "." + routingKey,
                eventNameResult + "." + routingKey);
            await this.StartBasicConsume().ConfigureAwait(false);
        }

        private async ValueTask DoInternalSubscriptionRpc(string eventName, string eventNameResult)
        {
            try
            {
                var containsKey = this.SubsManager?.HasSubscriptionsForEvent(eventName);
                if (containsKey.HasValue && containsKey.Value)
                {
                    return;
                }

                if (!this.PersistentConnection.IsConnected)
                {
                    var connection = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                }

                using (var channel = this.PersistentConnection.CreateModel())
                {
                    if (channel != null)
                    {
                        this.QueueInitialize(channel);

                        if (this.EventBusParameters != null && this.EventBusParameters.ExchangeDeclareParameters != null)
                        {
                            channel.QueueBind(this.QueueName, this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
                            channel.QueueBind(this._queueNameReply, this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventNameResult);
                        }
                    }
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger?.LogError(rex, "EventBusRabbitMqRpcClient RabbitMQClientException DoInternalSubscriptionRpc: ");
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "EventBusRabbitMqRpcServer DoInternalSubscriptionRpc: ");
            }
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeRpcServer<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TH : IIntegrationRpcServerHandler<T, TR>
            where TR : IIntegrationEventReply
        {
            this.SubsManager?.RemoveSubscriptionRpcServer<T, TR, TH>(routingKey);
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
            //this.Logger?.LogTrace("EventBusRabbitMqRpcServer Starting RabbitMQ basic consume.");

            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger?.LogWarning("EventBusRabbitMqRpcServer ConsumerChannel is null!");
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
                    //this.Logger?.LogInformation("EventBusRabbitMqRpcServer StartBasicConsume done. Queue name: {0}, autoAck: {1}", this.QueueName, false);

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

        protected override void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                var props = eventArgs.BasicProperties;
                var replyProps = this.ConsumerChannel?.Value.Result?.CreateBasicProperties();
                if (replyProps != null)
                {
                    replyProps.CorrelationId = props.CorrelationId;

                    var response = this.ProcessEventRpc(eventArgs.RoutingKey, eventName, eventArgs.Body);

                    if (response != null)
                    {
                        var ms = new MemoryStream();
                        Serializer.Serialize<IIntegrationEventReply>(ms, response);
                        var body = ms.ToArray();
                        if (!String.IsNullOrEmpty(this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName))
                        {
                            this._consumerChannelReply?.Value.Result.BasicPublish(
                                this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName,
                                (string)response.RoutingKey,
                                replyProps,
                                body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "CreateConsumerChannel RPC Received: ");
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

        protected override async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                var props = eventArgs.BasicProperties;
                if (this.ConsumerChannel != null)
                {
                    var replyProps = (await this.ConsumerChannel)?.CreateBasicProperties();
                    if (replyProps != null)
                    {
                        replyProps.CorrelationId = props.CorrelationId;

                        var response = await this.ProcessEventRpcAsync(eventArgs.RoutingKey, eventName, eventArgs.Body)
                            .ConfigureAwait(false);

                        if (response != null)
                        {
                            var ms = new MemoryStream();
                            Serializer.Serialize<IIntegrationEventReply>(ms, response);

                            var body = ms.ToArray();
                            if (this._consumerChannelReply != null &&
                                !String.IsNullOrEmpty(this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName))
                            {
                                (await this._consumerChannelReply).BasicPublish(
                                    this.EventBusParameters?.ExchangeDeclareParameters?.ExchangeName,
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

        protected override async ValueTask<IModel?> CreateConsumerChannelAsync(CancellationToken cancel = default)
        {
            //this.Logger?.LogTrace("EventBusRabbitMqRpcServer CreateConsumerChannelAsync queue name: {0}", this.QueueName);
            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            var channel = this.PersistentConnection.CreateModel();

            if (channel != null)
            {
                try
                {
                    this.QueueInitialize(channel);
                    channel.BasicQos(0, 1, false);
                }
                catch (Exception ex)
                {
                    this.Logger?.LogError(ex, "CreateConsumerChannelAsync: ");
                }

                channel.CallbackException += async (sender, ea) =>
                {
                    this.Logger?.LogError(ea.Exception, "CallbackException: ");
                    this.ConsumerChannel?.Value.Dispose();
                    this.ConsumerChannel = new AsyncLazy<IModel?>(async () => await this.CreateConsumerChannelAsync(cancel));
                    await this.StartBasicConsume().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        private async ValueTask<IModel?> CreateConsumerChannelReplyAsync(CancellationToken cancel = default)
        {
            //this.Logger?.LogTrace("CreateConsumerChannelReplyAsync queue name: {0}", this._queueNameReply);
            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            var channel = this.PersistentConnection.CreateModel();

            if (channel != null)
            {
                try
                {
                    this.QueueInitialize(channel);
                    channel.BasicQos(0, 1, false);
                }
                catch (Exception ex)
                {
                    this.Logger?.LogError(ex, "CreateConsumerChannelReplyAsync: ");
                }

                channel.CallbackException += (sender, ea) =>
                {
                    this.Logger?.LogError(ea.Exception, "CallbackException Rpc: ");
                    this._consumerChannelReply?.Value.Dispose();
                    this._consumerChannelReply =
                        new AsyncLazy<IModel?>(async () =>
                            await this.CreateConsumerChannelReplyAsync(cancel).ConfigureAwait(false));
                };

                return channel;
            }

            return null;
        }

        private dynamic? ProcessEventRpc(string routingKey, string eventName, ReadOnlyMemory<byte> message,
            CancellationToken cancel = default)
        {
            dynamic? output = null;

            if (this.SubsManager is null)
            {
                return null;
            }

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
                            break;
                        case SubscriptionManagerType.RpcClient:
                            break;
                        case SubscriptionManagerType.RpcServer:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger?.LogError("ProcessEventRpcServer _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventRpcServer: eventType is null! {0}", routingKey);
                                        return null;
                                    }

                                    var eventReplyType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventRpcServer: eventReplyType is null! {0}",
                                            routingKey);
                                        return null;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType =
                                            typeof(IIntegrationRpcServerHandler<,>).MakeGenericType(eventType,
                                                eventReplyType);

                                        //output = (dynamic)concreteType.GetMethod("HandleRpc")
                                        //    .Invoke(EventHandler, new[] { integrationEvent, cancel });

                                        output = concreteType.GetMethod("HandleRpc")
                                            .Invoke(this.EventHandler, new[] {integrationEvent, cancel});

                                        if (output is null)
                                        {
                                            this.Logger?.LogError("ProcessEventRpcServer output is null!");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger?.LogError(ex, "ProcessEventRpcServer: ");
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

        private async ValueTask<dynamic?> ProcessEventRpcAsync(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            dynamic? output = null;

            if (this.SubsManager is null)
            {
                return null;
            }

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
                            break;
                        case SubscriptionManagerType.RpcClient:
                            break;
                        case SubscriptionManagerType.RpcServer:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger?.LogError("ProcessEventRpcServer _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventRpcServer: eventType is null! {0}",
                                            routingKey);
                                        return null;
                                    }

                                    var eventReplyType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        this.Logger?.LogError("ProcessEventRpcServer: eventReplyType is null! {0}",
                                            routingKey);
                                        return null;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);

                                        var concreteType =
                                            typeof(IIntegrationRpcServerHandler<,>).MakeGenericType(eventType,
                                                eventReplyType);
                                        output = await ((dynamic)concreteType.GetMethod("HandleRpcAsync")
                                                .Invoke(this.EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);

                                        if (output is null)
                                        {
                                            this.Logger?.LogError("ProcessEventRpcServer output is null!");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger?.LogError(ex, "ProcessEventRpcServer: ");
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
        } //ProcessEventRpcAsync.
    }
}
