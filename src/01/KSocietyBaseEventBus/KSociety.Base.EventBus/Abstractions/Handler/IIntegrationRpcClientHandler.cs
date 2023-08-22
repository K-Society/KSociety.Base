// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <include file='..\..\Doc\Handler\IntegrationRpcClientHandler.xml' path='docs/members[@name="IntegrationRpcClientHandler"]/IntegrationRpcClientHandler/*'/>
    public interface IIntegrationRpcClientHandler<in TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEventReply : IIntegrationEventReply
    {
        void HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);
    }
}