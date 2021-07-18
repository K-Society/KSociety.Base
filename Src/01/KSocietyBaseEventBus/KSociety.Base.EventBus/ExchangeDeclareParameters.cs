namespace KSociety.Base.EventBus
{
    public class ExchangeDeclareParameters : IExchangeDeclareParameters
    {
        public string BrokerName { get; set; }
        public string ExchangeType { get; set; }

        public string ExchangeName
        {
            get
            {
                return BrokerName + "_" + ExchangeType;
            }
        }

        public bool ExchangeDurable { get; set; }
        public bool ExchangeAutoDelete { get; set; }

        public ExchangeDeclareParameters(){}

        public ExchangeDeclareParameters(string brokerName, ExchangeType exchangeType)
        {
            BrokerName = brokerName;
            ExchangeType = exchangeType.ToString().ToLower();
            ExchangeDurable = false;
            ExchangeAutoDelete = false;
        }

        public ExchangeDeclareParameters(string brokerName, ExchangeType exchangeType,  bool exchangeDurable, bool exchangeAutoDelete)
        {
            BrokerName = brokerName;
            ExchangeType = exchangeType.ToString().ToLower();
            ExchangeDurable = exchangeDurable;
            ExchangeAutoDelete = exchangeAutoDelete;
        }
    }
}
