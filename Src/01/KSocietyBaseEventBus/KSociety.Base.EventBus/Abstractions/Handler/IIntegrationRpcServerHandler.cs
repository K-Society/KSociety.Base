using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    public interface IIntegrationRpcServerHandler<in TIntegrationEvent, TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        //BufferBlock<TIntegrationEventReply> Queue { get; }

        //bool IsEmpty { get; }

        TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel = default);

        ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event, CancellationToken cancel = default);

        //ValueTask HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        //ValueTask<bool> Enqueue(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        //ValueTask<TIntegrationEventReply> Take(CancellationToken cancel = default);
    }
}
