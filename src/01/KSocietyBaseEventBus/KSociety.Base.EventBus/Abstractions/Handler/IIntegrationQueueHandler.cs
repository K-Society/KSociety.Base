// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    using System.Collections.Generic;
    //using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    /// <include file='..\..\Doc\Handler\IntegrationQueueHandler.xml' path='docs/members[@name="IntegrationQueueHandler"]/IntegrationQueueHandler/*'/>
    public interface IIntegrationQueueHandler<TIntegrationEvent> : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEvent
    {
        BufferBlock<TIntegrationEvent> Queue { get; }

        bool IsEmpty { get; }

        ValueTask<bool> Enqueue(TIntegrationEvent @integrationEvent, CancellationToken cancel = default);

        //IAsyncEnumerable<TIntegrationEvent> Dequeue([EnumeratorCancellation] CancellationToken cancel = default);
        //IAsyncEnumerable<TIntegrationEvent> Dequeue(CancellationToken cancel = default);
    }
}
