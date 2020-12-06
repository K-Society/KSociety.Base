using System.Threading;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Test.IntegrationEvent.Event;
using KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling;
using KSociety.Base.EventBusRabbitMQ;
using Xunit;

namespace KSociety.Base.EventBus.Test.TestEventBus
{
    public class TestEventBusTyped : Test
    {
        private readonly IEventBusTyped _eventBusTyped;

        public TestEventBusTyped()
        {

            _eventBusTyped = new EventBusRabbitMqTyped(PersistentConnection, LoggerFactory, new TestEventHandler(LoggerFactory), null, ExchangeDeclareParameters, QueueDeclareParameters, "Test", CancellationToken.None);
            _eventBusTyped.Subscribe<TestIntegrationEvent, TestEventHandler>("pippo");
        }

        [Fact]
        public async void SendData()
        {
            const string expectedName1 = "SuperPippo1";
            const string expectedName2 = "SuperPippo2";
            const string expectedName3 = "SuperPippo3";
            ;
            await _eventBusTyped.Publish(new TestIntegrationEvent("pippo", expectedName1, null))
                .ConfigureAwait(false);
            ;
            await _eventBusTyped.Publish(new TestIntegrationEvent("pippo", expectedName2, null))
                .ConfigureAwait(false);
            ;
            await _eventBusTyped.Publish(new TestIntegrationEvent("pippo", expectedName3, null))
                .ConfigureAwait(false);
        }
    }
}
