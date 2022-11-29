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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBusRabbitMQ
{
    public class EventBusRabbitMq : DisposableObject, IEventBus
    {
        protected readonly bool Debug;
        protected readonly IRabbitMqPersistentConnection PersistentConnection;
        protected readonly ILogger<EventBusRabbitMq> Logger;
        protected readonly IEventBusSubscriptionsManager SubsManager;
        protected readonly IEventBusParameters EventBusParameters;

        public IIntegrationGeneralHandler EventHandler { get; }

        protected AsyncLazy<IModel> ConsumerChannel;
        protected string QueueName;

        #region [Constructor]

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
        {
            Debug = eventBusParameters.Debug;
            EventBusParameters = eventBusParameters;
            PersistentConnection =
                persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            Logger = loggerFactory.CreateLogger<EventBusRabbitMq>();
            SubsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            QueueName = queueName;
            EventHandler = eventHandler;
            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;

        }

        protected EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
        {
            Debug = eventBusParameters.Debug;
            EventBusParameters = eventBusParameters;
            PersistentConnection =
                persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            Logger = loggerFactory.CreateLogger<EventBusRabbitMq>();
            SubsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            QueueName = queueName;
            EventHandler = null;
            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;

        }

        #endregion

        public virtual void Initialize(CancellationToken cancel = default)
        {
            Logger.LogTrace("EventBusRabbitMq Initialize.");
            ConsumerChannel =
                new AsyncLazy<IModel>(async () => await CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
        }

        private async void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!PersistentConnection.IsConnected)
            {
                await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using (var channel = PersistentConnection.CreateModel())
            {
                channel.QueueUnbind(QueueName, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);

                if (!SubsManager.IsEmpty) return;
                QueueName = string.Empty;
                (await ConsumerChannel).Close();
            }
        }

        public virtual async ValueTask Publish(IIntegrationEvent @event)
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
                        Logger.LogWarning(ex, "Publish: ");
                    });

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

                            channel.BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                routingKey, true, properties,
                                body);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "EventBusRabbitMq Publish: ");
            }
        }

        protected virtual void QueueInitialize(IModel channel)
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
                //    //{"x-dead-letter-routing-key", "some-routing-key" }
                //};
                channel.QueueDeclare(QueueName, EventBusParameters.QueueDeclareParameters.QueueDurable,
                    EventBusParameters.QueueDeclareParameters.QueueExclusive,
                    EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
            }
            catch (RabbitMQClientException rex)
            {
                Logger.LogError(rex, "EventBusRabbitMq RabbitMQClientException QueueInitialize: ");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "EventBusRabbitMq QueueInitialize: ");
            }
        }

        #region [Subscribe]

        public async ValueTask Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {

            var eventName = SubsManager.GetEventKey<T>();
            await DoInternalSubscription(eventName).ConfigureAwait(false);
            SubsManager.AddSubscription<T, TH>();
            await StartBasicConsume().ConfigureAwait(false);
        }

        protected async ValueTask DoInternalSubscription(string eventName)
        {
            var containsKey = SubsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey) return;
            if (!PersistentConnection.IsConnected)
            {
                await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using (var channel = PersistentConnection.CreateModel())
            {
                QueueInitialize(channel);

                channel.QueueBind(QueueName, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
            }
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
            ConsumerChannel?.Value.Dispose();
            SubsManager?.Clear();
        }

        protected virtual async ValueTask<bool> StartBasicConsume()
        {
            Logger.LogTrace("EventBusRabbitMq Starting RabbitMQ basic consume.");
            try
            {
                if (ConsumerChannel is null)
                {
                    Logger.LogWarning("ConsumerChannel is null!");
                    return false;
                }

                if (ConsumerChannel?.Value != null)
                {
                    var consumer = new AsyncEventingBasicConsumer(await ConsumerChannel);

                    consumer.Received += ConsumerReceivedAsync;

                    (await ConsumerChannel).BasicConsume(
                        queue: QueueName,
                        autoAck: false,
                        consumer: consumer);

                    Logger.LogInformation("EventBusRabbitMq StartBasicConsume done. Queue name: {0}, autoAck: {1}",
                        QueueName, true);

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

        protected virtual void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                ProcessEvent(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "ConsumerReceived: {0}", eventName);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                ConsumerChannel.Value.Result.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
            }
        }

        protected virtual async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

            try
            {
                await ProcessEvent(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "ConsumerReceived: {0}", eventName);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                (await ConsumerChannel).BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
            }
        }

        protected async virtual ValueTask<IModel> CreateConsumerChannelAsync(CancellationToken cancel = default)
        {
            Logger.LogTrace("CreateConsumerChannelAsync queue name: {0}", QueueName);

            if (!PersistentConnection.IsConnected)
            {
                await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            var channel = PersistentConnection.CreateModel();

            QueueInitialize(channel);

            channel.BasicQos(0, 1, false);

            channel.CallbackException += async (sender, ea) =>
            {
                Logger.LogError(ea.Exception, "CallbackException ExchangeName: {0} - QueueName: {1}",
                    EventBusParameters.ExchangeDeclareParameters.ExchangeName, QueueName);
                ConsumerChannel?.Value.Dispose();
                ConsumerChannel = new AsyncLazy<IModel>(async () =>
                    await CreateConsumerChannelAsync(cancel).ConfigureAwait(false));
                await StartBasicConsume().ConfigureAwait(false);
            };

            return channel;
        }

        protected virtual async ValueTask ProcessEvent(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
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
                                        Logger.LogError("ProcessEvent Typed: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType =
                                            typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                                        await ((ValueTask)concreteType.GetMethod("Handle")
                                                .Invoke(EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "ProcessEvent Typed: ");
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
                                        Logger.LogError("ProcessQueue: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType =
                                            typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);
                                        await ((ValueTask<bool>)concreteType.GetMethod("Enqueue")
                                                .Invoke(EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "ProcessQueue: ");
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
                                        Logger.LogError("ProcessEvent: eventType is null! {0}", routingKey);
                                        return;
                                    }

                                    var eventResultType = SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventResultType is null)
                                    {
                                        Logger.LogError("ProcessEvent: eventResultType is null! {0}", routingKey);
                                        return;
                                    }

                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                        var concreteType =
                                            typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType,
                                                eventResultType);
                                        await ((ValueTask)concreteType.GetMethod("HandleReply")
                                                .Invoke(EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "ProcessEvent Rpc: ");
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