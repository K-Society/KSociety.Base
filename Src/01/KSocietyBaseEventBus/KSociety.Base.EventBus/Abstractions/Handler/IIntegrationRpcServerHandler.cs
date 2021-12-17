using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler;

public interface IIntegrationRpcServerHandler<in TIntegrationEvent, TIntegrationEventReply>
    : IIntegrationGeneralHandler
    where TIntegrationEvent : IIntegrationEventRpc
    where TIntegrationEventReply : IIntegrationEventReply
{
    TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel = default);

    ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event, CancellationToken cancel = default);
}