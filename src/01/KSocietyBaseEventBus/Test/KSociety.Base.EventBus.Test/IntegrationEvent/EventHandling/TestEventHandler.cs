namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Handler;
using Event;
using Microsoft.Extensions.Logging;

public class TestEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
{
    private readonly ILoggerFactory _loggerFactory;
    private ILogger _logger;

    public TestEventHandler(
        ILoggerFactory loggerFactory
    )
    {
        this._loggerFactory = loggerFactory;
    }

    public async ValueTask Handle(TestIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        ;
    }
}
