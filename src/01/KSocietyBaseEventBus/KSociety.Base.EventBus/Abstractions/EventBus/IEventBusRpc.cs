// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using System.Threading.Tasks;
    using Handler;

    public interface IEventBusRpc : IEventBus
    {
        IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply> GetIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new();

        ValueTask<bool> SubscribeRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>;

        void UnsubscribeRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>;

        //ValueTask<uint> QueueReplyPurge(CancellationToken cancellationToken = default);
    }
}
