namespace KSociety.Base.EventBusRabbitMQ.Binding
{
    public class MessageBrokerOptions
    {
        public int EventBusNumber { get; set; }
        public ConnectionFactory ConnectionFactory { get; set; }
        public ExchangeDeclareParameters ExchangeDeclareParameters { get; set; }
        public QueueDeclareParameters QueueDeclareParameters { get; set; }

        public MessageBrokerOptions()
        {

        }
    }
}
