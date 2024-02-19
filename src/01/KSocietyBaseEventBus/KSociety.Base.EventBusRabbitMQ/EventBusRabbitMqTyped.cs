// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using EventBus;
    using EventBus.Abstractions;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using EventBus.Abstractions.Handler;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public sealed class EventBusRabbitMqTyped : EventBusRabbitMq, IEventBusTyped
    {
        #region [Constructor]

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
        {

        }

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null)
            : base(persistentConnection, loggerFactory, subsManager, eventBusParameters, queueName)
        {

        }

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default)
            : base(persistentConnection, eventHandler, subsManager, eventBusParameters, queueName, logger)
        {

        }

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionsManager subsManager,
            IEventBusParameters eventBusParameters,
            string queueName = null, ILogger<EventBusRabbitMq> logger = default)
            : base(persistentConnection, subsManager, eventBusParameters, queueName, logger)
        {

        }

        #endregion

        #region [Subscribe]

        public async ValueTask Subscribe<TIntegrationEvent, TIntegrationEventHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {

            var eventName = this.SubsManager?.GetEventKey<TIntegrationEvent>();
            //this.Logger?.LogTrace("SubscribeTyped routing key: {0}, event name: {1}", routingKey, eventName);
            await this.DoInternalSubscription(eventName + "." + routingKey).ConfigureAwait(false);
            this.SubsManager?.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>(eventName + "." + routingKey);
            await this.StartBasicConsume().ConfigureAwait(false);
        }

        #endregion

        #region [Unsubscribe]

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            this.SubsManager?.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>(routingKey);
        }

        #endregion
    }
}
