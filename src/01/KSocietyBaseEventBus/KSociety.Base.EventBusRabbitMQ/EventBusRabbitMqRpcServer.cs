// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using EventBus;
    using EventBus.Abstractions;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;
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
    using Helper;

    public sealed class EventBusRabbitMqRpcServer : EventBusRabbitMq, IEventBusRpcServer
    {
        private AsyncLazy<IModel> _consumerChannelReply;
        private string _queueNameReply;

        #region [Constructor]

        public EventBusRabbitMqRpcServer(IRabbitMqPersistentConnection persistentConnection,
            ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {

        }

        public EventBusRabbitMqRpcServer(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {

        }

        #endregion

        public void InitializeServer<TIntegrationEvent, TIntegrationEventReply>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            //this.Logger.LogTrace("EventBusRabbitMqRpcServer Initialize.");
            if (this.SubsManager != null)
            {
                this.SubsManager.OnEventReplyRemoved += this.SubsManager_OnEventReplyRemoved;
            }

            this.ConsumerChannel =
                new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelServerAsync<TIntegrationEvent, TIntegrationEventReply>(cancel).ConfigureAwait(false));
            this._queueNameReply = this.QueueName + "_Reply";

            this._consumerChannelReply =
                new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelReplyAsync(cancel).ConfigureAwait(false));
        }

        public IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply> GetIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>()
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            if (this.EventHandler is IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply> queue)
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
                        !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                    {
                        channel.QueueUnbind(this.QueueName,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                            eventName);

                        channel.QueueUnbind(this._queueNameReply,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
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
                (await this.ConsumerChannel).Close();
            }

            //ToDo

            this._queueNameReply = String.Empty;
            if (this._consumerChannelReply != null)
            {
                (await this._consumerChannelReply).Close();
            }
        }

        protected override void QueueInitialize(IModel channel)
        {
            try
            {
                if (this.EventBusParameters != null)
                {
                    channel.ExchangeDeclare(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

                    //var args = new Dictionary<string, object>
                    //{
                    //    { "x-dead-letter-exchange", EventBusParameters.ExchangeDeclareParameters.ExchangeName }
                    //};

                    channel.QueueDeclare(this.QueueName, this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);

                    //channel.QueueDeclare(this._queueNameReply,
                    //    this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                    //    this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                    //    this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger.LogError(rex, "EventBusRabbitMqRpcServer RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMqRpcServer QueueInitialize: ");
            }
        }

        //private void QueueInitializeReply(IModel channel)
        //{
        //    try
        //    {
        //        if (this.EventBusParameters != null)
        //        {
        //            channel.ExchangeDeclare(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
        //                this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
        //                this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
        //                this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

        //            //var args = new Dictionary<string, object>
        //            //{
        //            //    { "x-dead-letter-exchange", EventBusParameters.ExchangeDeclareParameters.ExchangeName }
        //            //};

        //            //channel.QueueDeclare(this.QueueName, this.EventBusParameters.QueueDeclareParameters.QueueDurable,
        //            //    this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
        //            //    this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);

        //            channel.QueueDeclare(this._queueNameReply,
        //                this.EventBusParameters.QueueDeclareParameters.QueueDurable,
        //                this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
        //                this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
        //        }
        //    }
        //    catch (RabbitMQClientException rex)
        //    {
        //        this.Logger.LogError(rex, "EventBusRabbitMqRpcServer RabbitMQClientException QueueInitialize: ");
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Logger.LogError(ex, "EventBusRabbitMqRpcServer QueueInitialize: ");
        //    }
        //}

        #region [Subscribe]

        public async ValueTask SubscribeRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
        {
            var eventName = this.SubsManager.GetEventKey<TIntegrationEventRpc>();
            var eventNameResult = this.SubsManager.GetEventReplyKey<TIntegrationEventReply>();
            //this.Logger.LogTrace("SubscribeRpcServer routing key: {0}, event name: {1}, event name result: {2}", routingKey, eventName, eventNameResult);
            await this.DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey)
                .ConfigureAwait(false);
            this.SubsManager.AddSubscriptionRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(eventName + "." + routingKey, eventNameResult + "." + routingKey);
            await this.StartBasicConsumeServer<TIntegrationEventRpc, TIntegrationEventReply>().ConfigureAwait(false);
        }

        private async ValueTask DoInternalSubscriptionRpc(string eventName, string eventNameResult)
        {
            try
            {
                //var containsKey = this.SubsManager.HasSubscriptionsForEvent(eventName);
                if (this.SubsManager.HasSubscriptionsForEvent(eventName))
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
                        //this.QueueInitializeReply(channel);

                        //if (this.EventBusParameters != null && this.EventBusParameters.ExchangeDeclareParameters != null)
                        //{
                        //    channel.QueueBind(this.QueueName, this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
                        //    channel.QueueBind(this._queueNameReply, this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventNameResult);
                        //}

                        if (!String.IsNullOrEmpty(this.QueueName) &&
                            //!String.IsNullOrEmpty(this._queueNameReply) &&
                            !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                        {
                            channel.QueueBind(this.QueueName,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
                            //channel.QueueBind(this._queueNameReply,
                            //    this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                            //    eventNameResult);
                        }
                    }
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger.LogError(rex, "EventBusRabbitMqRpcServer RabbitMQClientException DoInternalSubscriptionRpc: ");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMqRpcServer DoInternalSubscriptionRpc: ");
            }
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
        {
            this.SubsManager.RemoveSubscriptionRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(routingKey);
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

        protected async ValueTask<bool> StartBasicConsumeServer<TIntegrationEvent, TIntegrationEventReply>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            //this.Logger.LogTrace("EventBusRabbitMqRpcServer Starting RabbitMQ basic consume.");

            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpcServer ConsumerChannel is null!");
                    return false;
                }

                if (this.ConsumerChannel.Value != null)
                {
                    var consumer = new AsyncEventingBasicConsumer(await this.ConsumerChannel);

                    consumer.Received += this.ConsumerReceivedServerAsync<TIntegrationEvent, TIntegrationEventReply>;

                    (await this.ConsumerChannel).BasicConsume(
                        queue: this.QueueName,
                        autoAck: false,
                        consumer: consumer);
                    //this.Logger.LogInformation("EventBusRabbitMqRpcServer StartBasicConsume done. Queue name: {0}, autoAck: {1}", this.QueueName, false);

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

        protected void ConsumerReceivedServer<TIntegrationEvent, TIntegrationEventReply>(object sender, BasicDeliverEventArgs eventArgs)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                var props = eventArgs.BasicProperties;
                var replyProps = this.ConsumerChannel.Value.Result.CreateBasicProperties();
                if (replyProps != null)
                {
                    replyProps.CorrelationId = props.CorrelationId;

                    var response = this.ProcessEventRpc<TIntegrationEvent, TIntegrationEventReply>(eventArgs.RoutingKey, eventName, eventArgs.Body);

                    if (response != null)
                    {
                        var ms = new MemoryStream();
                        Serializer.Serialize(ms, response);
                        var body = ms.ToArray();
                        if (!String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                        {
                            this._consumerChannelReply.Value.Result.BasicPublish(
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                response.RoutingKey,
                                replyProps,
                                body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannel RPC Received: ");
            }


            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                this.ConsumerChannel.Value.Result.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
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
                var props = eventArgs.BasicProperties;
                if (this.ConsumerChannel != null)
                {
                    var replyProps = (await this.ConsumerChannel).CreateBasicProperties();
                    if (replyProps != null)
                    {
                        replyProps.CorrelationId = props.CorrelationId;

                        var response = await this.ProcessEventRpcAsync<TIntegrationEvent, TIntegrationEventReply>(eventArgs.RoutingKey, eventName, eventArgs.Body)
                            .ConfigureAwait(false);

                        if (response != null)
                        {
                            var ms = new MemoryStream();
                            //Serializer.Serialize<IIntegrationEventReply>(ms, response);
                            Serializer.Serialize(ms, response);

                            var body = ms.ToArray();
                            if (this._consumerChannelReply != null &&
                                !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                            {
                                (await this._consumerChannelReply).BasicPublish(
                                    this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                    response.RoutingKey, replyProps, body);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannel RPC Received: ");
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                if (this.ConsumerChannel != null)
                {
                    (await this.ConsumerChannel).BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
            }
        }

        protected async ValueTask<IModel> CreateConsumerChannelServerAsync<TIntegrationEvent, TIntegrationEventReply>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            //this.Logger.LogTrace("EventBusRabbitMqRpcServer CreateConsumerChannelAsync queue name: {0}", this.QueueName);
            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            var channel = this.PersistentConnection.CreateModel();

            if (channel != null)
            {
                try
                {
                    //this.QueueInitialize(channel);
                    channel.BasicQos(0, 1, false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "CreateConsumerChannelAsync: ");
                }

                channel.CallbackException += async (sender, ea) =>
                {
                    this.Logger.LogError(ea.Exception, "CallbackException: ");
                    this.ConsumerChannel.Value.Dispose();
                    this.ConsumerChannel = new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelServerAsync<TIntegrationEvent, TIntegrationEventReply>(cancel).ConfigureAwait(false));
                    await this.StartBasicConsumeServer<TIntegrationEvent, TIntegrationEventReply>().ConfigureAwait(false);
                };

                return channel;
            }

            return null;
        }

        private async ValueTask<IModel> CreateConsumerChannelReplyAsync(CancellationToken cancel = default)
        {
            //this.Logger.LogTrace("CreateConsumerChannelReplyAsync queue name: {0}", this._queueNameReply);
            if (!this.PersistentConnection.IsConnected)
            {
                await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            var channel = this.PersistentConnection.CreateModel();

            if (channel != null)
            {
                try
                {
                    //this.QueueInitializeReply(channel);
                    channel.BasicQos(0, 1, false);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "CreateConsumerChannelReplyAsync: ");
                }

                channel.CallbackException += (sender, ea) =>
                {
                    this.Logger.LogError(ea.Exception, "CallbackException Rpc: ");
                    this._consumerChannelReply.Value.Dispose();
                    this._consumerChannelReply =
                        new AsyncLazy<IModel>(async () =>
                            await this.CreateConsumerChannelReplyAsync(cancel).ConfigureAwait(false));
                };

                return channel;
            }

            return null;
        }

        //private dynamic ProcessEventRpc(string routingKey, string eventName, ReadOnlyMemory<byte> message,
        //    CancellationToken cancel = default)
        private TIntegrationEventReply ProcessEventRpc<TIntegrationEvent, TIntegrationEventReply>(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            TIntegrationEventReply output = default;

            if (this.SubsManager is null)
            {
                return default;
            }

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
                            break;
                        case SubscriptionManagerType.RpcClient:
                            break;
                        case SubscriptionManagerType.RpcServer:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger.LogError("ProcessEventRpcServer _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger.LogError("ProcessEventRpcServer: eventType is null! {0}", routingKey);
                                        return default;
                                    }

                                    var eventReplyType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        this.Logger.LogError("ProcessEventRpcServer: eventReplyType is null! {0}",
                                            routingKey);
                                        return default;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        //var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEvent>(ms);
                                        var concreteType =
                                            typeof(IIntegrationRpcServerHandler<,>).MakeGenericType(eventType,
                                                eventReplyType);

                                        //output = (dynamic)concreteType.GetMethod("HandleRpc")
                                        //    .Invoke(EventHandler, new[] { integrationEvent, cancel });

                                        output = (TIntegrationEventReply)concreteType.GetMethod("HandleRpc")
                                            .Invoke(this.EventHandler, new[] {(object)integrationEvent, cancel});

                                        //if (output is null)
                                        //{
                                        //    this.Logger.LogError("ProcessEventRpcServer output is null!");
                                        //}
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, "ProcessEventRpcServer: ");
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
        //where TR : IIntegrationEventReply
        //private async ValueTask<dynamic> ProcessEventRpcAsync(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        private async ValueTask<TIntegrationEventReply> ProcessEventRpcAsync<TIntegrationEvent, TIntegrationEventReply>(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            TIntegrationEventReply output = default;

            if (this.SubsManager is null)
            {
                return default;
            }

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
                            break;
                        case SubscriptionManagerType.RpcClient:
                            break;
                        case SubscriptionManagerType.RpcServer:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger.LogError("ProcessEventRpcServer _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        this.Logger.LogError("ProcessEventRpcServer: eventType is null! {0}",
                                            routingKey);
                                        return default;
                                    }

                                    var eventReplyType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        this.Logger.LogError("ProcessEventRpcServer: eventReplyType is null! {0}",
                                            routingKey);
                                        return default;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        //var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEvent>(ms);

                                        var concreteType =
                                            typeof(IIntegrationRpcServerHandler<,>).MakeGenericType(eventType, eventReplyType);
                                        //output = await ((dynamic)concreteType.GetMethod("HandleRpcAsync")
                                        //        .Invoke(this.EventHandler, new[] { integrationEvent, cancel }))
                                        //    .ConfigureAwait(false);

                                        //output = await concreteType.GetMethod("HandleRpcAsync")
                                        //    ?.Invoke(this.EventHandler, new[] {integrationEvent, cancel});
                                        //.ConfigureAwait(false);

                                        //ValueTask<TIntegrationEventReply>

                                        var generic = concreteType.GetMethod("HandleRpcAsync");

                                        var result = (ValueTask<TIntegrationEventReply>)generic.Invoke(this.EventHandler,
                                            new[] {(object)integrationEvent, cancel});

                                        output = await result.ConfigureAwait(false);

                                        //if (output is null)
                                        //{
                                        //    this.Logger.LogError("ProcessEventRpcServer output is null!");
                                        //}
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, "ProcessEventRpcServerAsync: ");
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
        } //ProcessEventRpcAsync.
    }
}
