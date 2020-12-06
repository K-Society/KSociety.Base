using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    public interface IEventBusRpcClient : IEventBus
    {
        //Task CallAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);

        Task<TIntegrationEventReply> CallAsync<TIntegrationEventReply>(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        //Task<TR> CallAsync<TR>(IIntegrationEvent @event, CancellationToken cancellationToken = default);
        where TIntegrationEventReply : IIntegrationEventReply;

        IIntegrationRpcClientHandler<TIntegrationEventReply> GetIntegrationRpcClientHandler<TIntegrationEventReply>()
        //IIntegrationRpcClientHandler<TR> GetIntegrationRpcClientHandler<TR>();
        where TIntegrationEventReply : IIntegrationEventReply;


        void SubscribeRpcClient<TIntegrationEventReply, TH>(/*string routingKey,*/ string replyRoutingKey)
            //where T : IIntegrationEventRpc
            where TIntegrationEventReply : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>;
        //void SubscribeRpcClient<TR, TH>(/*string routingKey,*/ string replyRoutingKey)
        //where T : IIntegrationEventRpc
        //where TR : IIntegrationEventReply
        //   where TH : IIntegrationRpcClientHandler<TR>;

        void UnsubscribeRpcClient<TIntegrationEventReply, TH>(string routingKey)
            where TIntegrationEventReply : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TIntegrationEventReply>;
        //void UnsubscribeRpcClient<TR, TH>(string routingKey)
        //    //where TR : IIntegrationEventReply
        //    where TH : IIntegrationRpcClientHandler<TR>;
    }
}
