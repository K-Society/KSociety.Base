using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus;

public interface IEventBusTyped : IEventBus
{
    void Subscribe<T, TH>(string routingKey)
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    void Unsubscribe<T, TH>(string routingKey)
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}