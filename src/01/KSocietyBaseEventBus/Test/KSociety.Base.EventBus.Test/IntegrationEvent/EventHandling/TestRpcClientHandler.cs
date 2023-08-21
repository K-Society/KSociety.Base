namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling;
using Autofac;
using Handlers;
using Event;
using Microsoft.Extensions.Logging;

public class TestRpcClientHandler : IntegrationRpcClientHandler<TestIntegrationEventReply>
{
    public TestRpcClientHandler(ILoggerFactory loggerFactory, IComponentContext componentContext)
        : base(loggerFactory, componentContext)
    {

    }
}
