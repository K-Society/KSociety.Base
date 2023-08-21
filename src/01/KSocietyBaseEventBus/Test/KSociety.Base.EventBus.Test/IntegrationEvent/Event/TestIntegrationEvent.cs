namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event;
using ProtoBuf;

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
        this.TestName = testName;
        this.ByteArray = byteArray;
    }
}
