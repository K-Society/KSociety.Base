using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    public class IntegrationEventHandler<TIntegrationEvent> : IntegrationGeneralHandler,
        IIntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
    {
        protected readonly ILogger<IntegrationEventHandler<TIntegrationEvent>>? Logger;

        public IntegrationEventHandler(ILoggerFactory loggerFactory, IComponentContext componentContext) 
            : base(loggerFactory, componentContext)
        {
            Logger = LoggerFactory?.CreateLogger<IntegrationEventHandler<TIntegrationEvent>>();
        }

        public IntegrationEventHandler(ILogger<IntegrationEventHandler<TIntegrationEvent>> logger, IComponentContext componentContext)
            : base(componentContext)
        {
            Logger = logger;
        }

        public virtual ValueTask Handle(TIntegrationEvent @event, CancellationToken cancel = default)
        {
            return new ValueTask();
        }
    }
}