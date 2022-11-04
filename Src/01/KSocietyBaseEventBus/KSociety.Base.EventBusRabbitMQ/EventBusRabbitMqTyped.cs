using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KSociety.Base.EventBusRabbitMQ;

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

    #endregion

    #region [Subscribe]

    public async ValueTask Subscribe<T, TH>(string routingKey)
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {

        var eventName = SubsManager.GetEventKey<T>();
        Logger.LogTrace("SubscribeTyped routing key: {0}, event name: {1}", routingKey, eventName);
        await DoInternalSubscription(eventName + "." + routingKey).ConfigureAwait(false);
        SubsManager.AddSubscription<T, TH>(eventName + "." + routingKey);
        await StartBasicConsume().ConfigureAwait(false);
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