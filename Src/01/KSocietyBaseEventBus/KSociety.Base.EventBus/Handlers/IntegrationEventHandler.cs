using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers;

public class IntegrationEventHandler<TIntegrationEvent> : IntegrationGeneralHandler, IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public IntegrationEventHandler(ILoggerFactory loggerFactory, IComponentContext componentContext) : base(loggerFactory, componentContext)
    {

    }

    public virtual ValueTask Handle(TIntegrationEvent @event, CancellationToken cancel = default)
    {
        return new ValueTask();
    }
}