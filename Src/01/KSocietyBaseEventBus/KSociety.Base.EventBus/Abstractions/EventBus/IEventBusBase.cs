using KSociety.Base.EventBus.Abstractions.Handler;
//using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    public interface IEventBusBase //: IAsyncInitialization
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
