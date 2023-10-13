// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading.Tasks;

    public interface IEventBusRpc : IEventBus
    {
        IIntegrationRpcHandler<T, TR>? GetIntegrationRpcHandler<T, TR>()
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply;

        ValueTask SubscribeRpc<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>;

        void UnsubscribeRpc<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationRpcHandler<T, TR>
            where TR : IIntegrationEventReply;
    }
}
