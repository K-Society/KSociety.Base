using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace KSociety.Base.EventBusRabbitMQ
{
    public interface IRabbitMqPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }
        //bool TryConnect();
        ValueTask<bool> TryConnectAsync();
        IModel CreateModel();
    }
}
