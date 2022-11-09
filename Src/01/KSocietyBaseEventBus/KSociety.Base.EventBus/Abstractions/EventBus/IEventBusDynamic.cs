using KSociety.Base.EventBus.Abstractions.Handler;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.EventBus;

public interface IEventBusDynamic
{
    ValueTask SubscribeDynamic<TH>(string routingKey)
        where TH : IDynamicIntegrationEventHandler;

    void UnsubscribeDynamic<TH>(string routingKey)
        where TH : IDynamicIntegrationEventHandler;
}