// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.EventBus
{
    using Handler;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEventBusBase
    {
        IIntegrationGeneralHandler EventHandler { get; }

        void Initialize(CancellationToken cancel = default);

        ValueTask Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
    }
}
