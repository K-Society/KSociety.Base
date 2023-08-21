namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event;
using ProtoBuf;

[ProtoContract]
public class BaseTestIntegrationEventRpc : Events.IntegrationEventRpc
{
    public BaseTestIntegrationEventRpc()
    {

    }

    public BaseTestIntegrationEventRpc(string routingKey, string replyRoutingKey)
        : base(routingKey, replyRoutingKey)
    {

    }
}
