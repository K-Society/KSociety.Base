// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;

    public interface IRabbitMqPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }
        ValueTask<bool> TryConnectAsync();
        Task<IChannel> CreateModel();
    }
}
