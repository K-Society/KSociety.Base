namespace KSociety.Base.EventBus.Events
{
    using System;
    using Abstractions;
    using ProtoBuf;

    ///<inheritdoc cref="IIntegrationEventReply"/>
    [ProtoContract]
    public abstract class IntegrationEventReply : IIntegrationEventReply
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        [ProtoMember(2), CompatibilityLevel(CompatibilityLevel.Level200)]
        public DateTime CreationDate { get; set; }

        [ProtoMember(3)] public string RoutingKey { get; set; }

        public IntegrationEventReply()
        {
            this.Id = Guid.NewGuid();
            this.CreationDate = DateTime.UtcNow;
            this.RoutingKey = this.GetType().Name;
        }

        public IntegrationEventReply(string routingKey)
        {
            this.Id = Guid.NewGuid();
            this.CreationDate = DateTime.UtcNow;
            this.RoutingKey = this.GetType().Name + "." + routingKey;
        }

        public string GetTypeName()
        {
            string[] result = this.RoutingKey.Split('.');
            return result.Length > 1 ? result[0] : this.RoutingKey;
        }
    }
}
