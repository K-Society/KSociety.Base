namespace KSociety.Base.EventBus
{
    ///<inheritdoc/>
    public class QueueDeclareParameters : IQueueDeclareParameters
    {
        ///<inheritdoc/>
        public bool QueueDurable { get; set; }

        ///<inheritdoc/>
        public bool QueueExclusive { get; set; }

        ///<inheritdoc/>
        public bool QueueAutoDelete { get; set; }

        public QueueDeclareParameters()
        {
            QueueDurable = true;
            QueueExclusive = false;
            QueueAutoDelete = false;
        }

        public QueueDeclareParameters(bool queueDurable, bool queueExclusive, bool queueAutoDelete)
        {
            QueueDurable = queueDurable;
            QueueExclusive = queueExclusive;
            QueueAutoDelete = queueAutoDelete;
        }
    }
}