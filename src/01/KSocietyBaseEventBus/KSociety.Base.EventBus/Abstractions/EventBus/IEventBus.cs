namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using System.Threading.Tasks;

    public interface IEventBus : IEventBusBase
    {
        ValueTask Publish(IIntegrationEvent @event);
    }
}