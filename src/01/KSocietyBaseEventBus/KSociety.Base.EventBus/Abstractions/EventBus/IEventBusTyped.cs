namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading.Tasks;

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
