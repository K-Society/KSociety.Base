// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.TestEventBus
{
    using EventBusRabbitMQ.Helper;
    using IntegrationEvent.Event;
    using IntegrationEvent.EventHandling;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using Xunit;

    public class TestEventBusTyped : Test
    {
        //private readonly IEventBusTyped _eventBusTyped;

        public Subscriber Subscriber { get; }

        public TestEventBusTyped()
        {
            this.Subscriber = new Subscriber(this.LoggerFactory, this.PersistentConnection, this.EventBusParameters, 700);
            //this._eventBusTyped = new EventBusRabbitMqTyped(this.PersistentConnection, this.LoggerFactory,
            //    new TestEventHandler(this.LoggerFactory), null, this.EventBusParameters, "Test");
            //this._eventBusTyped.Subscribe<TestIntegrationEvent, TestEventHandler>("pippo");

            for (var i = 0; i < 600; i++)
            {
                var routingKey = "pippo." + i;
                this.Subscriber.SubscribeTyped<TestEventHandler, TestIntegrationEvent>("Test_" + i, "TestQueue" + i,
                    routingKey, new TestEventHandler(this.LoggerFactory)).ConfigureAwait(false);
            }
        }

        [Fact]
        public async void SendData()
        {
            const string expectedName1 = "SuperPippo1";
            const string expectedName2 = "SuperPippo2";
            const string expectedName3 = "SuperPippo3";

            for (var i = 0; i < 600; i++)
            {
                if (this.Subscriber.EventBus.TryGetValue("Test_"+i, out var bus))
                {
                    await ((IEventBusTyped)bus).Publish(new TestIntegrationEvent("pippo." + i, expectedName1, null));
                    await ((IEventBusTyped)bus).Publish(new TestIntegrationEvent("pippo." + i, expectedName2, null));
                    await ((IEventBusTyped)bus).Publish(new TestIntegrationEvent("pippo." + i, expectedName3, null));
                }
            }

            //;
            //await this.Subscriber.Publish(new TestIntegrationEvent("pippo.1", expectedName1, null));
            //;
            //await this.Subscriber.Publish(new TestIntegrationEvent("pippo.2", expectedName2, null));
            //;
            //await this.Subscriber.Publish(new TestIntegrationEvent("pippo.3", expectedName3, null));
        }
    }
}
