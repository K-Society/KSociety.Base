using System;

namespace KSociety.Base.EventBus.Abstractions
{
    public interface IIntegrationEventReply
    {
        Guid Id { get; set; }
        DateTime CreationDate { get; set; }
        string RoutingKey { get; set; }
    }
}