using System;
using KSociety.Base.EventBus.Abstractions;
using ProtoBuf;

namespace KSociety.Base.EventBus.Events;

[ProtoContract]
public abstract class IntegrationEventReply : IIntegrationEventReply
{
    [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
    public Guid Id { get; set; }

    [ProtoMember(2), CompatibilityLevel(CompatibilityLevel.Level200)]
    public DateTime CreationDate { get; set; }

    [ProtoMember(3)]
    public string RoutingKey { get; set; }

    public IntegrationEventReply()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
        RoutingKey = GetType().Name;
    }

    public IntegrationEventReply(string routingKey)
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
        RoutingKey = GetType().Name + "." + routingKey;
    }

    public string GetTypeName()
    {
        string[] result = RoutingKey.Split('.');
        return result.Length > 1 ? result[0] : RoutingKey;
    }
}