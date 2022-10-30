using KSociety.Base.EventBus.Abstractions.Handler;
using System.Threading;

namespace KSociety.Base.EventBus.Abstractions.EventBus;

public interface IEventBusBase
{
    IIntegrationGeneralHandler EventHandler { get; }

    void Initialize(CancellationToken cancel = default);

    void Subscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    void Unsubscribe<T, TH>()
        where T : IIntegrationEvent
        where TH : IIntegrationEventHandler<T>;
}