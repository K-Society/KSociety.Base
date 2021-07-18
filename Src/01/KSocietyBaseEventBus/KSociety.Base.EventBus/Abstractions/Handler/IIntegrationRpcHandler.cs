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

        TIntegrationEventReply HandleRpc(TIntegrationEvent @event);
        TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel);

        ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event);
        ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event, CancellationToken cancel);

        ValueTask HandleReply(TIntegrationEventReply @integrationEventReply);
        ValueTask HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel);

        ValueTask<bool> Enqueue(TIntegrationEventReply @integrationEventReply);
        ValueTask<bool> Enqueue(TIntegrationEventReply @integrationEventReply, CancellationToken cancel);

        ValueTask<TIntegrationEventReply> Take();
        ValueTask<TIntegrationEventReply> Take(CancellationToken cancel);
    }
}
