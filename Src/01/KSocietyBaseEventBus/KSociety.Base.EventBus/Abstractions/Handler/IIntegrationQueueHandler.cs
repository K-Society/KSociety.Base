using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    public interface IIntegrationQueueHandler<TIntegrationEvent> : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEvent
    {
        BufferBlock<TIntegrationEvent> Queue { get; } 

        bool IsEmpty { get; }

        ValueTask<bool> Enqueue(TIntegrationEvent @integrationEvent);
        ValueTask<bool> Enqueue(TIntegrationEvent @integrationEvent, CancellationToken cancel);

        IAsyncEnumerable<TIntegrationEvent> Dequeue([EnumeratorCancellation] CancellationToken cancel);
    }
}
