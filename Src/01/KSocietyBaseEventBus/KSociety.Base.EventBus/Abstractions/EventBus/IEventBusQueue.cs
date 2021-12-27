using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus;

public interface IEventBusQueue : IEventBus
{
    IIntegrationQueueHandler<T> GetIntegrationQueueHandler<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationQueueHandler<T>;

    void SubscribeQueue<T, TH>(string routingKey)
        where T : IIntegrationEvent
        where TH : IIntegrationQueueHandler<T>;

    void UnsubscribeQueue<T, TH>(string routingKey)
        where T : IIntegrationEvent
        where TH : IIntegrationQueueHandler<T>;
}