// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Abstractions;
    using Abstractions.Handler;
    using Microsoft.Extensions.Logging;

    ///<inheritdoc cref="IIntegrationEventHandler{TIntegrationEvent}"/>
    public class IntegrationEventHandler<TIntegrationEvent> : IntegrationGeneralHandler,
        IIntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        protected readonly ILogger<IntegrationEventHandler<TIntegrationEvent>>? Logger;

        #region [Constructors]

        public IntegrationEventHandler(ILoggerFactory? loggerFactory = default)
            : base(loggerFactory)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IntegrationEventHandler<TIntegrationEvent>>();
        }

        public IntegrationEventHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default) 
            : base(loggerFactory, componentContext)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IntegrationEventHandler<TIntegrationEvent>>();
        }

        public IntegrationEventHandler(ILogger<IntegrationEventHandler<TIntegrationEvent>>? logger = default, IComponentContext? componentContext = default)
            : base(componentContext)
        {
            this.Logger = logger;
        }

        #endregion

        public virtual ValueTask Handle(TIntegrationEvent @event, CancellationToken cancel = default)
        {
            return new ValueTask();
        }
    }
}
