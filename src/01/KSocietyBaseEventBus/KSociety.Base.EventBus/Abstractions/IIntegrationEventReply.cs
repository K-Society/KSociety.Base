// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions
{
    using System;

    /// <include file='..\Doc\IntegrationEventReply.xml' path='docs/members[@name="IntegrationEventReply"]/IntegrationEventReply/*'/>
    public interface IIntegrationEventReply
    {
        Guid Id { get; set; }
        DateTime CreationDate { get; set; }
        string RoutingKey { get; set; }
    }
}