using ProtoBuf;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event;

[ProtoContract]
public class BaseTestIntegrationEvent : KSociety.Base.EventBus.Events.IntegrationEvent
{
    public BaseTestIntegrationEvent()
    {

    }

    public BaseTestIntegrationEvent(string routingKey)
        : base(routingKey)
    {

    }
}