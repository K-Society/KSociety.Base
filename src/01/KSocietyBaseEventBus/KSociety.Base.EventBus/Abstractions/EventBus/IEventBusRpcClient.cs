// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using System.Threading;
    using System.Threading.Tasks;
    using Handler;

    public interface IEventBusRpcClient : IEventBus
    {
        Task<TIntegrationEventReply> CallAsync<TIntegrationEventReply>(IIntegrationEvent @event,
            CancellationToken cancellationToken = default)
            where TIntegrationEventReply : IIntegrationEventReply;

        IIntegrationRpcClientHandler<TIntegrationEventReply>? GetIntegrationRpcClientHandler<TIntegrationEventReply>()
            where TIntegrationEventReply : IIntegrationEventReply;

        ValueTask SubscribeRpcClient<TIntegrationEventReply, TH>(string replyRoutingKey)
            where TIntegrationEventReply : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>;

        void UnsubscribeRpcClient<TIntegrationEventReply, TH>(string routingKey)
            where TIntegrationEventReply : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>;
    }
}
