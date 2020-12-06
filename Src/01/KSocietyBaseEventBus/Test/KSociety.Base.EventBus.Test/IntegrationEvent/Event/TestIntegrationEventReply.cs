using ProtoBuf;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event
{
    [ProtoContract]
    public class TestIntegrationEventReply : BaseTestIntegrationEventReply
    {
        [ProtoMember(1)]
        public string TestName { get; set; }

        [ProtoMember(2)]
        public byte[] ByteArray { get; set; }

        public TestIntegrationEventReply()
        {

        }

        public TestIntegrationEventReply(
            string routingKey, 
            string testName,
            byte[] byteArray)
        :base(routingKey)
        {
            TestName = testName;
            ByteArray = byteArray;
        }
    }
}
