namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class MessageBrokerOptions
    {
        public ConnectionFactory ConnectionFactory { get; set; }
        public ExchangeDeclareParameters ExchangeDeclareParameters { get; set; }
        public QueueDeclareParameters QueueDeclareParameters { get; set; }

        public MessageBrokerOptions()
        {
            ConnectionFactory = new ConnectionFactory();
            ExchangeDeclareParameters = new ExchangeDeclareParameters();
            QueueDeclareParameters = new QueueDeclareParameters();
        }
    }
}
