namespace KSociety.Base.EventBus.Abstractions
{
    /// <include file='..\Doc\IntegrationEventRpc.xml' path='docs/members[@name="IntegrationEventRpc"]/IntegrationEventRpc/*'/>
    public interface IIntegrationEventRpc : IIntegrationEvent
    {
        string ReplyRoutingKey { get; set; }
    }
}