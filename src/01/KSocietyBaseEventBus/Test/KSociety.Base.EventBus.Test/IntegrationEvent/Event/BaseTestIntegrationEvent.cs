// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.IntegrationEvent.Event
{
    using ProtoBuf;

    [ProtoContract]
    public class BaseTestIntegrationEvent : KSociety.Base.EventBus.Events.IntegrationEvent
    {
        public BaseTestIntegrationEvent()
        {

        }

        public BaseTestIntegrationEvent(string routingKey)
            : base(routingKey)
        {

        }
    }
}
