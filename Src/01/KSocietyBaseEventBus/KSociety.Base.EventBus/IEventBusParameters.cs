namespace KSociety.Base.EventBus
{
    public interface IEventBusParameters
    {
        bool Debug { get; set; }
        IExchangeDeclareParameters ExchangeDeclareParameters { get; set; }
        IQueueDeclareParameters QueueDeclareParameters { get; set; }
    }
}
