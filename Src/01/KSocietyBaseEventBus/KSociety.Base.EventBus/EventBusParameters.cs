namespace KSociety.Base.EventBus
{
    public class EventBusParameters : IEventBusParameters
    {
        public bool Debug { get; set; }
        public IExchangeDeclareParameters ExchangeDeclareParameters { get; set; }
        public IQueueDeclareParameters QueueDeclareParameters { get; set; }

        public EventBusParameters(){}

        public EventBusParameters(IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters, bool debug = false)
        {
            Debug = debug;
            ExchangeDeclareParameters = exchangeDeclareParameters;
            QueueDeclareParameters = queueDeclareParameters;
        }
    }
}
