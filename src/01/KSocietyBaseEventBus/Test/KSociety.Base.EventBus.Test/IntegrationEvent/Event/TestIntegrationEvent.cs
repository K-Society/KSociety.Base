using ProtoBuf;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event;

[ProtoContract]
public class TestIntegrationEvent : BaseTestIntegrationEvent
{
    [ProtoMember(1)]
    public string TestName { get; set; }

    [ProtoMember(2)]
    public byte[] ByteArray { get; set; }

    public TestIntegrationEvent() { }

    public TestIntegrationEvent(
        string routingKey,
        string testName,
        byte[] byteArray
    )
        : base(routingKey)
    {

        TestName = testName;
        ByteArray = byteArray;
    }
}