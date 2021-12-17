namespace KSociety.Base.EventBus.Abstractions;

public interface IIntegrationEventRpc : IIntegrationEvent
{
    string ReplyRoutingKey { get; set; }
}