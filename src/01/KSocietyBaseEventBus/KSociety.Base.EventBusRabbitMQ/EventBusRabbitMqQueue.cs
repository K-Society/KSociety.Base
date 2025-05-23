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
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {

        }

        public EventBusRabbitMqQueue(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {

        }

        #endregion

        public IIntegrationQueueHandler<TIntegrationEvent> GetIntegrationQueueHandler<TIntegrationEvent, TIntegrationQueueHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>
        {
            if (this.EventHandler is IIntegrationQueueHandler<TIntegrationEvent> queue)
            {
                return queue;
            }

            return null;
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
                    this.Logger.LogWarning("EventBusRabbitMqQueue Publish: {0}!", "no connection");
                    return;
                }
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .Or<Exception>()
                .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    this.Logger?.LogWarning(ex, "Publish: ");
                });

            using (var channel = await this.PersistentConnection.CreateModel().ConfigureAwait(false))
            {
                if (channel != null)
                {
                    var routingKey = @event.RoutingKey;
                    if (this.EventBusParameters?.ExchangeDeclareParameters != null)
                    {
                        await channel.ExchangeDeclareAsync(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeDurable,
                            this.EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete).ConfigureAwait(false);

                        using (var ms = new MemoryStream())
                        {
                            Serializer.Serialize(ms, @event);
                            var body = ms.ToArray();

                            await policy.Execute(async () =>
                            {
                                //var properties = channel.CreateBasicProperties();
                                //var properties = BasicProperties();
                                //properties.DeliveryMode = 1; //2 = persistent, write on disk

                                var props = new BasicProperties
                                {
                                    DeliveryMode = DeliveryModes.Transient
                                };

                                await channel.BasicPublishAsync(this.EventBusParameters.ExchangeDeclareParameters.ExchangeName,
                                    routingKey,
                                    true,
                                    props, body).ConfigureAwait(false);
                            }).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        #region [Subscribe]

        public async ValueTask<bool> SubscribeQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>
        {
            var eventName = this.SubsManager?.GetEventKey<TIntegrationEvent>();
            var internalSubscriptionResult = await this.DoInternalSubscription(eventName + "." + routingKey).ConfigureAwait(false);
            if (internalSubscriptionResult)
            {
                this.SubsManager?.AddSubscriptionQueue<TIntegrationEvent, TIntegrationQueueHandler>(eventName + "." + routingKey);
                return await this.StartBasicConsumeAsync<TIntegrationEvent>().ConfigureAwait(false);
            }
            return false;
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>
        {
            this.SubsManager?.RemoveSubscriptionQueue<TIntegrationEvent, TIntegrationQueueHandler>(routingKey);
        }

        #endregion

        protected override async ValueTask ProcessEventAsync<TIntegrationEvent>(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            if (this.SubsManager is null)
            {
                return;
            }

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
                                        var integrationEvent = Serializer.Deserialize<TIntegrationEvent>(ms);
                                        var concreteType =
                                            typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);

                                        var enqueue = concreteType.GetMethod("Enqueue");
                                        if (enqueue != null)
                                        {
                                            await ((ValueTask<bool>)enqueue
                                                    .Invoke(this.EventHandler, new[] {(object)integrationEvent, cancel}))
                                                .ConfigureAwait(false);
                                        }
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
