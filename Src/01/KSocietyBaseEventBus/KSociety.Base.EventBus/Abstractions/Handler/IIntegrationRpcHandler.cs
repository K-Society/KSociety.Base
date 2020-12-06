using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
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
