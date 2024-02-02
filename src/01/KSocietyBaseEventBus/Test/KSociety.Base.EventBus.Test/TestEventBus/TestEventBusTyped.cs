// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.TestEventBus
{
    using EventBusRabbitMQ;
    using IntegrationEvent.Event;
    using IntegrationEvent.EventHandling;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using Xunit;

    public class TestEventBusTyped : Test
    {
        private readonly IEventBusTyped _eventBusTyped;

        public TestEventBusTyped()
        {
            this._eventBusTyped = new EventBusRabbitMqTyped(this.PersistentConnection, this.LoggerFactory,
                new TestEventHandler(this.LoggerFactory), null, this.EventBusParameters, "Test");
            this._eventBusTyped.Subscribe<TestIntegrationEvent, TestEventHandler>("pippo");
        }

        [Fact]
        public async void SendData()
        {
            const string expectedName1 = "SuperPippo1";
            const string expectedName2 = "SuperPippo2";
            const string expectedName3 = "SuperPippo3";
            ;
            await this._eventBusTyped.Publish(new TestIntegrationEvent("pippo", expectedName1, null));
            ;
            await this._eventBusTyped.Publish(new TestIntegrationEvent("pippo", expectedName2, null));
            ;
            await this._eventBusTyped.Publish(new TestIntegrationEvent("pippo", expectedName3, null));
        }
    }
}
