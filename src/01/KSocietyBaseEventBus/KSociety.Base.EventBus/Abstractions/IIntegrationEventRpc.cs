// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions
{
    /// <include file='..\Doc\IntegrationEventRpc.xml' path='docs/members[@name="IntegrationEventRpc"]/IntegrationEventRpc/*'/>
    public interface IIntegrationEventRpc : IIntegrationEvent
    {
        string ReplyRoutingKey { get; set; }
    }
}