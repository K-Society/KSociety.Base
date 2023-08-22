// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    using System;
    using Autofac;
    using EventBus;
    using EventBusRabbitMQ;
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
        TExchangeDeclareParametersClass,
        TQueueDeclareParametersClass,
        TEventBusParametersClass> : Module
        where TExchangeDeclareParameters : IExchangeDeclareParameters
        where TQueueDeclareParameters : IQueueDeclareParameters
        where TEventBusParameters : IEventBusParameters
        where TConnectionFactory : IConnectionFactory
        where TExchangeDeclareParametersClass : ExchangeDeclareParameters, new()
        where TQueueDeclareParametersClass : QueueDeclareParameters, new()
        where TEventBusParametersClass : EventBusParameters, new()
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
            this._debug = debug;
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

        public MessageBroker(Class.MessageBrokerOptions messageBroker, bool debug = false)
        {
            this._debug = debug;
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

            var rabbitMqConnectionFactory = new ConnectionFactory
            {
                HostName = this._mqHostName,
                UserName = this._mqUserName,
                Password = this._mqPassword,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                ContinuationTimeout = TimeSpan.FromSeconds(120),
                DispatchConsumersAsync = true
            };

            builder.RegisterInstance(exchangeDeclareParameters).As<TExchangeDeclareParameters>().SingleInstance();
            builder.RegisterInstance(queueDeclareParameters).As<TQueueDeclareParameters>().SingleInstance();
            builder.RegisterInstance(eventBusParameters).As<TEventBusParameters>().SingleInstance();
            builder.RegisterInstance(rabbitMqConnectionFactory).As<TConnectionFactory>().SingleInstance();
            builder.RegisterType<DefaultRabbitMqPersistentConnection>().As<IRabbitMqPersistentConnection>().UsingConstructor(typeof(IConnectionFactory), typeof(ILogger<DefaultRabbitMqPersistentConnection>))
                .SingleInstance();
        }
    }
}
