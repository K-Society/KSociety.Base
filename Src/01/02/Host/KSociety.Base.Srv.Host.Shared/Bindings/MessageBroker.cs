﻿using System;
using Autofac;
using KSociety.Base.EventBus;
using RabbitMQ.Client;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    /// <summary>
    /// The MessageBroker module for Autofac.
    /// </summary>
    public class MessageBroker : Module
    {
        private readonly bool _debug;
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
            string mqHostName, string mqUserName, string mqPassword, bool debug,
            bool queueDurable,
            bool queueExclusive,
            bool queueAutoDelete)
        {
            _debug = debug;
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

        public MessageBroker(Class.MessageBrokerOptions messageBroker, bool debug = false)
        {
            _debug = debug;
            _brokerName = messageBroker.ExchangeDeclareParameters.BrokerName;
            _exchangeType = messageBroker.ExchangeDeclareParameters.ExchangeType;
            _exchangeDurable = messageBroker.ExchangeDeclareParameters.ExchangeDurable;
            _exchangeAutoDelete = messageBroker.ExchangeDeclareParameters.ExchangeAutoDelete;

            _queueDurable = messageBroker.QueueDeclareParameters.QueueDurable;
            _queueExclusive = messageBroker.QueueDeclareParameters.QueueExclusive;
            _queueAutoDelete = messageBroker.QueueDeclareParameters.QueueAutoDelete;

            _mqHostName = messageBroker.ConnectionFactory.MqHostName;
            _mqUserName = messageBroker.ConnectionFactory.MqUserName;
            _mqPassword = messageBroker.ConnectionFactory.MqPassword;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var exchangeDeclareParameters = new ExchangeDeclareParameters(_brokerName, _exchangeType, _exchangeDurable, _exchangeAutoDelete);
            var queueDeclareParameters = new QueueDeclareParameters(_queueDurable, _queueExclusive, _queueAutoDelete);
            var eventBusParameters = new EventBusParameters(exchangeDeclareParameters, queueDeclareParameters, _debug);

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
            builder.RegisterInstance(eventBusParameters).As<IEventBusParameters>().SingleInstance();
            builder.RegisterInstance(rabbitMqConnectionFactory).As<IConnectionFactory>().SingleInstance();
        }
    }
}
