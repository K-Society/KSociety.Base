using System;
using System.IO;
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
using RabbitMQ.Client.Exceptions;

namespace KSociety.Base.EventBusRabbitMQ
{
    public sealed class EventBusRabbitMqQueue : EventBusRabbitMq, IEventBusQueue
    {

        #region [Constructor]

        public EventBusRabbitMqQueue(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            //IExchangeDeclareParameters exchangeDeclareParameters,
            //IQueueDeclareParameters queueDeclareParameters,
            string queueName = null,
            CancellationToken cancel = default)
        :base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters,/*exchangeDeclareParameters, queueDeclareParameters,*/ queueName, cancel)
        {

        }

        #endregion

        public IIntegrationQueueHandler<T> GetIntegrationQueueHandler<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            if (EventHandler is not null && EventHandler is IIntegrationQueueHandler<T> queue)
            {
                return queue;
            }

            return null;
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

            using var channel = PersistentConnection.CreateModel();
            var routingKey = @event.RoutingKey;

            channel.ExchangeDeclare(EventBusParameters.ExchangeDeclareParameters.ExchangeName, EventBusParameters.ExchangeDeclareParameters.ExchangeType, EventBusParameters.ExchangeDeclareParameters.ExchangeDurable, EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

            await using var ms = new MemoryStream();
            Serializer.Serialize(ms, @event);
            var body = ms.ToArray();

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 1; //2 = persistent, write on disk

                channel.BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName, routingKey, true, properties, body);
            });
        }

        #region [Subscribe]

        public void SubscribeQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            var eventName = SubsManager.GetEventKey<T>();
            DoInternalSubscription(eventName + "." + routingKey);
            SubsManager.AddSubscriptionQueue<T, TH>(eventName + "." + routingKey);
            StartBasicConsume();
        }
        
        #endregion

        #region [Unsubscribe]

        public void UnsubscribeQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            SubsManager.RemoveSubscriptionQueue<T, TH>(routingKey);
        }

        #endregion

        protected override async ValueTask ProcessEvent(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            if (SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                
                    var subscriptions = SubsManager.GetHandlersForEvent(routingKey);
                    foreach (var subscription in subscriptions)
                    {

                        switch (subscription.SubscriptionManagerType)
                        {


                            case SubscriptionManagerType.Queue:
                                try
                                {
                                    if (EventHandler is null)
                                    {

                                    }
                                    else
                                    {
                                        var eventType = SubsManager.GetEventTypeByName(routingKey);
                                        await using var ms = new MemoryStream(message.ToArray());
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType = typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);
                                        await ((ValueTask<bool>)concreteType.GetMethod("Enqueue")
                                            .Invoke(EventHandler, new[] { integrationEvent, cancel })).ConfigureAwait(false);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex, "ProcessQueue: ");
                                }
                                break;


                            case SubscriptionManagerType.Dynamic:
                                break;
                            case SubscriptionManagerType.Typed:
                                break;
                            case SubscriptionManagerType.Rpc:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
            }
        }//ProcessEvent.
    }
}

