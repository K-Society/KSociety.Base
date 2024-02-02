// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event
{
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
}
