using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
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
            Logger = LoggerFactory?.CreateLogger<IntegrationEventHandler<TIntegrationEvent>>();
        }

        public IntegrationEventHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default) 
            : base(loggerFactory, componentContext)
        {
            Logger = LoggerFactory?.CreateLogger<IntegrationEventHandler<TIntegrationEvent>>();
        }

        public IntegrationEventHandler(ILogger<IntegrationEventHandler<TIntegrationEvent>>? logger = default, IComponentContext? componentContext = default)
            : base(componentContext)
        {
            Logger = logger;
        }

        #endregion

        public virtual ValueTask Handle(TIntegrationEvent @event, CancellationToken cancel = default)
        {
            return new ValueTask();
        }
    }
}