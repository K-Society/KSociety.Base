using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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

namespace KSociety.Base.EventBusRabbitMQ
{
    public class EventBusRabbitMq : DisposableObject, IEventBus
    {
        protected readonly IRabbitMqPersistentConnection PersistentConnection;
        protected readonly ILogger<EventBusRabbitMq> Logger;
        protected readonly IEventBusSubscriptionsManager SubsManager;
        protected readonly IExchangeDeclareParameters ExchangeDeclareParameters;
        protected readonly IQueueDeclareParameters QueueDeclareParameters;

        public IIntegrationGeneralHandler EventHandler { get; }

        protected IModel ConsumerChannel;
        protected string QueueName;

        #region [Constructor]

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters, IQueueDeclareParameters queueDeclareParameters, string queueName = null,
            CancellationToken cancel = default)
        {
            ExchangeDeclareParameters = exchangeDeclareParameters;
            PersistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            Logger = loggerFactory.CreateLogger<EventBusRabbitMq>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            SubsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            QueueDeclareParameters = queueDeclareParameters;
            QueueName = queueName;
            EventHandler = eventHandler;
            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters, IQueueDeclareParameters queueDeclareParameters, string queueName = null,
            CancellationToken cancel = default)
        {
            ExchangeDeclareParameters = exchangeDeclareParameters;
            PersistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            Logger = loggerFactory.CreateLogger<EventBusRabbitMq>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            SubsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            QueueDeclareParameters = queueDeclareParameters;
            QueueName = queueName;
            EventHandler = null;
            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        #endregion

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!PersistentConnection.IsConnected)
            {
                PersistentConnection.TryConnect();
            }

            using var channel = PersistentConnection.CreateModel();
            channel.QueueUnbind(QueueName, ExchangeDeclareParameters.ExchangeName, eventName);

