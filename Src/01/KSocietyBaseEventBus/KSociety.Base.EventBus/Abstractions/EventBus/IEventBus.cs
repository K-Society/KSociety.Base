using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.EventBus;

public interface IEventBus : IEventBusBase
{
    ValueTask Publish(IIntegrationEvent @event);
}