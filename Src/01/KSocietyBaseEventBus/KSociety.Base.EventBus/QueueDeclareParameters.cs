namespace KSociety.Base.EventBus
{
    public class QueueDeclareParameters : IQueueDeclareParameters
    {
        public bool QueueDurable { get; set; }
        public bool QueueExclusive { get; set; }
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
