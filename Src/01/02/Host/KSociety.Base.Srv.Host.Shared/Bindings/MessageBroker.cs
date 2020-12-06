using System;
using Autofac;
using KSociety.Base.EventBus;
using RabbitMQ.Client;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    public class MessageBroker : Module
    {
        private readonly bool _debugFlag;

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
            string brokerName, EventBus.ExchangeType exchangeType,
            bool exchangeDurable, bool exchangeAutoDelete,
            string mqHostName, string mqUserName, string mqPassword, 
            bool debugFlag,
            bool queueDurable,
            bool queueExclusive,
            bool queueAutoDelete)
        {
            _debugFlag = debugFlag;

            _brokerName = brokerName;
            _exchangeType = exchangeType;
            _exchangeDurable = exchangeDurable;
            _exchangeAutoDelete = exchangeAutoDelete;

            _queueDurable = queueDurable;
            _queueExclusive = queueExclusive;
            _queueAutoDelete = queueAutoDelete;

            _mqHostName = mqHostName;
            _mqUserName = mqUserName;
            _mqPassword = mqPassword;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var exchangeDeclareParameters = new ExchangeDeclareParameters(_brokerName, _exchangeType, _exchangeDurable, _exchangeAutoDelete);
            var queueDeclareParameters = new QueueDeclareParameters(_queueDurable, _queueExclusive, _queueAutoDelete);

            var rabbitMqConnectionFactory = new ConnectionFactory
            {
                HostName = _mqHostName,
                UserName = _mqUserName,
                Password = _mqPassword,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                DispatchConsumersAsync = true
            };

            builder.RegisterInstance(exchangeDeclareParameters).As<IExchangeDeclareParameters>().SingleInstance();
            builder.RegisterInstance(queueDeclareParameters).As<IQueueDeclareParameters>().SingleInstance();
            builder.RegisterInstance(rabbitMqConnectionFactory).As<IConnectionFactory>().SingleInstance();
        }
    }
}
