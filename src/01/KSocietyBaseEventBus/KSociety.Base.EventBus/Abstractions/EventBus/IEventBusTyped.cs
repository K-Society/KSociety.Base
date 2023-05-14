using KSociety.Base.EventBus.Abstractions.Handler;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    public interface IEventBusTyped : IEventBus
    {
        ValueTask Subscribe<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}