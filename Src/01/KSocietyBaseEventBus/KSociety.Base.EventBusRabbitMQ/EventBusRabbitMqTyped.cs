﻿using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace KSociety.Base.EventBusRabbitMQ
{
    public sealed class EventBusRabbitMqTyped : EventBusRabbitMq, IEventBusTyped
    {
        #region [Constructor]

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            //IExchangeDeclareParameters exchangeDeclareParameters,
            //IQueueDeclareParameters queueDeclareParameters,
            string queueName = null,
            CancellationToken cancel = default)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters,/*exchangeDeclareParameters, queueDeclareParameters,*/ queueName, cancel)
        {

        }

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            //IExchangeDeclareParameters exchangeDeclareParameters,
            //IQueueDeclareParameters queueDeclareParameters,
            string queueName = null,
            CancellationToken cancel = default)
            : base(persistentConnection, loggerFactory, subsManager, eventBusParameters,/*exchangeDeclareParameters, queueDeclareParameters,*/ queueName, cancel)
        {

        }

        #endregion

        #region [Subscribe]

        public void Subscribe<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {

            var eventName = SubsManager.GetEventKey<T>();
            Logger.LogTrace("SubscribeTyped routing key: {0}, event name: {1}", routingKey, eventName);
            DoInternalSubscription(eventName + "." + routingKey);
            SubsManager.AddSubscription<T, TH>(eventName + "." + routingKey);
            StartBasicConsume();
        }

        #endregion

        #region [Unsubscribe]

        public void Unsubscribe<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            SubsManager.RemoveSubscription<T, TH>(routingKey);
        }

        #endregion
    }
}
