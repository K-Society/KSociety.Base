using Autofac;
using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBusRabbitMQ
{
    public sealed class EventBusRabbitMqDynamic : EventBusRabbitMq, IEventBusDynamic
    {
        private readonly ILifetimeScope _autofac;
        private const string AutofacScopeName = "k-society_event_bus";

        #region [Constructor]

        public EventBusRabbitMqDynamic(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            ILifetimeScope autofac, IIntegrationGeneralHandler? eventHandler, IEventBusSubscriptionsManager? subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {
            _autofac = autofac;
        }

        public EventBusRabbitMqDynamic(IRabbitMqPersistentConnection persistentConnection,
            ILifetimeScope autofac, IIntegrationGeneralHandler? eventHandler, IEventBusSubscriptionsManager? subsManager,
            IEventBusParameters eventBusParameters,
            string? queueName = null, ILogger<EventBusRabbitMqDynamic>? logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {
            _autofac = autofac;
        }

        #endregion

        #region [Subscribe]

        public async ValueTask SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            await DoInternalSubscription(eventName).ConfigureAwait(false);
            SubsManager.AddDynamicSubscription<TH>(eventName);
            await StartBasicConsume().ConfigureAwait(false);
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            SubsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        #endregion

        protected override async ValueTask ProcessEvent(string routingKey, string eventName,
            ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            if (SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                if (_autofac != null)
                {
                    using (var scope = _autofac.BeginLifetimeScope(AutofacScopeName))
                    {
                        var subscriptions = SubsManager.GetHandlersForEvent(routingKey);
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