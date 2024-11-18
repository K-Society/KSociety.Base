namespace KSociety.Base.EventBusRabbitMQ.Binding
{
    using System;
    using Autofac;
    using EventBus;
    using EventBusRabbitMQ;
    using Helper;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    /// <summary>
    /// The MessageBroker module for Autofac.
    /// </summary>
    public class MessageBroker<
        TExchangeDeclareParameters,
        TQueueDeclareParameters,
        TEventBusParameters,
        TConnectionFactory,
        TSubscriber,
        TExchangeDeclareParametersClass,
        TQueueDeclareParametersClass,
        TEventBusParametersClass,
        TSubscriberClass> : Module
        where TExchangeDeclareParameters : IExchangeDeclareParameters
        where TQueueDeclareParameters : IQueueDeclareParameters
        where TEventBusParameters : IEventBusParameters
        where TConnectionFactory : IConnectionFactory
        where TSubscriber : ISubscriber
        where TExchangeDeclareParametersClass : EventBus.ExchangeDeclareParameters, TExchangeDeclareParameters, new()
        where TQueueDeclareParametersClass : EventBus.QueueDeclareParameters, TQueueDeclareParameters, new()
        where TEventBusParametersClass : EventBusParameters, TEventBusParameters, new()
        where TSubscriberClass : Subscriber, TSubscriber
    {
        private readonly bool _debug;
        private readonly int _eventBusNumber;
        //private readonly bool _dispatchConsumersAsync;
        private readonly string _brokerName;
        private readonly EventBus.ExchangeType _exchangeType;
        private readonly bool _exchangeDurable;
        private readonly bool _exchangeAutoDelete;
        private readonly bool _queueDurable;
        private readonly bool _queueExclusive;
        private readonly bool _queueAutoDelete;

        private readonly string _mqHostName;
        private readonly string _mqUserName;
        private readonly string _mqPassword;

        public MessageBroker(
            int eventBusNumber, /*bool dispatchConsumersAsync,*/
            string brokerName, EventBus.ExchangeType exchangeType,
            bool exchangeDurable, bool exchangeAutoDelete,
            string mqHostName, string mqUserName, string mqPassword, bool debug,
            bool queueDurable,
            bool queueExclusive,
            bool queueAutoDelete)
        {
            this._debug = debug;
            this._eventBusNumber = eventBusNumber;
            //this._dispatchConsumersAsync = dispatchConsumersAsync;
            this._brokerName = brokerName;
            this._exchangeType = exchangeType;
            this._exchangeDurable = exchangeDurable;
            this._exchangeAutoDelete = exchangeAutoDelete;

            this._queueDurable = queueDurable;
            this._queueExclusive = queueExclusive;
            this._queueAutoDelete = queueAutoDelete;

            this._mqHostName = mqHostName;
            this._mqUserName = mqUserName;
            this._mqPassword = mqPassword;
        }

        public MessageBroker(MessageBrokerOptions messageBroker, bool debug = false)
        {
            this._debug = debug;
            this._eventBusNumber = messageBroker.EventBusNumber;
            //this._dispatchConsumersAsync = messageBroker.DispatchConsumersAsync;
            this._brokerName = messageBroker.ExchangeDeclareParameters.BrokerName;
            this._exchangeType = messageBroker.ExchangeDeclareParameters.ExchangeType;
            this._exchangeDurable = messageBroker.ExchangeDeclareParameters.ExchangeDurable;
            this._exchangeAutoDelete = messageBroker.ExchangeDeclareParameters.ExchangeAutoDelete;

            this._queueDurable = messageBroker.QueueDeclareParameters.QueueDurable;
            this._queueExclusive = messageBroker.QueueDeclareParameters.QueueExclusive;
            this._queueAutoDelete = messageBroker.QueueDeclareParameters.QueueAutoDelete;

            this._mqHostName = messageBroker.ConnectionFactory.MqHostName;
            this._mqUserName = messageBroker.ConnectionFactory.MqUserName;
            this._mqPassword = messageBroker.ConnectionFactory.MqPassword;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var exchangeDeclareParameters = new TExchangeDeclareParametersClass
            {
                BrokerName = this._brokerName,
                ExchangeType = this._exchangeType.ToString().ToLower(),
                ExchangeDurable = this._exchangeDurable,
                ExchangeAutoDelete = this._exchangeAutoDelete
            };

            var queueDeclareParameters = new TQueueDeclareParametersClass
            {
                QueueDurable = this._queueDurable, QueueExclusive = this._queueExclusive, QueueAutoDelete = this._queueAutoDelete
            };

            var eventBusParameters = new TEventBusParametersClass
            {
                ExchangeDeclareParameters = exchangeDeclareParameters,
                QueueDeclareParameters = queueDeclareParameters,
                Debug = this._debug
            };

            var rabbitMqConnectionFactory = new RabbitMQ.Client.ConnectionFactory
            {
                HostName = this._mqHostName,
                UserName = this._mqUserName,
                Password = this._mqPassword,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                ContinuationTimeout = TimeSpan.FromSeconds(120)
                //DispatchConsumersAsync = this._dispatchConsumersAsync
            };

            builder.RegisterInstance(exchangeDeclareParameters).As<TExchangeDeclareParametersClass>()
                .As<TExchangeDeclareParameters>().SingleInstance();
            builder.RegisterInstance(queueDeclareParameters).As<TQueueDeclareParametersClass>().As<TQueueDeclareParameters>().SingleInstance();
            //builder.RegisterInstance(eventBusParameters).As<TEventBusParameters>().As<IEventBusParameters>().SingleInstance();
            builder.RegisterInstance(eventBusParameters).As<TEventBusParametersClass>().As<TEventBusParameters>().SingleInstance();
            builder.RegisterInstance(rabbitMqConnectionFactory).As<TConnectionFactory>().SingleInstance();

            //builder.RegisterType<TEventBusParametersClass>().As<TEventBusParameters>().SingleInstance();
            builder.RegisterType<DefaultRabbitMqPersistentConnection>().As<IRabbitMqPersistentConnection>()
                .UsingConstructor(typeof(TConnectionFactory), typeof(ILogger<DefaultRabbitMqPersistentConnection>)).SingleInstance();

            builder.RegisterType<TSubscriberClass>().UsingConstructor(typeof(IRabbitMqPersistentConnection), typeof(TEventBusParameters), typeof(int), /*typeof(bool),*/ typeof(ILogger<EventBusRabbitMq>)).WithParameter("eventBusNumber", this._eventBusNumber)/*.WithParameter("dispatchConsumersAsync", this._dispatchConsumersAsync)*/.As<TSubscriber>().SingleInstance();
        }
    }
}
