using ProtoBuf;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event
{
    [ProtoContract]
    public class TestIntegrationEventRpc : BaseTestIntegrationEventRpc
    {
        [ProtoMember(1)]
        public string TestName { get; set; }

        [ProtoMember(2)]
        public byte[] ByteArray { get; set; }

        public TestIntegrationEventRpc() { }

        public TestIntegrationEventRpc(
            string routingKey,
            string replyRoutingKey,
            string testName,
            byte[] byteArray
        )
            : base(routingKey, replyRoutingKey)
        {
            
            TestName = testName;
            ByteArray = byteArray;
        }
    }
}
