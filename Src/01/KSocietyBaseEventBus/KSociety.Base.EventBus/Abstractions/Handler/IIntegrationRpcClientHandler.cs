using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    /// <include file='..\..\Doc\Handler\IntegrationRpcClientHandler.xml' path='docs/members[@name="IntegrationRpcClientHandler"]/IntegrationRpcClientHandler/*'/>
    public interface IIntegrationRpcClientHandler<in TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEventReply : IIntegrationEventReply
    {
        void HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);

        ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);
    }
}