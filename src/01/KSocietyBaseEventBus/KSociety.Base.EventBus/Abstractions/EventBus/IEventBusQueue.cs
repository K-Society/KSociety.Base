// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading.Tasks;

    public interface IEventBusQueue : IEventBus
    {
        IIntegrationQueueHandler<T>? GetIntegrationQueueHandler<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>;

        ValueTask SubscribeQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>;

        void UnsubscribeQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>;
    }
}
