// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading.Tasks;

    public interface IEventBusQueue : IEventBus
    {
        IIntegrationQueueHandler<TIntegrationEvent> GetIntegrationQueueHandler<TIntegrationEvent, TIntegrationQueueHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>;

        ValueTask SubscribeQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>;

        void UnsubscribeQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>;
    }
}
