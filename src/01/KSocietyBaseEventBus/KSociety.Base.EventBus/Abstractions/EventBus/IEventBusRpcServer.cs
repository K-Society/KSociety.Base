// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using System.Threading;
    using System.Threading.Tasks;
    using Handler;

    public interface IEventBusRpcServer : IEventBusBase
    {
        bool InitializeServer<TIntegrationEventRpc, TIntegrationEventReply>(CancellationToken cancel = default)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new();

        IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply> GetIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>()
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new();

        ValueTask<bool> SubscribeRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>;

        void UnsubscribeRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>;

        //ValueTask<uint> QueueReplyPurge(CancellationToken cancellationToken = default);
    }
}
