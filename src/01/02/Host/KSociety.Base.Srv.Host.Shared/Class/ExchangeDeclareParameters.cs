namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class ExchangeDeclareParameters
    {
        public string BrokerName { get; set; }
        public EventBus.ExchangeType ExchangeType { get; set; } = EventBus.ExchangeType.Direct;
        public bool ExchangeDurable { get; set; } = false;
        public bool ExchangeAutoDelete { get; set; } = true;

        public ExchangeDeclareParameters()
        {

        }
    }
}
