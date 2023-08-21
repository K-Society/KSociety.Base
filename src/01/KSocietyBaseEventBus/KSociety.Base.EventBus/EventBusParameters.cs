namespace KSociety.Base.EventBus
{
    ///<inheritdoc/>
    public class EventBusParameters : IEventBusParameters
    {
        ///<inheritdoc/>
        public bool Debug { get; set; }

        ///<inheritdoc/>
        public IExchangeDeclareParameters ExchangeDeclareParameters { get; set; }

        ///<inheritdoc/>
        public IQueueDeclareParameters QueueDeclareParameters { get; set; }

        public EventBusParameters()
        {
        }

        public EventBusParameters(IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters, bool debug = false)
        {
            this.Debug = debug;
            this.ExchangeDeclareParameters = exchangeDeclareParameters;
            this.QueueDeclareParameters = queueDeclareParameters;
        }
    }
}
