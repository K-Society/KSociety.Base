using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    public interface IEventBusRpcServer : IEventBusBase
    {
        IIntegrationRpcServerHandler<T, TR> GetIntegrationRpcServerHandler<T, TR>()
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply;

        void SubscribeRpcServer<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>;

        void UnsubscribeRpcServer<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TH : IIntegrationRpcServerHandler<T, TR>
            where TR : IIntegrationEventReply;
    }
}
