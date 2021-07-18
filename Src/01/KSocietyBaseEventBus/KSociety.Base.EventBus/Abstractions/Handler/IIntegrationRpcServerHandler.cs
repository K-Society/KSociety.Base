using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    public interface IIntegrationRpcServerHandler<in TIntegrationEvent, TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        TIntegrationEventReply HandleRpc(TIntegrationEvent @event);
        TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel);

        ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event);
        ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event, CancellationToken cancel);
    }
}
