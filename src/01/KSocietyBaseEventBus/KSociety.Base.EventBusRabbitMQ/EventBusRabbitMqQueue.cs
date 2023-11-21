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
    using RabbitMQ.Client.Exceptions;
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class EventBusRabbitMqQueue : EventBusRabbitMq, IEventBusQueue
    {

        #region [Constructor]

        public EventBusRabbitMqQueue(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager? subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {

        }

        public EventBusRabbitMqQueue(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager? subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null, ILogger<EventBusRabbitMq>? logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {

        }

        #endregion

        public IIntegrationQueueHandler<T>? GetIntegrationQueueHandler<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            if (this.EventHandler is IIntegrationQueueHandler<T> queue)
            {
                return queue;
            }

            return null;
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
                    this.Logger?.LogWarning(ex, "Publish: ");
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

        #region [Subscribe]

        public async ValueTask SubscribeQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            var eventName = this.SubsManager?.GetEventKey<T>();
            await this.DoInternalSubscription(eventName + "." + routingKey).ConfigureAwait(false);
            this.SubsManager?.AddSubscriptionQueue<T, TH>(eventName + "." + routingKey);
            await this.StartBasicConsume().ConfigureAwait(false);
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            this.SubsManager?.RemoveSubscriptionQueue<T, TH>(routingKey);
        }

        #endregion

        protected override async ValueTask ProcessEvent(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            if (this.SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                var subscriptions = this.SubsManager.GetHandlersForEvent(routingKey);
                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SubscriptionManagerType)
                    {

                        case SubscriptionManagerType.Queue:
                            try
                            {
                                if (this.EventHandler is null)
                                {

                                }
                                else
                                {
                                    var eventType = this.SubsManager.GetEventTypeByName(routingKey);
                                    using (var ms = new MemoryStream(message.ToArray()))
                                    {
                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
                                        var concreteType =
                                            typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);
                                        await ((ValueTask<bool>)concreteType.GetMethod("Enqueue")
                                                .Invoke(this.EventHandler, new[] {integrationEvent, cancel}))
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Logger?.LogError(ex, "ProcessQueue: ");
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
        } //ProcessEvent.
    }
}
