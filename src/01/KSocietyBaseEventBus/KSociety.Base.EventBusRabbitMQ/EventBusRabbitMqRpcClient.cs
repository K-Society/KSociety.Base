using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.Extensions.Logging;
using Polly;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBusRabbitMQ
{
    public sealed class EventBusRabbitMqRpcClient : EventBusRabbitMq, IEventBusRpcClient
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<dynamic>> _callbackMapper =
            new ConcurrentDictionary<string, TaskCompletionSource<dynamic>>();

        private string _queueNameReply;

        #region [Constructor]

        public EventBusRabbitMqRpcClient(IRabbitMqPersistentConnection persistentConnection,
            ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler? eventHandler, IEventBusSubscriptionsManager? subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {

        }

        #endregion

        public override void Initialize(CancellationToken cancel = default)
        {
            Logger.LogTrace("EventBusRabbitMqRpcClient Initialize.");
            _queueNameReply = QueueName + "_Reply";
            SubsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;
            ConsumerChannel =
                new AsyncLazy<IModel>(async () => await CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
        }

        public IIntegrationRpcClientHandler<TIntegrationEventReply> GetIntegrationRpcClientHandler<
            TIntegrationEventReply>()
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (EventHandler is IIntegrationRpcClientHandler<TIntegrationEventReply> queue)
            {
                return queue;
            }

            return null;
        }

        private async void SubsManager_OnEventReplyRemoved(object sender, string eventName)
        {
            if (!PersistentConnection.IsConnected)
            {
                await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using (var channel = PersistentConnection.CreateModel())
            {
                channel.QueueUnbind(QueueName, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
                channel.QueueUnbind(_queueNameReply, EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                    eventName); //ToDo
            }

            if (!SubsManager.IsReplyEmpty) return;

            _queueNameReply = string.Empty;
            QueueName = string.Empty;
            (await ConsumerChannel).Close();

        }

        public override async ValueTask Publish(IIntegrationEvent @event)
        {
            if (!PersistentConnection.IsConnected)
            {
                await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .Or<Exception>()
                .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    Logger.LogWarning(ex, "Publish: ");
                });
            var correlationId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<dynamic>(TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            using (var channel = PersistentConnection.CreateModel())
            {
                var routingKey = @event.RoutingKey;

                channel.ExchangeDeclare(EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                    EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                    EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                    EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, @event);
                    var body = ms.ToArray();

                    policy.Execute(() =>
                    {
                        var properties = channel.CreateBasicProperties();
                        properties.DeliveryMode = 1; //2 = persistent, write on disk
                        properties.CorrelationId = correlationId;
                        properties.ReplyTo = _queueNameReply; //ToDo
                        channel.BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName, routingKey,
                            true,
                            properties, body);
                    });
                }
            }
        }

        public async Task<TIntegrationEventReply> CallAsync<TIntegrationEventReply>(IIntegrationEvent @event,
            CancellationToken cancellationToken = default)
            where TIntegrationEventReply : IIntegrationEventReply
        {

            try
            {
                if (!PersistentConnection.IsConnected)
                {
                    await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                }

                var policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .Or<Exception>()
                    .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        Logger.LogWarning(ex, "CallAsync: ");
                    });

                var correlationId = Guid.NewGuid().ToString();

                var tcs = new TaskCompletionSource<dynamic>(TaskCreationOptions.RunContinuationsAsynchronously);
                _callbackMapper.TryAdd(correlationId, tcs);

                using (var channel = PersistentConnection.CreateModel())
                {
                    var routingKey = @event.RoutingKey;

                    channel.ExchangeDeclare(EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                        EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                        EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                        EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

                    using (var ms = new MemoryStream())
                    {
                        Serializer.Serialize(ms, @event);
                        var body = ms.ToArray();

                        policy.Execute(() =>
                        {
                            var properties = channel.CreateBasicProperties();

                            properties.DeliveryMode = 1; //2 = persistent, write on disk
                            properties.CorrelationId = correlationId;
                            properties.ReplyTo = _queueNameReply; //ToDo

                            channel.BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName, routingKey,
                                true, properties,
                                body);
                        });
                    }
                }

                cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));

                var result = await tcs.Task.ConfigureAwait(false);

                return (TIntegrationEventReply)result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CallAsync: ");
            }

            return default;
        }

        protected override void QueueInitialize(IModel channel)
        {
            try
            {
                channel.ExchangeDeclare(EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                    EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                    EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                    EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

                //var args = new Dictionary<string, object>
                //{
                //    { "x-dead-letter-exchange", EventBusParameters.ExchangeDeclareParameters.ExchangeName }
                //};

                channel.QueueDeclare(QueueName, EventBusParameters.QueueDeclareParameters.QueueDurable,
                    EventBusParameters.QueueDeclareParameters.QueueExclusive,
                    EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
                //ToDo
                channel.QueueDeclare(_queueNameReply, EventBusParameters.QueueDeclareParameters.QueueDurable,
                    EventBusParameters.QueueDeclareParameters.QueueExclusive,
                    EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
            }
            catch (RabbitMQClientException rex)
            {
                Logger.LogError(rex, "EventBusRabbitMqRpcClient RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "EventBusRabbitMqRpcClient QueueInitialize: ");
            }
        }

        #region [Subscribe]

        public async ValueTask SubscribeRpcClient<TIntegrationEventReply, TH>(string replyRoutingKey)
            where TIntegrationEventReply : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            var eventNameResult = SubsManager.GetEventReplyKey<TIntegrationEventReply>();
            Logger.LogTrace("SubscribeRpcClient reply routing key: {0}, event name result: {1}", replyRoutingKey,
                eventNameResult);
            await DoInternalSubscriptionRpc(eventNameResult + "." + replyRoutingKey);
            SubsManager.AddSubscriptionRpcClient<TIntegrationEventReply, TH>(eventNameResult + "." + replyRoutingKey);
            await StartBasicConsume().ConfigureAwait(false);
        }

        private async ValueTask DoInternalSubscriptionRpc(string eventNameResult)
        {
            try
            {
                var containsKey = SubsManager.HasSubscriptionsForEvent(eventNameResult);
                if (containsKey) return;
                if (!PersistentConnection.IsConnected)
                {
                    await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                }

                using (var channel = PersistentConnection.CreateModel())
                {
                    QueueInitialize(channel);

                    channel.QueueBind(_queueNameReply, EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                        eventNameResult); //ToDo
                }
            }
            catch (RabbitMQClientException rex)
            {
                Logger.LogError(rex, "EventBusRabbitMqRpcClient RabbitMQClientException DoInternalSubscriptionRpc: ");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "EventBusRabbitMqRpcClient DoInternalSubscriptionRpc: ");
            }
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeRpcClient<TIntegrationEventReply, TH>(string routingKey)
            where TIntegrationEventReply : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            SubsManager.RemoveSubscriptionRpcClient<TIntegrationEventReply, TH>(routingKey);
        }

        #endregion

        protected override void DisposeManagedResources()
        {
            ConsumerChannel?.Value.Dispose();
            SubsManager?.Clear();
            SubsManager?.ClearReply();
        }

        protected override async ValueTask<bool> StartBasicConsume()
        {
            Logger.LogTrace("EventBusRabbitMqRpcClient Starting RabbitMQ basic consume");

            try
            {
                if (ConsumerChannel is null)
                {
                    Logger.LogWarning("EventBusRabbitMqRpcClient ConsumerChannel is null!");
                    return false;
                }

                if (ConsumerChannel?.Value != null)
                {
                    var consumer = new AsyncEventingBasicConsumer(await ConsumerChannel);

                    consumer.Received += ConsumerReceivedAsync;


                    // autoAck specifies that as soon as the consumer gets the message,
                    // it will ack, even if it dies mid-way through the callback

                    (await ConsumerChannel).BasicConsume(
                        queue: _queueNameReply, //ToDo
                        autoAck: true, //ToDo
                        consumer: consumer);

                    Logger.LogInformation(
                        "EventBusRabbitMqRpcClient StartBasicConsume done. Queue name: {0}, autoAck: {1}",
                        _queueNameReply, true);

                    return true;
                }
                else
                {
                    Logger.LogError("StartBasicConsume can't call on ConsumerChannel is null");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "StartBasicConsume: ");
            }

            return false;
        }

        protected override void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                if (!_callbackMapper.TryRemove(eventArgs.BasicProperties.CorrelationId,
                        out TaskCompletionSource<dynamic> tcs))
                    return;

                ProcessEventReply(eventArgs.RoutingKey, eventName, eventArgs.Body, tcs);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "ConsumerReceivedReply: {0}", eventName);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            //ConsumerChannel?.BasicAck(eventArgs.DeliveryTag, multiple: false); //ToDo
        }

        protected override async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                if (!_callbackMapper.TryRemove(eventArgs.BasicProperties.CorrelationId,
                        out TaskCompletionSource<dynamic> tcs))
                {
                    Logger.LogWarning("ConsumerReceivedAsync TryRemove: {0}", eventArgs.BasicProperties.CorrelationId);
                    return;
                }

                if (tcs != null)
                {
                    await ProcessEventReplyAsync(eventArgs.RoutingKey, eventName, eventArgs.Body, tcs)
                        .ConfigureAwait(false);
                }
                else
                {
                    Logger.LogError("ConsumerReceivedAsync: cts null!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "ConsumerReceivedReply: {0}", eventName);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            //ConsumerChannel?.BasicAck(eventArgs.DeliveryTag, multiple: false); //ToDo
        }

        protected async override ValueTask<IModel> CreateConsumerChannelAsync(CancellationToken cancel = default)
        {
            Logger.LogTrace(
                "EventBusRabbitMqRpcClient CreateConsumerChannelAsync queue name: {0} - queue reply name: {1}",
                QueueName, _queueNameReply);
            try
            {
                if (!PersistentConnection.IsConnected)
                {
                    await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                }

                var channel = PersistentConnection.CreateModel();

                QueueInitialize(channel);

                channel.CallbackException += async (sender, ea) =>
                {
                    Logger.LogError(ea.Exception, "CallbackException: ");
                    ConsumerChannel?.Value.Dispose();
                    ConsumerChannel = new AsyncLazy<IModel>(async () => await CreateConsumerChannelAsync(cancel));
                    await StartBasicConsume().ConfigureAwait(false);
                };

                return channel;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CreateConsumerChannelAsync: ");
            }

            return null;
        }

        private void ProcessEventReply(string routingKey, string eventName, ReadOnlyMemory<byte> message,
            TaskCompletionSource<dynamic> tcs,
            CancellationToken cancel = default)
        {
            if (SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    Logger.LogError("ProcessEventReply subscriptions no items! {0}", routingKey);
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
                                if (EventHandler is null)
                                {
                                    Logger.LogError("ProcessEventReplyClient _eventHandler is null!");
                                }
                                else
                                {

                                    //var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    //if (eventType is null)
                                    //{
                                    //    Logger.LogError("ProcessEventReplyClient: eventType is null! " + routingKey);
                                    //    return;
                                    //}

                                    var eventResultType = SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        Logger.LogError("ProcessEventReplyClient: eventResultType is null! {0}",
                                            routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                        tcs.TrySetResult( /*(dynamic)*/integrationEvent);
                                        var concreteType =
                                            typeof(IIntegrationRpcClientHandler<>).MakeGenericType(
                                                eventResultType);
                                        concreteType.GetMethod("HandleReply")
                                            .Invoke(EventHandler, new[] {integrationEvent, cancel});
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "ProcessEventReplyClient: ");
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
                Logger.LogError("ProcessEventReplyClient HasSubscriptionsForEventReply {0} No Subscriptions!",
                    routingKey);
            }
        }

        private async ValueTask ProcessEventReplyAsync(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, TaskCompletionSource<dynamic> tcs,
            CancellationToken cancel = default)
        {
            if (SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    Logger.LogError("ProcessEventReply subscriptions no items! {0}", routingKey);
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
                                if (EventHandler is null)
                                {
                                    Logger.LogError("ProcessEventReplyClient _eventHandler is null!");
                                }
                                else
                                {

                                    //var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    //if (eventType is null)
                                    //{
                                    //    Logger.LogError("ProcessEventReplyClient: eventType is null! " + routingKey);
                                    //    return;
                                    //}

                                    var eventResultType = SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        Logger.LogError("ProcessEventReplyClient: eventResultType is null! {0}",
                                            routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                        if (!tcs.TrySetResult( /*(dynamic)*/integrationEvent))
                                        {
                                            Logger.LogWarning("ProcessEventReplyAsync tcs.TrySetResult error!");
                                        }

                                        var concreteType =
                                            typeof(IIntegrationRpcClientHandler<>).MakeGenericType(
                                                eventResultType);
                                        await ((ValueTask)concreteType.GetMethod("HandleReplyAsync")
                                                .Invoke(EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "ProcessEventReplyClient: ");
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
                Logger.LogError("ProcessEventReplyClient HasSubscriptionsForEventReply {0} No Subscriptions!",
                    routingKey);
            }
        }
    }
}