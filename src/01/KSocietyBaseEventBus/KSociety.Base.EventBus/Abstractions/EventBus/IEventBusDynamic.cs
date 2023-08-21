namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading.Tasks;

    public interface IEventBusDynamic
    {
        ValueTask SubscribeDynamic<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;
    }
}
