namespace KSociety.Base.EventBus.Abstractions.Handler
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <include file='..\..\Doc\Handler\IntegrationRpcServerHandler.xml' path='docs/members[@name="IntegrationRpcServerHandler"]/IntegrationRpcServerHandler/*'/>
    public interface IIntegrationRpcServerHandler<in TIntegrationEvent, TIntegrationEventReply>
        : IIntegrationGeneralHandler
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel = default);

        ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event, CancellationToken cancel = default);
    }
}