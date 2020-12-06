using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.EventBus.Abstractions.Handler;
using KSociety.Base.EventBus.Test.IntegrationEvent.Event;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling
{
    public class TestEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        private readonly ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public TestEventHandler(
            ILoggerFactory loggerFactory
        )
        {
            _loggerFactory = loggerFactory;
        }

        public async ValueTask Handle(TestIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            ;
        }
    }
}
