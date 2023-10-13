// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Events
{
    using System;
    using Abstractions;
    using ProtoBuf;

    ///<inheritdoc cref="IIntegrationEventRpc"/>
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
            this.Id = Guid.NewGuid();
            this.CreationDate = DateTime.UtcNow;
            this.RoutingKey = this.GetType().Name;
            this.ReplyRoutingKey = this.GetType().Name + ".Reply";
        }

        public IntegrationEventRpc(string routingKey, string replyRoutingKey)
        {
            this.Id = Guid.NewGuid();
            this.CreationDate = DateTime.UtcNow;
            this.RoutingKey = this.GetType().Name + "." + routingKey;
            this.ReplyRoutingKey = replyRoutingKey;
        }

        public string GetTypeName()
        {
            var result = this.RoutingKey.Split('.');
            return result.Length > 1 ? result[0] : this.RoutingKey;
        }
    }
}
