namespace KSociety.Base.EventBus
{
    /// <summary>
    /// The QueueDeclare parameters.
    /// </summary>
    public interface IQueueDeclareParameters
    {
        /// <summary>
        /// The Queue durable flag property.
        /// </summary>
        bool QueueDurable { get; set; }

        /// <summary>
        /// The Queue exclusive flag property.
        /// </summary>
        bool QueueExclusive { get; set; }

        /// <summary>
        /// The Queue auto delete flag property.
        /// </summary>
        bool QueueAutoDelete { get; set; }
    }
}