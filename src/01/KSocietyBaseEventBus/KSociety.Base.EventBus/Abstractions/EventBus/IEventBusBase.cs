namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEventBusBase
    {
        IIntegrationGeneralHandler? EventHandler { get; }

        void Initialize(CancellationToken cancel = default);

        ValueTask Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
