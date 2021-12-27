using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler;

public interface IIntegrationRpcClientHandler<in TIntegrationEventReply>
    : IIntegrationGeneralHandler
    where TIntegrationEventReply : IIntegrationEventReply
{
    void HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);
    ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply, CancellationToken cancel = default);
}