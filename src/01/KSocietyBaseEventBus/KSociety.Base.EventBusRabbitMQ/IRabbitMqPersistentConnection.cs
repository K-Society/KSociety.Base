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
        IModel? CreateModel();
    }
}