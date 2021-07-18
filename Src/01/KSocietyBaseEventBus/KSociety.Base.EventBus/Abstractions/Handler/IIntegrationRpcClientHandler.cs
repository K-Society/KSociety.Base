using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    public interface IIntegrationRpcClientHandler<in TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEventReply : IIntegrationEventReply
    {
        void HandleReply(TIntegrationEventReply @integrationEventReply);
        void HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel);
        ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply);
        ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply, CancellationToken cancel);
    }
}
