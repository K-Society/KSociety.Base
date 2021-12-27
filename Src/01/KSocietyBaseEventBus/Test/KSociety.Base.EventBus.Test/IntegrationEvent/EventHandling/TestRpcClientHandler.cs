using Autofac;
using KSociety.Base.EventBus.Handlers;
using KSociety.Base.EventBus.Test.IntegrationEvent.Event;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling;

public class TestRpcClientHandler : IntegrationRpcClientHandler<TestIntegrationEventReply>
{
    public TestRpcClientHandler(ILoggerFactory loggerFactory, IComponentContext componentContext)
        : base(loggerFactory, componentContext)
    {

    }
}