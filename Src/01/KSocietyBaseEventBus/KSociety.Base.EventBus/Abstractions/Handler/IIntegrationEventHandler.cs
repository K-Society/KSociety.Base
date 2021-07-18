using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationGeneralHandler 
        where TIntegrationEvent : IIntegrationEvent
    {
        ValueTask Handle(TIntegrationEvent @event);
        ValueTask Handle(TIntegrationEvent @event, CancellationToken cancel);
    }
}
