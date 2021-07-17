namespace KSociety.Base.EventBus
{
    /// <summary>
    /// The IExchangeDeclareParameters interface.
    /// </summary>
    public interface IExchangeDeclareParameters
    {
        string BrokerName { get; set; }
        string ExchangeType { get; set; }
        string ExchangeName { get; }
        bool ExchangeDurable { get; set; }
        bool ExchangeAutoDelete { get; set; }
    }
}
