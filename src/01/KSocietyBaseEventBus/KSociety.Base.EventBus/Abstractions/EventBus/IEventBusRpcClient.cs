// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using System.Threading;
    using System.Threading.Tasks;
    using Handler;

    public interface IEventBusRpcClient<TIntegrationEventReply> : IEventBus
        where TIntegrationEventReply : IIntegrationEventReply, new()
    {
        bool Initialize(bool asyncMode = true, CancellationToken cancel = default);

        Task<TIntegrationEventReply> CallAsync<TIntegrationEventRpc>(TIntegrationEventRpc @event, CancellationToken cancellationToken = default)
            where TIntegrationEventRpc : IIntegrationEventRpc, new();

        IIntegrationRpcClientHandler<TIntegrationEventReply> GetIntegrationRpcClientHandler();

        ValueTask<bool> SubscribeRpcClient<TIntegrationEventHandler>(string replyRoutingKey, bool asyncMode = true)
            where TIntegrationEventHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>;

        void UnsubscribeRpcClient<TIntegrationEventHandler>(string routingKey)
            where TIntegrationEventHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>;
    }
}
