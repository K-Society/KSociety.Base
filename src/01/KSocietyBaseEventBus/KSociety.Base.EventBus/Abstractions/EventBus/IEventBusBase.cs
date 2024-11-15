// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using System.Threading;
    using System.Threading.Tasks;
    using Handler;

    public interface IEventBusBase
    {
        IIntegrationGeneralHandler EventHandler { get; }

        bool Initialize<TIntegrationEvent>(CancellationToken cancel = default)
            where TIntegrationEvent : IIntegrationEvent, new();

        ValueTask<bool> Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
    }
}
