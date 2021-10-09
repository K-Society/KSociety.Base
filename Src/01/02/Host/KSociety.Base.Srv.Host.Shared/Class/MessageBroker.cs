namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class MessageBroker
    {
        public ConnectionFactory ConnectionFactory { get; set; }
        public ExchangeDeclareParameters ExchangeDeclareParameters { get; set; }
        public QueueDeclareParameters QueueDeclareParameters { get; set; }
    }
}
