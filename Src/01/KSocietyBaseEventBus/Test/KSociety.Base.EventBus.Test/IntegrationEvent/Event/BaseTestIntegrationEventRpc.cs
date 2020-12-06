using ProtoBuf;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event
{
    [ProtoContract]
    public class BaseTestIntegrationEventRpc : KSociety.Base.EventBus.Events.IntegrationEventRpc
    {
        public BaseTestIntegrationEventRpc()
        {

        }

        public BaseTestIntegrationEventRpc(string routingKey, string replyRoutingKey)
            : base(routingKey, replyRoutingKey)
        {

        }
    }
}
