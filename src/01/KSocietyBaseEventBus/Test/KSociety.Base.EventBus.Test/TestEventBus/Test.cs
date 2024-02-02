// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.TestEventBus
{
    using System;
    using Autofac;
    using KSociety.Base.EventBus.Abstractions.EventBus;
    using ProtoModel;
    using EventBusRabbitMQ;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    public class Test
    {
        protected const bool ExchangeAutoDelete = true;
        protected const bool QueueAutoDelete = true;
        protected ILoggerFactory LoggerFactory;
        protected IConnectionFactory ConnectionFactory;
        protected IRabbitMqPersistentConnection PersistentConnection;
        protected IEventBusParameters EventBusParameters;
        private readonly IExchangeDeclareParameters _exchangeDeclareParameters;
        private readonly IQueueDeclareParameters _queueDeclareParameters;
        protected IComponentContext ComponentContext;
        protected IEventBus EventBus;
        //protected readonly IEventBusRpcServer _eventBusRpcServer;

        public Test()
        {
            Configuration.ProtoBufConfiguration();
            this.LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            this.ConnectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "KSociety",
                Password = "KSociety",
                //UserName = "guest",
                //Password = "guest",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                DispatchConsumersAsync = true
            };

            this._exchangeDeclareParameters = new ExchangeDeclareParameters("k-society_test",
                KSociety.Base.EventBus.ExchangeType.Direct, false, ExchangeAutoDelete);
            this._queueDeclareParameters = new QueueDeclareParameters(false, false, QueueAutoDelete);

            this.EventBusParameters =
                new EventBusParameters(this._exchangeDeclareParameters, this._queueDeclareParameters, true);
            this.PersistentConnection =
                new DefaultRabbitMqPersistentConnection(this.ConnectionFactory, this.LoggerFactory);

            var builder = new ContainerBuilder();
            builder.RegisterModule(new Bindings.Test.Test());
            var container = builder.Build();

            this.ComponentContext = container.BeginLifetimeScope();
        }
    }
}
