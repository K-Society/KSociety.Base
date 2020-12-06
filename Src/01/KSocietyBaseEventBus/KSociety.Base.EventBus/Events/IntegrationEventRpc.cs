using System;
using KSociety.Base.EventBus.Abstractions;
using ProtoBuf;

namespace KSociety.Base.EventBus.Events
{
    [ProtoContract]
    public class IntegrationEventRpc : IIntegrationEventRpc
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        [ProtoMember(2), CompatibilityLevel(CompatibilityLevel.Level200)]
        public DateTime CreationDate { get; set; }

        [ProtoMember(3)]
        public string RoutingKey { get; set; }

        [ProtoMember(4)]
        public string ReplyRoutingKey { get; set; }

        public IntegrationEventRpc()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            RoutingKey = GetType().Name;
            ReplyRoutingKey = GetType().Name + ".Reply";
        }

        public IntegrationEventRpc(string routingKey, string replyRoutingKey)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            RoutingKey = GetType().Name + "." + routingKey;
            ReplyRoutingKey = /*GetType().Name + "." +*/ replyRoutingKey;
        }

        public string GetTypeName()
        {
            string[] result = RoutingKey.Split('.');
            return result.Length > 1 ? result[0] : RoutingKey;
        }
    }
}
