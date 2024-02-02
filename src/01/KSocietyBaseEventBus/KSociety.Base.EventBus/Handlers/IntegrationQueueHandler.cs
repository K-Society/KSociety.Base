// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Autofac;
    using Abstractions;
    using Abstractions.Handler;
    using Microsoft.Extensions.Logging;

    ///<inheritdoc cref="IIntegrationQueueHandler{TIntegrationEvent}"/>
    public class IntegrationQueueHandler<TIntegrationEvent> : IntegrationGeneralHandler,
        IIntegrationQueueHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        protected readonly ILogger<IIntegrationQueueHandler<TIntegrationEvent>> Logger;

        public BufferBlock<TIntegrationEvent> Queue { get; }

        public bool IsEmpty => this.Queue.Count == 0;

        #region [Constructors]

        public IntegrationQueueHandler(ILoggerFactory loggerFactory = default)
            : base(loggerFactory)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationQueueHandler<TIntegrationEvent>>();
            this.Queue = new BufferBlock<TIntegrationEvent>();
        }

        public IntegrationQueueHandler(ILoggerFactory loggerFactory = default, IComponentContext componentContext = default)
            : base(loggerFactory, componentContext)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationQueueHandler<TIntegrationEvent>>();
            this.Queue = new BufferBlock<TIntegrationEvent>();
        }

        public IntegrationQueueHandler(ILogger<IIntegrationQueueHandler<TIntegrationEvent>> logger = default, IComponentContext componentContext = default)
            : base(componentContext)
        {
            this.Logger = logger;
            this.Queue = new BufferBlock<TIntegrationEvent>();
        }

        #endregion

        public virtual async ValueTask<bool> Enqueue(TIntegrationEvent @integrationEvent,
            CancellationToken cancel = default)
        {
            return await this.Queue.SendAsync(@integrationEvent, cancel).ConfigureAwait(false);
        }

        //ToDo
        //public virtual async IAsyncEnumerable<TIntegrationEvent> Dequeue(
        //    [EnumeratorCancellation] CancellationToken cancel = default)
        //{
        //    while (!cancel.IsCancellationRequested)
        //    {
        //        var result = default(TIntegrationEvent);
        //        try
        //        {
        //            result = await this.Queue.ReceiveAsync(cancel).ConfigureAwait(false);
        //        }
        //        catch (Exception ex)
        //        {
        //            this.Logger?.LogError(ex, "Dequeue: ");
        //        }

        //        if (result != null)
        //        {
        //            yield return result;

        //            //return result;
        //        }
        //    }
        //}
        //ToDo
        public virtual async IAsyncEnumerable<TIntegrationEvent> Dequeue(
            [EnumeratorCancellation] CancellationToken cancel = default)
        {
            while (!cancel.IsCancellationRequested)
            {
                var result = default(TIntegrationEvent);
                try
                {
                    result = await this.Queue.ReceiveAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.Logger?.LogError(ex, "Dequeue: ");
                }

                if (result != null)
                {
                    return (IAsyncEnumerable<TIntegrationEvent>) result;
                }
            }

            return null;
        }
    }
}
