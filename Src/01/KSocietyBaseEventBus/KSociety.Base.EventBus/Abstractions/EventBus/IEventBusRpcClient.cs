using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus;

public interface IEventBusRpcClient : IEventBus
{
    Task<TIntegrationEventReply> CallAsync<TIntegrationEventReply>(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        where TIntegrationEventReply : IIntegrationEventReply;

    IIntegrationRpcClientHandler<TIntegrationEventReply> GetIntegrationRpcClientHandler<TIntegrationEventReply>()
        where TIntegrationEventReply : IIntegrationEventReply;

    void SubscribeRpcClient<TIntegrationEventReply, TH>(string replyRoutingKey)
        where TIntegrationEventReply : IIntegrationEventReply
        where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>;

    void UnsubscribeRpcClient<TIntegrationEventReply, TH>(string routingKey)
        where TIntegrationEventReply : IIntegrationEventReply
        where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>;
}