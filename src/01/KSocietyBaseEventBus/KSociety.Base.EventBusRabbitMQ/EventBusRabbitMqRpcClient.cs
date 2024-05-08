// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using EventBus;
    using EventBus.Abstractions;
    using EventBus.Abstractions.Handler;
    using Helper;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using Microsoft.Extensions.Logging;
    using Polly;
    using ProtoBuf;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;

    public sealed class EventBusRabbitMqRpcClient<TIntegrationEventReply> : EventBusRabbitMq, IEventBusRpcClient<TIntegrationEventReply>
        where TIntegrationEventReply : IIntegrationEventReply, new()
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<TIntegrationEventReply>> _callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<TIntegrationEventReply>>();

        private string _queueNameReply;

        #region [Constructor]

        public EventBusRabbitMqRpcClient(IRabbitMqPersistentConnection persistentConnection,
            ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {

        }

        public EventBusRabbitMqRpcClient(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {

        }

        #endregion

        public void Initialize(bool asyncMode = true, CancellationToken cancel = default)
        {
            //this.Logger.LogTrace("EventBusRabbitMqRpcClient Initialize.");
            //this._callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<TIntegrationEventReply>>();

            this._queueNameReply = this.QueueName + "_Reply";
            if (this.SubsManager != null)
            {
                this.SubsManager.OnEventReplyRemoved += this.SubsManager_OnEventReplyRemoved;
            }

            if(asyncMode)
            {
                this.ConsumerChannel =
                new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
            }
            else
            {
                this.ConsumerChannel =
                new AsyncLazy<IModel>(() => this.CreateConsumerChannel());
            }
        }

        public IIntegrationRpcClientHandler<TIntegrationEventReply> GetIntegrationRpcClientHandler()
            //where TIntegrationEventReply : IIntegrationEventReply
        {
            if (this.EventHandler is IIntegrationRpcClientHandler<TIntegrationEventReply> queue)
            {
                return queue;
            }

            return null;
        }

        private async void SubsManager_OnEventReplyRemoved(object sender, string eventName)
        {
            if (this.PersistentConnection is null)
            {
                return;
            }

            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpcClient SubsManager_OnEventReplyRemoved: {0}!", "no connection");
                    return;
                }
            }

            using (var channel = this.PersistentConnection.CreateModel())
            {
                if (//!String.IsNullOrEmpty(this.QueueName) &&
                    !String.IsNullOrEmpty(this._queueNameReply) &&
                    !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                {
                    //channel.QueueUnbind(this.QueueName, this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                    //    eventName);
                    channel.QueueUnbind(this._queueNameReply,
                        this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName); //ToDo
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

            this._queueNameReply = String.Empty;
            this.QueueName = String.Empty;
            if (this.ConsumerChannel != null)
            {
                (await this.ConsumerChannel).Close();
            }
        }

        public override async ValueTask Publish(IIntegrationEvent @event)
        {
            if (this.PersistentConnection is null)
            {
                return;
            }

            if (!this.PersistentConnection.IsConnected)
            {
                var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                if (!connectionResult)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpcClient Publish: {0}!", "no connection");
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
            var correlationId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<TIntegrationEventReply>(TaskCreationOptions.RunContinuationsAsynchronously);
            this._callbackMapper.TryAdd(correlationId, tcs);

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
                                properties.CorrelationId = correlationId;
                                properties.ReplyTo = this._queueNameReply; //ToDo
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

        public async Task<TIntegrationEventReply> CallAsync<TIntegrationEventRpc>(TIntegrationEventRpc @event,
            CancellationToken cancellationToken = default)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
        {
            try
            {
                if (this.PersistentConnection is null)
                {
                    return default;
                }

                bool isConnected;
                if (this.PersistentConnection.IsConnected)
                {
                    isConnected = true;
                }
                else
                {
                    isConnected = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                }

                if (isConnected)
                {
                    var policy = Policy.Handle<BrokerUnreachableException>()
                        .Or<SocketException>()
                        .Or<Exception>()
                        .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            (ex, time) =>
                            {
                                this.Logger.LogWarning(ex, "CallAsync: ");
                            });

                    var correlationId = Guid.NewGuid().ToString();

                    var tcs = new TaskCompletionSource<TIntegrationEventReply>(TaskCreationOptions
                        .RunContinuationsAsynchronously);
                    var addResult = this._callbackMapper.TryAdd(correlationId, tcs);

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
                                        properties.CorrelationId = correlationId;
                                        properties.ReplyTo = this._queueNameReply; //ToDo

                                        channel.BasicPublish(
                                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeName, routingKey,
                                            true, properties, body);
                                    });
                                }
                            }
                        }
                    }

                    //cancellationToken.Register(() => this._callbackMapper.TryRemove(correlationId, out var tmp));
                    cancellationToken.Register(() => this.HandleResponse(correlationId, cancellationToken));

                    var result = await tcs.Task.ConfigureAwait(false);

                    return result;
                }

                this.Logger.LogWarning("CallAsync: {0}", "Not connected to bus!");
            }
            catch (TaskCanceledException)
            {
                this.Logger.LogWarning("CallAsync: {0}", "No response in time.");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CallAsync: ");
            }

            return default;
        }

        private void HandleResponse(string correlationId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (this._callbackMapper.TryGetValue(correlationId, out var value))
                {
                    var trySetCancelled = value.TrySetCanceled(cancellationToken);

                    var tryRemoveResult = this._callbackMapper.TryRemove(correlationId, out var tmp);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "HandleResponse: ");
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

                    //channel.QueueDeclare(this.QueueName,
                    //    this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                    //    this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                    //    this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
                    //ToDo
                    channel.QueueDeclare(this._queueNameReply,
                        this.EventBusParameters.QueueDeclareParameters.QueueDurable,
                        this.EventBusParameters.QueueDeclareParameters.QueueExclusive,
                        this.EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger.LogError(rex, "EventBusRabbitMqRpcClient RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMqRpcClient QueueInitialize: ");
            }
        }

        #region [Subscribe]

        //public async ValueTask SubscribeRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(string replyRoutingKey)
        //    where TIntegrationEventReply : IIntegrationEventReply, new()
        //    where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        //{
        //    var eventNameResult = this.SubsManager.GetEventReplyKey<TIntegrationEventReply>();
        //    //this.Logger.LogTrace("SubscribeRpcClient reply routing key: {0}, event name result: {1}", replyRoutingKey, eventNameResult);
        //    await this.DoInternalSubscriptionRpc(eventNameResult + "." + replyRoutingKey).ConfigureAwait(false);
        //    this.SubsManager.AddSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(eventNameResult + "." + replyRoutingKey);
        //    await this.StartBasicConsume<TIntegrationEventReply>().ConfigureAwait(false);
        //}

        public async ValueTask<bool> SubscribeRpcClient<TIntegrationRpcClientHandler>(string replyRoutingKey, bool asyncMode = true)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            var eventNameResult = this.SubsManager.GetEventReplyKey<TIntegrationEventReply>();
            //this.Logger.LogTrace("SubscribeRpcClient reply routing key: {0}, event name result: {1}", replyRoutingKey, eventNameResult);
            var internalSubscriptionResult = await this.DoInternalSubscriptionRpc(eventNameResult + "." + replyRoutingKey).ConfigureAwait(false);

            if (internalSubscriptionResult)
            {
                this.SubsManager.AddSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(eventNameResult + "." + replyRoutingKey);
                if (asyncMode)
                {
                    return await this.StartBasicConsumeAsync().ConfigureAwait(false);
                }
                else
                {
                    return this.StartBasicConsume();
                }
            }

            return false;
        }

        private async ValueTask<bool> DoInternalSubscriptionRpc(string eventNameResult)
        {
            try
            {
                //var containsKey = this.SubsManager.HasSubscriptionsForEvent(eventNameResult);
                if (this.SubsManager.HasSubscriptionsForEvent(eventNameResult))
                {
                    return false;
                }

                if (!this.PersistentConnection.IsConnected)
                {
                    var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);

                    if (!connectionResult)
                    {
                        this.Logger.LogWarning("EventBusRabbitMqRpcClient DoInternalSubscriptionRpc: {0}!", "no connection");
                        return false;
                    }
                }

                using (var channel = this.PersistentConnection.CreateModel())
                {
                    if (channel != null)
                    {
                        this.QueueInitialize(channel);

                        if (!String.IsNullOrEmpty(this._queueNameReply) &&
                            !String.IsNullOrEmpty(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName))
                        {
                            channel.QueueBind(this._queueNameReply,
                                this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                eventNameResult); //ToDo

                            return true;
                        }
                    }
                }
            }
            catch (RabbitMQClientException rex)
            {
                this.Logger.LogError(rex, "EventBusRabbitMqRpcClient RabbitMQClientException DoInternalSubscriptionRpc: ");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "EventBusRabbitMqRpcClient DoInternalSubscriptionRpc: ");
            }

            return false;
        }

        #endregion

        #region [Unsubscribe]

        //public void UnsubscribeRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(string routingKey)
        //    where TIntegrationEventReply : IIntegrationEventReply, new()
        //    where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        //{
        //    this.SubsManager.RemoveSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(routingKey);
        //}

        public void UnsubscribeRpcClient<TIntegrationRpcClientHandler>(string routingKey)
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            this.SubsManager.RemoveSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(routingKey);
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
                this.SubsManager.ClearReply();
            }

            base.Dispose(disposing);
        }

        #endregion



        protected bool StartBasicConsume()
        {
            this.Logger.LogTrace("EventBusRabbitMqRpcClient Starting RabbitMQ basic consume");

            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpcClient ConsumerChannel is null!");
                    return false;
                }

                if (this.ConsumerChannel.Value != null)
                {
                    var model = this.ConsumerChannel.Value.Result;
                    var consumer = new EventingBasicConsumer(model);

                    consumer.Received += this.ConsumerReceived;

                    //var consumer = new EventingBasicConsumer(await this.ConsumerChannel);

                    //consumer.Received += this.ConsumerReceived;


                    // autoAck specifies that as soon as the consumer gets the message,
                    // it will ack, even if it dies mid-way through the callback

                    model.BasicConsume(
                        queue: this._queueNameReply, //ToDo
                        autoAck: true, //ToDo
                        consumer: consumer);

                    //this.Logger.LogInformation("EventBusRabbitMqRpcClient StartBasicConsume done. Queue name: {0}, autoAck: {1}", this._queueNameReply, true);

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

        protected async ValueTask<bool> StartBasicConsumeAsync()
        {
            this.Logger.LogTrace("EventBusRabbitMqRpcClient Starting RabbitMQ basic consume");

            try
            {
                if (this.ConsumerChannel is null)
                {
                    this.Logger.LogWarning("EventBusRabbitMqRpcClient ConsumerChannel is null!");
                    return false;
                }

                if (this.ConsumerChannel.Value != null)
                {
                    var asyncConsumer = new AsyncEventingBasicConsumer(await this.ConsumerChannel);

                    asyncConsumer.Received += this.ConsumerReceivedAsync;

                    //var consumer = new EventingBasicConsumer(await this.ConsumerChannel);

                    //consumer.Received += this.ConsumerReceived;


                    // autoAck specifies that as soon as the consumer gets the message,
                    // it will ack, even if it dies mid-way through the callback

                    (await this.ConsumerChannel).BasicConsume(
                        queue: this._queueNameReply, //ToDo
                        autoAck: true, //ToDo
                        consumer: asyncConsumer);

                    //this.Logger.LogInformation("EventBusRabbitMqRpcClient StartBasicConsume done. Queue name: {0}, autoAck: {1}", this._queueNameReply, true);

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

        protected void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                if (!this._callbackMapper.TryRemove(eventArgs.BasicProperties.CorrelationId, out var tcs))
                {
                    return;
                }

                this.ProcessEventReply(eventArgs.RoutingKey, eventName, eventArgs.Body, tcs);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "ConsumerReceivedReply: {0}", eventName);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            //ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false); //ToDo
        }

        protected async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                if (!this._callbackMapper.TryRemove(eventArgs.BasicProperties.CorrelationId, out var tcs))
                {
                    this.Logger.LogWarning("ConsumerReceivedAsync TryRemove: {0}", eventArgs.BasicProperties.CorrelationId);
                    return;
                }

                if (tcs != null)
                {
                    await this.ProcessEventReplyAsync(eventArgs.RoutingKey, eventName, eventArgs.Body, tcs)
                        .ConfigureAwait(false);
                }
                else
                {
                    this.Logger.LogError("ConsumerReceivedAsync: cts null!");
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "ConsumerReceivedReply: {0}", eventName);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            //ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false); //ToDo
        }

        protected IModel CreateConsumerChannel()
        {
            //this.Logger.LogTrace("EventBusRabbitMqRpcClient CreateConsumerChannel queue name: {0} - queue reply name: {1}", this.QueueName, this._queueNameReply);
            try
            {
                if (this.PersistentConnection is null)
                {
                    return null;
                }

                if (!this.PersistentConnection.IsConnected)
                {
                    this.PersistentConnection.TryConnect(); //.TryConnectAsync().ConfigureAwait(false);
                    //this.PersistentConnection.TryConnect();
                }

                var channel = this.PersistentConnection.CreateModel();
                if (channel != null)
                {
                    //this.QueueInitialize(channel);

                    channel.CallbackException += (sender, ea) =>
                    {
                        this.Logger.LogError(ea.Exception, "CallbackException: ");
                        this.ConsumerChannel.Value.Dispose();
                        this.ConsumerChannel = new AsyncLazy<IModel>(() => this.CreateConsumerChannel());
                        this.StartBasicConsume();
                    };

                    return channel;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannel: ");
            }

            return null;
        }

        protected async ValueTask<IModel> CreateConsumerChannelAsync(CancellationToken cancel = default)
        {
            //this.Logger.LogTrace("EventBusRabbitMqRpcClient CreateConsumerChannelAsync queue name: {0} - queue reply name: {1}", this.QueueName, this._queueNameReply);
            try
            {
                if (this.PersistentConnection is null)
                {
                    return null;
                }

                if (!this.PersistentConnection.IsConnected)
                {
                    var connectionResult = await this.PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                    if (!connectionResult)
                    {
                        return null;
                    }
                }

                var channel = this.PersistentConnection.CreateModel();
                if (channel != null)
                {
                    //this.QueueInitialize(channel);

                    channel.CallbackException += async (sender, ea) =>
                    {
                        this.Logger.LogError(ea.Exception, "CallbackException: ");
                        this.ConsumerChannel.Value.Dispose();
                        this.ConsumerChannel = new AsyncLazy<IModel>(async () => await this.CreateConsumerChannelAsync(cancel));
                        await this.StartBasicConsumeAsync().ConfigureAwait(false);
                    };

                    return channel;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateConsumerChannelAsync: ");
            }

            return null;
        }

        private void ProcessEventReply(string routingKey, string eventName, ReadOnlyMemory<byte> message,
            TaskCompletionSource<TIntegrationEventReply> tcs, CancellationToken cancel = default)
        {
            if (this.SubsManager is null)
            {
                return;
            }

            if (this.SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = this.SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    this.Logger.LogError("ProcessEventReply subscriptions no items! {0}", routingKey);
                }

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Rpc:
                            break;
                        case SubscriptionManagerType.RpcClient:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger.LogError("ProcessEventReplyClient _eventHandler is null!");
                                }
                                else
                                {

                                    //var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    //if (eventType is null)
                                    //{
                                    //    Logger.LogError("ProcessEventReplyClient: eventType is null! " + routingKey);
                                    //    return;
                                    //}

                                    var eventResultType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        this.Logger.LogError("ProcessEventReplyClient: eventResultType is null! {0}",
                                            routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEventReply>(ms);
                                        tcs.TrySetResult(integrationEvent);
                                        var concreteType =
                                            typeof(IIntegrationRpcClientHandler<>).MakeGenericType(
                                                eventResultType);
                                        concreteType.GetMethod("HandleReply")
                                            .Invoke(this.EventHandler, new[] {(object)integrationEvent, cancel});
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, "ProcessEventReplyClient: ");
                            }

                            break;

                        case SubscriptionManagerType.RpcServer:
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
                this.Logger.LogError("ProcessEventReplyClient HasSubscriptionsForEventReply {0} No Subscriptions!",
                    routingKey);
            }
        }

        private async ValueTask ProcessEventReplyAsync(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, TaskCompletionSource<TIntegrationEventReply> tcs, CancellationToken cancel = default)
        {
            if (this.SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = this.SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    this.Logger.LogError("ProcessEventReply subscriptions no items! {0}", routingKey);
                }

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Rpc:
                            break;
                        case SubscriptionManagerType.RpcClient:
                            try
                            {
                                if (this.EventHandler is null)
                                {
                                    this.Logger.LogError("ProcessEventReplyClient _eventHandler is null!");
                                }
                                else
                                {

                                    //var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    //if (eventType is null)
                                    //{
                                    //    Logger.LogError("ProcessEventReplyClient: eventType is null! " + routingKey);
                                    //    return;
                                    //}

                                    var eventResultType = this.SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        this.Logger.LogError("ProcessEventReplyClient: eventResultType is null! {0}",
                                            routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEventReply>(ms);
                                        if (!tcs.TrySetResult(integrationEvent))
                                        {
                                            this.Logger.LogWarning("ProcessEventReplyAsync tcs.TrySetResult error!");
                                        }

                                        var concreteType =
                                            typeof(IIntegrationRpcClientHandler<>).MakeGenericType(
                                                eventResultType);
                                        await ((ValueTask)concreteType.GetMethod("HandleReplyAsync")
                                                .Invoke(this.EventHandler, new[] {(object)integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger.LogError(ex, "ProcessEventReplyClient: ");
                            }

                            break;

                        case SubscriptionManagerType.RpcServer:
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
                this.Logger.LogError("ProcessEventReplyClient HasSubscriptionsForEventReply {0} No Subscriptions!",
                    routingKey);
            }
        }
    }
}
