namespace KSociety.Base.EventBusRabbitMQ.Binding
{
    public class QueueDeclareParameters
    {
        public bool QueueDurable { get; set; } = false;
        public bool QueueExclusive { get; set; } = false;
        public bool QueueAutoDelete { get; set; } = true;

        public QueueDeclareParameters()
        {

        }
    }
}
