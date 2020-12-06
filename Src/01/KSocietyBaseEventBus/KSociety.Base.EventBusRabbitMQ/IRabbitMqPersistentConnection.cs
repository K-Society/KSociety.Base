using System;
using RabbitMQ.Client;

namespace KSociety.Base.EventBusRabbitMQ
{
    public interface IRabbitMqPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
