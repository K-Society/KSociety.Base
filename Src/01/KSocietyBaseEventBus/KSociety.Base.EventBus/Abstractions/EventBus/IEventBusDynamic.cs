using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    public interface IEventBusDynamic
    {
        void SubscribeDynamic<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;
    }
}
