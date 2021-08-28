using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using Polly;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KSociety.Base.EventBusRabbitMQ
{
    public sealed class EventBusRabbitMqRpcClient : EventBusRabbitMq, IEventBusRpcClient
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<dynamic>> _callbackMapper =
            new();

        private string _queueNameReply;

        #region [Constructor]

        public EventBusRabbitMqRpcClient(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters,
            string queueName = null,
            CancellationToken cancel = default)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, exchangeDeclareParameters, queueDeclareParameters, queueName, cancel)
        {
            _queueNameReply = QueueName + "_Reply";

            SubsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;
            //ConsumerChannel = CreateConsumerChannel(cancel);
            //ConsumerChannel = new Lazy<IModel>(CreateConsumerChannelAsync(cancel).Result);
        }

        #endregion

        public IIntegrationRpcClientHandler<TIntegrationEventReply> GetIntegrationRpcClientHandler<TIntegrationEventReply>()
            where TIntegrationEventReply : IIntegrationEventReply
        {
            if (EventHandler is not null && EventHandler is IIntegrationRpcClientHandler<TIntegrationEventReply> queue)
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
                channel.QueueUnbind(QueueName, ExchangeDeclareParameters.ExchangeName, eventName);
                channel.QueueUnbind(_queueNameReply, ExchangeDeclareParameters.ExchangeName, eventName); //ToDo
            }

            if (!SubsManager.IsReplyEmpty) return;

            _queueNameReply = string.Empty;
            QueueName = string.Empty;
            ConsumerChannel?.Value.Close();

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
                    Logger.LogWarning(ex.ToString());
                });
            var correlationId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<dynamic>(TaskCreationOptions.RunContinuationsAsynchronously);
            _callbackMapper.TryAdd(correlationId, tcs);

            using (var channel = PersistentConnection.CreateModel())
            {
                var routingKey = @event.RoutingKey;

                channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType,
                    ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

                await using var ms = new MemoryStream();
                Serializer.Serialize(ms, @event);
                var body = ms.ToArray();


                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 1; //2 = persistent, write on disk
                    properties.CorrelationId = correlationId;
                    properties.ReplyTo = _queueNameReply; //ToDo
                    channel.BasicPublish(ExchangeDeclareParameters.ExchangeName, routingKey, true, properties, body);
                });
            }
        }

        public async Task<TIntegrationEventReply> CallAsync<TIntegrationEventReply>(IIntegrationEvent @event, CancellationToken cancellationToken = default)
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
                        Logger.LogWarning(ex.ToString());
                    });

                var correlationId = Guid.NewGuid().ToString();

                var tcs = new TaskCompletionSource<dynamic>(TaskCreationOptions.RunContinuationsAsynchronously);
                _callbackMapper.TryAdd(correlationId, tcs);

                using (var channel = PersistentConnection.CreateModel())
                {
                    var routingKey = @event.RoutingKey;

                    channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName,
                        ExchangeDeclareParameters.ExchangeType,
                        ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

                    await using var ms = new MemoryStream();

                    Serializer.Serialize(ms, @event);
                    var body = ms.ToArray();

                    policy.Execute(() =>
                    {
                        var properties = channel.CreateBasicProperties();
                        //properties.ContentType = "application/protobuf";
                        properties.DeliveryMode = 1; //2 = persistent, write on disk
                        properties.CorrelationId = correlationId;
                        //properties.ReplyTo = QueueName;
                        properties.ReplyTo = _queueNameReply; //ToDo

                        channel.BasicPublish(ExchangeDeclareParameters.ExchangeName, routingKey, true, properties,
                            body);
                    });
                }

                cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));

                var result = await tcs.Task.ConfigureAwait(false);

                return (TIntegrationEventReply)result; 
            }
            catch (Exception ex)
            {
                Logger.LogError("CallAsync: " + ex.Message + " " + ex.StackTrace);
            }
            return default;
        }

        #region [Subscribe]

        public void SubscribeRpcClient<TIntegrationEventReply, TH>(string replyRoutingKey)
            where TIntegrationEventReply : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            var eventNameResult = SubsManager.GetEventReplyKey<TIntegrationEventReply>();
            DoInternalSubscriptionRpc(eventNameResult + "." + replyRoutingKey);
            SubsManager.AddSubscriptionRpcClient<TIntegrationEventReply, TH>(eventNameResult + "." + replyRoutingKey);
            StartBasicConsume();
        }

        private async void DoInternalSubscriptionRpc(string eventNameResult)
        {
            var containsKey = SubsManager.HasSubscriptionsForEvent(eventNameResult);
            if (containsKey) return;
            if (!PersistentConnection.IsConnected)
            {
                await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using var channel = PersistentConnection.CreateModel();

            channel.QueueBind(_queueNameReply, ExchangeDeclareParameters.ExchangeName, eventNameResult); //ToDo
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

        protected override void StartBasicConsume()
        {
            Logger.LogTrace("Starting RabbitMQ basic consume");

            try
            {
                if (ConsumerChannel is null) { Logger.LogWarning("ConsumerChannel is null"); return; }
                if (ConsumerChannel?.Value is not null)
                {
                    var consumer = new AsyncEventingBasicConsumer(ConsumerChannel?.Value);

                    consumer.Received += ConsumerReceivedAsync;


                    // autoAck specifies that as soon as the consumer gets the message,
                    // it will ack, even if it dies mid-way through the callback
                    ConsumerChannel?.Value.BasicConsume(
                        queue: _queueNameReply, //ToDo
                        autoAck: true, //ToDo
                        consumer: consumer);
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
        }

        protected override void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                if (!_callbackMapper.TryRemove(eventArgs.BasicProperties.CorrelationId, out TaskCompletionSource<dynamic> tcs))
                    return;

                ProcessEventReply(eventArgs.RoutingKey, eventName, eventArgs.Body, tcs);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("ConsumerReceivedReply: " + eventName + " - " + ex.Message + " - " + ex.StackTrace);
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
                    Logger.LogWarning("ConsumerReceivedAsync TryRemove: " + eventArgs.BasicProperties.CorrelationId);
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
                Logger.LogWarning("ConsumerReceivedReply: " + eventName + " - " + ex.Message + " - " + ex.StackTrace);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            //ConsumerChannel?.BasicAck(eventArgs.DeliveryTag, multiple: false); //ToDo
        }

        //protected override IModel CreateConsumerChannel(CancellationToken cancel = default)
        //{
        //    try
        //    {
        //        if (!PersistentConnection.IsConnected)
        //        {
        //            PersistentConnection.TryConnect();
        //        }

        //        var channel = PersistentConnection.CreateModel();

        //        channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType,
        //            ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

        //        channel.QueueDeclare(QueueName, QueueDeclareParameters.QueueDurable, QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
        //        //ToDo
        //        channel.QueueDeclare(_queueNameReply, QueueDeclareParameters.QueueDurable,
        //            QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
        //        //channel.BasicQos(0, 1, false);

        //        channel.CallbackException += (sender, ea) =>
        //        {
        //            Logger.LogError("CallbackException: " + ea.Exception.Message);
        //            ConsumerChannel?.Dispose();
        //            ConsumerChannel = CreateConsumerChannel(cancel);
        //            StartBasicConsume();
        //        };

        //        return channel;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError("CreateConsumerChannel: " + ex.Message + " - " + ex.StackTrace);
        //    }

        //    return null;
        //}

        //protected override IModel CreateConsumerChannel(CancellationToken cancel = default)
        //{
        //    try
        //    {
        //        if (!PersistentConnection.IsConnected)
        //        {
        //            PersistentConnection.TryConnect();
        //        }

        //        var channel = PersistentConnection.CreateModel();

        //        channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType,
        //            ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

        //        channel.QueueDeclare(QueueName, QueueDeclareParameters.QueueDurable, QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
        //        //ToDo
        //        channel.QueueDeclare(_queueNameReply, QueueDeclareParameters.QueueDurable,
        //            QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
        //        //channel.BasicQos(0, 1, false);

        //        channel.CallbackException += (sender, ea) =>
        //        {
        //            Logger.LogError("CallbackException: " + ea.Exception.Message);
        //            ConsumerChannel?.Dispose();
        //            ConsumerChannel = CreateConsumerChannel(cancel);
        //            StartBasicConsume();
        //        };

        //        return channel;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError("CreateConsumerChannel: " + ex.Message + " - " + ex.StackTrace);
        //    }

        //    return null;
        //}

        protected async override ValueTask<IModel> CreateConsumerChannelAsync(CancellationToken cancel = default)
        {
            try
            {
                if (!PersistentConnection.IsConnected)
                {
                    await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
                }

                var channel = PersistentConnection.CreateModel();

                channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType,
                    ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

                channel.QueueDeclare(QueueName, QueueDeclareParameters.QueueDurable, QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
                //ToDo
                channel.QueueDeclare(_queueNameReply, QueueDeclareParameters.QueueDurable,
                    QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
                //channel.BasicQos(0, 1, false);

                channel.CallbackException += async (sender, ea) =>
                {
                    Logger.LogError(ea.Exception, "CallbackException: ");
                    ConsumerChannel?.Value.Dispose();
                    ConsumerChannel = new Lazy<IModel>(await CreateConsumerChannelAsync(cancel).ConfigureAwait(false));//await CreateConsumerChannelAsync(cancel).ConfigureAwait(false);
                    StartBasicConsume();
                };

                return channel;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CreateConsumerChannel: ");
            }

            return null;
        }

        private void ProcessEventReply(string routingKey, string eventName, ReadOnlyMemory<byte> message, TaskCompletionSource<dynamic> tcs,
            CancellationToken cancel = default)
        {
            if (SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    Logger.LogError("ProcessEventReply subscriptions no items! " + routingKey);
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
                                        Logger.LogError("ProcessEventReplyClient: eventResultType is null! " + routingKey);
                                        return;
                                    }

                                    using var ms = new MemoryStream(message.ToArray());
                                    var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                    tcs.TrySetResult(/*(dynamic)*/integrationEvent);
                                    var concreteType =
                                        typeof(IIntegrationRpcClientHandler<>).MakeGenericType(
                                            eventResultType);
                                    concreteType.GetMethod("HandleReply")
                                        .Invoke(EventHandler, new[] { integrationEvent, cancel });
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("ProcessEventReplyClient: " + ex.Message + " - " + ex.StackTrace);
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
                Logger.LogError("ProcessEventReplyClient HasSubscriptionsForEventReply " + routingKey + " No Subscriptions!");
            }
        }

        private async ValueTask ProcessEventReplyAsync(string routingKey, string eventName, ReadOnlyMemory<byte> message, TaskCompletionSource<dynamic> tcs,
            CancellationToken cancel = default)
        {
            if (SubsManager.HasSubscriptionsForEventReply(routingKey))
            {
                var subscriptions = SubsManager.GetHandlersForEventReply(routingKey);
                if (!subscriptions.Any())
                {
                    Logger.LogError("ProcessEventReply subscriptions no items! " + routingKey);
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
                                        Logger.LogError("ProcessEventReplyClient: eventResultType is null! " + routingKey);
                                        return;
                                    }

                                    await using var ms = new MemoryStream(message.ToArray());
                                    var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                    if (!tcs.TrySetResult(/*(dynamic)*/integrationEvent))
                                    {
                                        Logger.LogWarning("ProcessEventReplyAsync tcs.TrySetResult error!");
                                    }
                                    var concreteType =
                                        typeof(IIntegrationRpcClientHandler<>).MakeGenericType(
                                            eventResultType);
                                    await ((ValueTask)concreteType.GetMethod("HandleReplyAsync")
                                        .Invoke(EventHandler, new[] { integrationEvent, cancel })).ConfigureAwait(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("ProcessEventReplyClient: " + ex.Message + " - " + ex.StackTrace);
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
                Logger.LogError("ProcessEventReplyClient HasSubscriptionsForEventReply " + routingKey + " No Subscriptions!");
            }
        }
    }
}
