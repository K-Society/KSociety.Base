using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    /// <include file='..\..\Doc\Handler\IntegrationEventHandler.xml' path='docs/members[@name="IntegrationEventHandler"]/IntegrationEventHandler/*'/>
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEvent
    {
        ValueTask Handle(TIntegrationEvent @event, CancellationToken cancel = default);
    }
}