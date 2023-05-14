﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    ///<inheritdoc cref="IIntegrationQueueHandler{TIntegrationEvent}"/>
    public class IntegrationQueueHandler<TIntegrationEvent> : IntegrationGeneralHandler,
        IIntegrationQueueHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        protected readonly ILogger<IIntegrationQueueHandler<TIntegrationEvent>>? Logger;

        public BufferBlock<TIntegrationEvent> Queue { get; }

        public bool IsEmpty => Queue.Count == 0;

        #region [Constructors]

        public IntegrationQueueHandler(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            Logger = LoggerFactory?.CreateLogger<IIntegrationQueueHandler<TIntegrationEvent>>();
            Queue = new BufferBlock<TIntegrationEvent>();
        }

        public IntegrationQueueHandler(ILoggerFactory loggerFactory, IComponentContext componentContext)
            : base(loggerFactory, componentContext)
        {
            Logger = LoggerFactory?.CreateLogger<IIntegrationQueueHandler<TIntegrationEvent>>();
            Queue = new BufferBlock<TIntegrationEvent>();
        }

        public IntegrationQueueHandler(ILogger<IIntegrationQueueHandler<TIntegrationEvent>> logger, IComponentContext componentContext)
            : base(componentContext)
        {
            Logger = logger;
            Queue = new BufferBlock<TIntegrationEvent>();
        }

        #endregion

        public virtual async ValueTask<bool> Enqueue(TIntegrationEvent @integrationEvent,
            CancellationToken cancel = default)
        {
            return await Queue.SendAsync(@integrationEvent, cancel).ConfigureAwait(false);
        }


        public virtual async IAsyncEnumerable<TIntegrationEvent> Dequeue(
            [EnumeratorCancellation] CancellationToken cancel = default)
        {
            while (!cancel.IsCancellationRequested)
            {
                var result = default(TIntegrationEvent);
                try
                {
                    result = await Queue.ReceiveAsync(cancel).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Dequeue: ");
                }

                if (result != null)
                {
                    yield return result;
                }
            }
        }
    }
}