// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using Autofac;
    using EventBus;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class EventBusRabbitMqDynamic : EventBusRabbitMq, IEventBusDynamic
    {
        private readonly ILifetimeScope _autofac;
        private const string AutofacScopeName = "k-society_event_bus";

        #region [Constructor]

        public EventBusRabbitMqDynamic(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            ILifetimeScope autofac, IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {
            this._autofac = autofac;
        }

        public EventBusRabbitMqDynamic(IRabbitMqPersistentConnection persistentConnection,
            ILifetimeScope autofac, IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {
            this._autofac = autofac;
        }

        #endregion

        #region [Subscribe]

        public async ValueTask SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            await this.DoInternalSubscription(eventName).ConfigureAwait(false);
            this.SubsManager?.AddDynamicSubscription<TH>(eventName);
            await this.StartBasicConsume().ConfigureAwait(false);
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            this.SubsManager?.RemoveDynamicSubscription<TH>(eventName);
        }

        #endregion

        protected override async ValueTask ProcessEvent(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            if (this.SubsManager is null)
            {
                return;
            }

            if (this.SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                if (this._autofac != null)
                {
                    using (var scope = this._autofac.BeginLifetimeScope(AutofacScopeName))
                    {
                        var subscriptions = this.SubsManager.GetHandlersForEvent(routingKey);
                        foreach (var subscription in subscriptions)
                        {
                            //if (subscription.IsDynamic)
                            //{
                            //    var handler =
                            //        scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;

                            //    //ToDo
                            //    //dynamic eventData = JObject.Parse(message);

                            //    //await using var ms = new MemoryStream(message);
                            //    //var integrationEvent = Serializer.Deserialize(eventType, ms);

                            //    //await handler.Handle(eventData);
                            //}
                            //else
                            //{
                            //    var eventType = _subsManager.GetEventTypeByName(routingKey);
                            //    //var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                            //    await using var ms = new MemoryStream(message);
                            //    var integrationEvent = Serializer.Deserialize(eventType, ms);

                            //    var handler = scope.ResolveOptional(subscription.HandlerType);
                            //    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            //    await (ValueTask) concreteType.GetMethod("Handle")
                            //        .Invoke(handler, new[] {integrationEvent});
                            //}

                            switch (subscription.SubscriptionManagerType)
                            {
                                //case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Dynamic:
                                //    var handler =
                                //        scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;

                                //    //ToDo
                                //    //dynamic eventData = JObject.Parse(message);

                                //    await using var ms = new MemoryStream(message);
                                //    var integrationEvent = Serializer.Deserialize(eventType, ms);

                                //    await handler.Handle(eventData);
                                //    break;

                                //default:
                                //    {
                                //        var eventType = _subsManager.GetEventTypeByName(routingKey);
                                //        //var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                                //        await using var ms = new MemoryStream(message);
                                //        var integrationEvent = Serializer.Deserialize(eventType, ms);

                                //        var handler = scope.ResolveOptional(subscription.HandlerType);
                                //        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                                //        await (ValueTask)concreteType.GetMethod("Handle")
                                //            .Invoke(handler, new[] { integrationEvent, cancel });
                                //        break;
                                //    }
                            }
                        }
                    }
                }
            }
        } //ProcessEvent.
    }
}
