using ProtoBuf;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event
{
    [ProtoContract]
    public class BaseTestIntegrationEventReply : KSociety.Base.EventBus.Events.IntegrationEventReply
    {
        public BaseTestIntegrationEventReply()
        {

        }

        public BaseTestIntegrationEventReply(string routingKey)
            : base(routingKey)
        {

        }
    }
}

