// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <include file='..\..\Doc\Handler\IntegrationEventHandler.xml' path='docs/members[@name="IntegrationEventHandler"]/IntegrationEventHandler/*'/>
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEvent
    {
        ValueTask Handle(TIntegrationEvent @event, CancellationToken cancel = default);
    }
}