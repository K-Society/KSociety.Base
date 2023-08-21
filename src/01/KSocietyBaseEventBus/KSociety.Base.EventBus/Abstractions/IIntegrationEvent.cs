namespace KSociety.Base.EventBus.Abstractions
{
    using System;

    /// <include file='..\Doc\IntegrationEvent.xml' path='docs/members[@name="IntegrationEvent"]/IntegrationEvent/*'/>
    public interface IIntegrationEvent
    {
        Guid Id { get; set; }
        DateTime CreationDate { get; set; }
        string RoutingKey { get; set; }
    }
}