            if (!SubsManager.IsEmpty) return;
            QueueName = string.Empty;
            ConsumerChannel?.Close();
        }

        public virtual async ValueTask Publish(IIntegrationEvent @event)
        {
            //Logger.LogTrace("Publish: " + @event.GetType());
            try
            {
                if (!PersistentConnection.IsConnected)
                {
                    PersistentConnection.TryConnect();
                }

                var policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .Or<Exception>()
                    .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        Logger.LogWarning(ex.ToString());
                    });

                using var channel = PersistentConnection.CreateModel();
                var routingKey = @event.RoutingKey;
                channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName,
                    ExchangeDeclareParameters.ExchangeType, ExchangeDeclareParameters.ExchangeDurable,
                    ExchangeDeclareParameters.ExchangeAutoDelete);

                await using var ms = new MemoryStream();
                Serializer.Serialize(ms, @event);
                var body = ms.ToArray();

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 1; //2 = persistent, write on disk

                    channel.BasicPublish(ExchangeDeclareParameters.ExchangeName, 
                        routingKey, true, properties,
                        body);
                });
            }
            catch (Exception ex)
            {
                Logger.LogError("EventBusRabbitMq Publish: " + ex.Message + " - " + ex.StackTrace);
                if (!(ex.InnerException is null))
                {
                    Logger.LogError("EventBusRabbitMq Publish InnerException: " + ex.InnerException.Message + " - " + ex.InnerException.StackTrace);
                }
            }
        }

        #region [Subscribe]

        public void Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {

            var eventName = SubsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);
            SubsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        protected void DoInternalSubscription(string eventName)
        {
            var containsKey = SubsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey) return;
            if (!PersistentConnection.IsConnected)
            {
                PersistentConnection.TryConnect();
            }

            using var channel = PersistentConnection.CreateModel();
            channel.QueueBind(QueueName, ExchangeDeclareParameters.ExchangeName, eventName);
        }

        #endregion

        #region [Unsubscribe]

        public void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            SubsManager.RemoveSubscription<T, TH>();
        }

        #endregion

        protected override void DisposeManagedResources()
        {
            ConsumerChannel?.Dispose();
            SubsManager?.Clear();
        }

        protected virtual void StartBasicConsume()
        {
            Logger.LogTrace("Starting RabbitMQ basic consume");

            if (ConsumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(ConsumerChannel);

                consumer.Received += ConsumerReceivedAsync;

                ConsumerChannel.BasicConsume(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                Logger.LogError("StartBasicConsume can't call on ConsumerChannel == null");
            }
        }

        protected virtual void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                //if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                //{
                //    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                //}

                //await ProcessEvent(eventName, message);

                ProcessEvent(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                //Logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
                Logger.LogWarning("ConsumerReceived: " + eventName + " - " + ex.Message + " - " + ex.StackTrace);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateConsumerChannel RPC Received 2: " + ex.Message + " - " + ex.StackTrace);
            }
        }

        protected virtual async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                //if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                //{
                //    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                //}

                //await ProcessEvent(eventName, message);

                await ProcessEvent(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                //Logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
                Logger.LogWarning("ConsumerReceived: " + eventName + " - " + ex.Message + " - " + ex.StackTrace);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateConsumerChannel RPC Received 2: " + ex.Message + " - " + ex.StackTrace);
            }
        }

        protected virtual IModel CreateConsumerChannel(CancellationToken cancel = default)
        {
            if (!PersistentConnection.IsConnected)
            {
                PersistentConnection.TryConnect();
            }

            var channel = PersistentConnection.CreateModel();

            channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, 
                ExchangeDeclareParameters.ExchangeType, 
                ExchangeDeclareParameters.ExchangeDurable, 
                ExchangeDeclareParameters.ExchangeAutoDelete);

            channel.QueueDeclare(QueueName, QueueDeclareParameters.QueueDurable, 
                QueueDeclareParameters.QueueExclusive, 
                QueueDeclareParameters.QueueAutoDelete, null);
            channel.BasicQos(0, 1, false);

            channel.CallbackException += (sender, ea) =>
            {
                Logger.LogError("CallbackException: " + ExchangeDeclareParameters.ExchangeName + " " + QueueName + " " + ea.Exception.Message + " - " + ea.Exception.StackTrace);
                ConsumerChannel.Dispose();
                ConsumerChannel = CreateConsumerChannel(cancel);
                StartBasicConsume();
            };

            return channel;
        }

        protected virtual async ValueTask ProcessEvent(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            if (SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                
                var subscriptions = SubsManager.GetHandlersForEvent(routingKey);
                foreach (var subscription in subscriptions)
                {

                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Dynamic:
                            break;

                        case SubscriptionManagerType.Typed:
                            try
                            {
                                if (EventHandler is null)
                                {

                                }
                                else
                                {
                                    var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        Logger.LogError("ProcessEvent Typed: eventType is null! " + routingKey);
                                        return;
                                    }

                                    await using var ms = new MemoryStream(message.ToArray());
                                    var integrationEvent = Serializer.Deserialize(eventType, ms);
                                    var concreteType =
                                        typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                                    await ((ValueTask) concreteType.GetMethod("Handle")
                                        .Invoke(EventHandler, new[] {integrationEvent, cancel})).ConfigureAwait(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("ProcessEvent Typed: " + ex.Message + " - " + ex.StackTrace);
                            }
                            break;

                        case SubscriptionManagerType.Queue:
                            try
                            {
                                if (EventHandler is null)
                                {

                                }
                                else
                                {
                                    var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        Logger.LogError("ProcessQueue: eventType is null! " + routingKey);
                                        return;
                                    }

                                    await using var ms = new MemoryStream(message.ToArray());
                                    var integrationEvent = Serializer.Deserialize(eventType, ms);
                                    var concreteType =
                                        typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);
                                    await ((ValueTask<bool>) concreteType.GetMethod("Enqueue")
                                        .Invoke(EventHandler, new[] {integrationEvent, cancel})).ConfigureAwait(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("ProcessQueue: " + ex.Message + " - " + ex.StackTrace);
                            }
                            break;

                        case SubscriptionManagerType.Rpc:
                            try
                            {
                                if (EventHandler is null)
                                {

                                }
                                else
                                {
                                    var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        Logger.LogError("ProcessEvent: eventType is null! " + routingKey);
                                        return;
                                    }

                                    var eventResultType = SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        Logger.LogError("ProcessEvent: eventResultType is null! " + routingKey);
                                        return;
                                    }

                                    await using var ms = new MemoryStream(message.ToArray());
                                    var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                    var concreteType =
                                        typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType,
                                            eventResultType);
                                    await ((ValueTask) concreteType.GetMethod("HandleReply")
                                        .Invoke(EventHandler, new[] {integrationEvent, cancel})).ConfigureAwait(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("ProcessEvent Rpc: " + ex.Message + " - " + ex.StackTrace);
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }//ProcessEvent.
    }
}
