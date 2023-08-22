// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    /// <include file='..\..\Doc\Handler\IntegrationRpcHandler.xml' path='docs/members[@name="IntegrationRpcHandler"]/IntegrationRpcHandler/*'/>
    public interface IIntegrationRpcHandler<in TIntegrationEvent, TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEvent
        where TIntegrationEventReply : IIntegrationEventReply
    {
        BufferBlock<TIntegrationEventReply> Queue { get; }

        bool IsEmpty { get; }

        TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel = default);

        ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event, CancellationToken cancel = default);

        ValueTask HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        ValueTask<bool> Enqueue(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        ValueTask<TIntegrationEventReply> Take(CancellationToken cancel = default);
    }
}