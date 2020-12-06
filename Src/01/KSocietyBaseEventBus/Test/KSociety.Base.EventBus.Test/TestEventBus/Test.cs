using System;
using Autofac;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Test.ProtoModel;
using KSociety.Base.EventBusRabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace KSociety.Base.EventBus.Test.TestEventBus
{
    public class Test
    {
        protected const bool ExchangeAutoDelete = false;
        protected const bool QueueAutoDelete = false;
        protected ILoggerFactory LoggerFactory;
        protected IConnectionFactory ConnectionFactory;
        protected IRabbitMqPersistentConnection PersistentConnection;
        protected IExchangeDeclareParameters ExchangeDeclareParameters;
        protected IQueueDeclareParameters QueueDeclareParameters;
        protected IComponentContext ComponentContext;
        protected IEventBus EventBus;
        //protected readonly IEventBusRpcServer _eventBusRpcServer;

        public Test()
        {
            Configuration.ProtoBufConfiguration();
            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            ConnectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "KSociety",
                Password = "KSociety",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                DispatchConsumersAsync = true
            };

            ExchangeDeclareParameters = new ExchangeDeclareParameters("k-society_test", KSociety.Base.EventBus.ExchangeType.Direct, false, ExchangeAutoDelete);
            QueueDeclareParameters = new QueueDeclareParameters(false, false, QueueAutoDelete);

            PersistentConnection = new DefaultRabbitMqPersistentConnection(ConnectionFactory, LoggerFactory);

            var builder = new ContainerBuilder();
            builder.RegisterModule(new Bindings.Test.Test());
            var container = builder.Build();

            ComponentContext = container.BeginLifetimeScope();
        }
    }
}
