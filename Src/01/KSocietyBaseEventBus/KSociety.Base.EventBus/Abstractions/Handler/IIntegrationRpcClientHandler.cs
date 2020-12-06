using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    public interface IIntegrationRpcClientHandler<in TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEventReply : IIntegrationEventReply
    {
        //BufferBlock<TIntegrationEventReply> Queue { get; }

        //bool IsEmpty { get; }
        void HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);
        ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        //ValueTask<bool> Enqueue(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        //ValueTask<TIntegrationEventReply> Take(CancellationToken cancel = default);
    }
}
