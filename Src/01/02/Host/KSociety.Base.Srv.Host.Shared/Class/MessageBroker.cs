namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class MessageBroker
    {
        public ConnectionFactory ConnectionFactory { get; set; }
        public ExchangeDeclareParameters ExchangeDeclareParameters { get; set; }
        public QueueDeclareParameters QueueDeclareParameters { get; set; }
    }

    public class ConnectionFactory
    {
        public string MqHostName { get; set; } = "localhost";
        public string MqUserName { get; set; } = "KSociety";
        public string MqPassword { get; set; } = "KSociety";
    }

    public class ExchangeDeclareParameters
    {
        public string BrokerName { get; set; }
        public Base.EventBus.ExchangeType ExchangeType { get; set; } = EventBus.ExchangeType.Direct;
        public bool ExchangeDurable { get; set; } = false;
        public bool ExchangeAutoDelete { get; set; } = true;
    }

    public class QueueDeclareParameters
    {
        public bool QueueDurable { get; set; } = false;
        public bool QueueExclusive { get; set; } = false;
        public bool QueueAutoDelete { get; set; } = true;
    }
}
