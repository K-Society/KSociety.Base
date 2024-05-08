// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading.Tasks;

    public interface IEventBusDynamic
    {
        ValueTask<bool> SubscribeDynamic<TDynamicIntegrationEventHandler>(string routingKey)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TDynamicIntegrationEventHandler>(string routingKey)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;
    }
}
