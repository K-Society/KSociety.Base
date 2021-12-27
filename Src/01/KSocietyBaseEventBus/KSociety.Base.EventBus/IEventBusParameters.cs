namespace KSociety.Base.EventBus;

/// <summary>
/// The EventBus parameters.
/// </summary>
public interface IEventBusParameters
{
    /// <summary>
    /// The debug flag property.
    /// </summary>
    bool Debug { get; set; }

    /// <summary>
    /// The ExchangeDeclare parameters set property.
    /// </summary>
    IExchangeDeclareParameters ExchangeDeclareParameters { get; set; }

    /// <summary>
    /// The QueueDeclare parameters set property.
    /// </summary>
    IQueueDeclareParameters QueueDeclareParameters { get; set; }
}