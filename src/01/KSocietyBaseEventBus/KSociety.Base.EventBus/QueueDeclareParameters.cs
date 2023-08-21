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
            this.QueueDurable = true;
            this.QueueExclusive = false;
            this.QueueAutoDelete = false;
        }

        public QueueDeclareParameters(bool queueDurable, bool queueExclusive, bool queueAutoDelete)
        {
            this.QueueDurable = queueDurable;
            this.QueueExclusive = queueExclusive;
            this.QueueAutoDelete = queueAutoDelete;
        }
    }
}
