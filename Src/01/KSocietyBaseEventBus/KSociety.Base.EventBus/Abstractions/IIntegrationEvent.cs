using System;

namespace KSociety.Base.EventBus.Abstractions;

public interface IIntegrationEvent
{
    Guid Id { get; set; }
    DateTime CreationDate { get; set; }
    string RoutingKey { get; set; }
}