namespace KSociety.Base.EventBus
{
    public interface IQueueDeclareParameters
    {
        bool QueueDurable { get; set; }
        bool QueueExclusive { get; set; }
        bool QueueAutoDelete { get; set; }
    }
}
