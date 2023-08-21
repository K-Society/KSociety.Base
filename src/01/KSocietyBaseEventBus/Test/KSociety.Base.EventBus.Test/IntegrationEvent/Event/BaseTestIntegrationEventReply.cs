namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event;
using ProtoBuf;

[ProtoContract]
public class BaseTestIntegrationEventReply : Events.IntegrationEventReply
{
    public BaseTestIntegrationEventReply()
    {

    }

    public BaseTestIntegrationEventReply(string routingKey)
        : base(routingKey)
    {

    }
}
