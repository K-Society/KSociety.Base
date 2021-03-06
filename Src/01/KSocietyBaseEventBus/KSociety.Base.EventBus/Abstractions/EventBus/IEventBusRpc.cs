﻿using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    public interface IEventBusRpc : IEventBus
    {
        IIntegrationRpcHandler<T, TR> GetIntegrationRpcHandler<T, TR>()
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply;

        void SubscribeRpc<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>;

        void UnsubscribeRpc<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationRpcHandler<T, TR>
            where TR : IIntegrationEventReply;
    }
}
