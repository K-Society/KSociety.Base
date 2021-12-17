namespace KSociety.Base.EventBus;

///<inheritdoc/>
public class ExchangeDeclareParameters : IExchangeDeclareParameters
{
    ///<inheritdoc/>
    public string BrokerName { get; set; }

    ///<inheritdoc/>
    public string ExchangeType { get; set; }

    ///<inheritdoc/>
    public string ExchangeName
    {
        get
        {
            return BrokerName + "_" + ExchangeType;
        }
    }

    ///<inheritdoc/>
    public bool ExchangeDurable { get; set; }

    ///<inheritdoc/>
    public bool ExchangeAutoDelete { get; set; }

    public ExchangeDeclareParameters(){}

    public ExchangeDeclareParameters(string brokerName, ExchangeType exchangeType,  bool exchangeDurable = false, bool exchangeAutoDelete = false)
    {
        BrokerName = brokerName;
        ExchangeType = exchangeType.ToString().ToLower();
        ExchangeDurable = exchangeDurable;
        ExchangeAutoDelete = exchangeAutoDelete;
    }
}