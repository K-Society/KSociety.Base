using System.Threading;
using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBusRabbitMQ
{
    public sealed class EventBusRabbitMqTyped : EventBusRabbitMq, IEventBusTyped
    {
        #region [Constructor]

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters,
            string queueName)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, exchangeDeclareParameters, queueDeclareParameters, queueName)
        {

            ConsumerChannel = CreateConsumerChannel();
        }

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters,
            string queueName,
            CancellationToken cancel)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, exchangeDeclareParameters, queueDeclareParameters, queueName, cancel)
        {
            
            ConsumerChannel = CreateConsumerChannel(cancel);
        }

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters,
            string queueName)
            : base(persistentConnection, loggerFactory, subsManager, exchangeDeclareParameters, queueDeclareParameters, queueName)
        {

            ConsumerChannel = CreateConsumerChannel();
        }

        public EventBusRabbitMqTyped(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters,
            string queueName,
            CancellationToken cancel)
            : base(persistentConnection, loggerFactory, subsManager, exchangeDeclareParameters, queueDeclareParameters, queueName, cancel)
        {

            ConsumerChannel = CreateConsumerChannel(cancel);
        }

        #endregion

        #region [Subscribe]

        public void Subscribe<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {

            var eventName = SubsManager.GetEventKey<T>();
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
