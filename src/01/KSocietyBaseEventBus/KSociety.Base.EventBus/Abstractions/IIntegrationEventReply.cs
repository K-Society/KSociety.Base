namespace KSociety.Base.EventBus.Abstractions
{
    using System;

    /// <include file='..\Doc\IntegrationEventReply.xml' path='docs/members[@name="IntegrationEventReply"]/IntegrationEventReply/*'/>
    public interface IIntegrationEventReply
    {
        Guid Id { get; set; }
        DateTime CreationDate { get; set; }
        string RoutingKey { get; set; }
    }
}