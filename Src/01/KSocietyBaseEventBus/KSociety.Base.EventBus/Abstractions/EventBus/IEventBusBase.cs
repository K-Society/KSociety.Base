using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    public interface IEventBusBase
    {
        IIntegrationGeneralHandler EventHandler { get; }

        void Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
