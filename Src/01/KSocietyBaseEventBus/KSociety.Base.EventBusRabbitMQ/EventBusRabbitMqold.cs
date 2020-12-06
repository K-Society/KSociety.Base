//using Autofac;
//using Microsoft.Extensions.Logging;
//using Polly;
//using ProtoBuf;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using RabbitMQ.Client.Exceptions;

//using System;
//using System.IO;
//using System.Linq;
//using System.Net.Sockets;
//using System.Threading;
//using System.Threading.Tasks;


//namespace EventBusRabbitMQ
//{
//    public class EventBusRabbitMqold : DisposableObject, IEventBus
//    {
//        private const string BrokerName = "k-society_event_bus";

//        private readonly string _exchange;

//        private readonly IRabbitMqPersistentConnection _persistentConnection;
//        private readonly ILogger<EventBusRabbitMqold> _logger;
//        private readonly IEventBusSubscriptionsManager _subsManager;
//        private readonly ILifetimeScope _autofac;

//        //private readonly IIntegrationGeneralHandler EventHandler;
//        public IIntegrationGeneralHandler EventHandler { get; }
//        private const string AutofacScopeName = "k-society_event_bus";

//        private IModel _consumerChannel;
//        private IModel _consumerChannelReply;
//        private string _queueName;
//        private string _queueNameReply;
//        private readonly string _type;
//        private readonly bool _rpcFlag;

//        private readonly string _correlationId;

//        #region [Constructor]

//        public EventBusRabbitMqold(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
//            ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string type, string queueName = null,
//            CancellationToken cancel = default, bool rpcFlag = false)
//        {
//            _type = type;
//            _rpcFlag = rpcFlag;
//            _exchange = BrokerName + "_" + _type;
//            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
//            _logger = loggerFactory.CreateLogger<EventBusRabbitMqold>() ?? throw new ArgumentNullException(nameof(loggerFactory));
//            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
//            _queueName = queueName;
//            _consumerChannel = CreateConsumerChannel(cancel);
//            _autofac = autofac;
//            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
//            _subsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;

//            if (_rpcFlag)
//            {
//                _queueNameReply = _queueName + "_Reply";
//                _consumerChannelReply = CreateConsumerChannelReply(cancel);
//                _correlationId = Guid.NewGuid().ToString();
                
//            }

//        }

//        public EventBusRabbitMqold(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
//            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager, string type, string queueName = null,
//            CancellationToken cancel = default, bool rpcFlag = false)
//        {
//            _type = type;
//            _rpcFlag = rpcFlag;
//            _exchange = BrokerName + "_" + _type;
//            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
//            _logger = loggerFactory.CreateLogger<EventBusRabbitMqold>() ?? throw new ArgumentNullException(nameof(loggerFactory));
//            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
//            _queueName = queueName;
//            _consumerChannel = CreateConsumerChannel(cancel);
//            EventHandler = eventHandler;
//            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
//            _subsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;

//            _logger.LogInformation("EventBusRabbitMq: " + rpcFlag);
//            if (_rpcFlag)
//            {
//                _queueNameReply = _queueName + "_Reply";
//                _consumerChannelReply = CreateConsumerChannelReply();
//                _correlationId = Guid.NewGuid().ToString();
                
//            }
//        }

//        //public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
//        //    IIntegrationGeneralHandler eventHandler, IIntegrationGeneralHandler eventReplyHandler, IEventBusSubscriptionsManager subsManager, string type, string queueName = null,
//        //    CancellationToken cancel = default, bool rpcFlag = false) 
//        //{
//        //    _type = type;
//        //    _rpcFlag = rpcFlag;
//        //    _exchange = BrokerName + "_" + _type;
//        //    _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
//        //    _logger = loggerFactory.CreateLogger<EventBusRabbitMq>() ?? throw new ArgumentNullException(nameof(loggerFactory));
//        //    _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
//        //    _queueName = queueName;
//        //    _consumerChannel = CreateConsumerChannel(cancel);
//        //    _eventHandler = eventHandler;
//        //    _eventReplyHandler = eventReplyHandler;
//        //    _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;

//        //    _logger.LogInformation("EventBusRabbitMq: " + rpcFlag);
//        //    if (_rpcFlag)
//        //    {
//        //        _queueNameRpc = _queueName + "Rpc";
//        //        _consumerChannelRpc = CreateConsumerChannelRpc();
//        //        _correlationId = Guid.NewGuid().ToString();

//        //    }
//        //}

//        public EventBusRabbitMqold(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
//            IEventBusSubscriptionsManager subsManager, string type, string queueName = null,
//            CancellationToken cancel = default, bool rpcFlag = false)
//        {
//            _type = type;
//            _rpcFlag = rpcFlag;
//            _exchange = BrokerName + "_" + _type;
//            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
//            _logger = loggerFactory.CreateLogger<EventBusRabbitMqold>() ?? throw new ArgumentNullException(nameof(loggerFactory));
//            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
//            _queueName = queueName;
//            _consumerChannel = CreateConsumerChannel(cancel);
//            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
//            _subsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;

//            if (_rpcFlag)
//            {
//                _queueNameReply = _queueName + "_Reply";
//                _consumerChannelReply = CreateConsumerChannelReply();
//                _correlationId = Guid.NewGuid().ToString();
                
//            }
//        }

//        #endregion

//        public IIntegrationQueueHandler<T> GetIntegrationQueueHandler<T, TH>()
//            where T : IIntegrationEvent
//            where TH : IIntegrationQueueHandler<T>
//        {
//            if (!(EventHandler is null) && EventHandler is IIntegrationQueueHandler<T> queue)
//            {
//                return queue;
//            }

//            return null;
//        }

//        private void SubsManager_OnEventRemoved(object sender, string eventName)
//        {
//            if (!_persistentConnection.IsConnected)
//            {
//                _persistentConnection.TryConnect();
//            }

//            using var channel = _persistentConnection.CreateModel();

//            channel.QueueUnbind(_queueName, _exchange, eventName);


//            if (!_subsManager.IsEmpty) return;
//            _queueName = string.Empty;
//            _consumerChannel?.Close();

//        }

//        private void SubsManager_OnEventReplyRemoved(object sender, string eventName)
//        {
//            if (!_persistentConnection.IsConnected)
//            {
//                _persistentConnection.TryConnect();
//            }

//            using var channel = _persistentConnection.CreateModel();

//            channel.QueueUnbind(_queueNameReply, _exchange, eventName);

//            if (!_subsManager.IsReplyEmpty) return;

//            _queueNameReply = string.Empty;
//            _consumerChannelReply?.Close();

//        }

//        public async ValueTask Publish(IIntegrationEvent @event)
//        {
//            _logger.LogWarning("Publish: " + @event.GetType() + " " + _rpcFlag);
//            if (!_persistentConnection.IsConnected)
//            {
//                _persistentConnection.TryConnect();
//            }

//            //var policy = Policy.Handle<BrokerUnreachableException>()
//            //    .Or<SocketException>()
//            //    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
//            //    {
//            //        //_logger.LogWarning(ex.ToString());
//            //        _logger.Warn(ex.ToString());
//            //    });

//            var policy = Policy.Handle<BrokerUnreachableException>()
//                .Or<SocketException>()
//                .Or<Exception>()
//                .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
//                {
//                    _logger.LogWarning(ex.ToString());
//                });

//            using var channel = _persistentConnection.CreateModel();
//            //var eventName = @event.GetType().Name;
//            //var eventName = @event.RoutingKey;
//            var routingKey = @event.RoutingKey;

            
//            //var replyQueueName = channel.QueueDeclare().QueueName;
//            //if (string.IsNullOrEmpty(@event.RoutingKey))
//            //{

//            //}
//            //else
//            //{
//            //    routingKey = eventName + "." + @event.RoutingKey;
//            //}
//            //_logger.LogInformation("Publish RoutingKey: " + routingKey + " Id: " + @event.Id + " Date: " + @event.CreationDate + " RK: " + @event.RoutingKey);
//            channel.ExchangeDeclare(_exchange, _type);
//            //channel.ExchangeDeclare(BrokerName, _type);
//            //channel.ExchangeDeclare(BrokerName,"direct");
//            //channel.ExchangeDeclare(BrokerName,"direct", true, true);

//            //var message = JsonConvert.SerializeObject(@event);
//            //var body = Encoding.UTF8.GetBytes(message);

//            await using var ms = new MemoryStream();
//            Serializer.Serialize(ms, @event);
//            var body = ms.ToArray();

//            policy.Execute(() =>
//            {
//                var properties = channel.CreateBasicProperties();
//                properties.DeliveryMode = 1; //2 = persistent, write on disk

//                if (_rpcFlag)
//                {
//                    //_logger.LogInformation("Publish: " + _correlationId + " " + replyQueueName);
//                    properties.CorrelationId = _correlationId;
//                    properties.ReplyTo = _queueNameReply; //replyQueueName;
//                }
//                //properties.ReplyTo

//                //channel.BasicPublish(BrokerName,eventName,true,properties,body);
//                //channel.BasicPublish(BrokerName,routingKey,true,properties,body);
//                channel.BasicPublish(_exchange, routingKey, true, properties, body);
//            });
//        }

//        //public void PublishRpc(IIntegrationEvent @event)
//        //{
//        //    if (!_persistentConnection.IsConnected)
//        //    {
//        //        _persistentConnection.TryConnect();
//        //    }

//        //    //var policy = Policy.Handle<BrokerUnreachableException>()
//        //    //    .Or<SocketException>()
//        //    //    .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
//        //    //    {
//        //    //        //_logger.LogWarning(ex.ToString());
//        //    //        _logger.Warn(ex.ToString());
//        //    //    });

//        //    var policy = Policy.Handle<BrokerUnreachableException>()
//        //        .Or<SocketException>()
//        //        .Or<Exception>()
//        //        .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
//        //        {
//        //            //_logger.LogWarning(ex.ToString());
//        //            _logger.LogWarning(ex.ToString());
//        //        });

//        //    using var channel = _persistentConnection.CreateModel();
//        //    //var eventName = @event.GetType().Name;
//        //    //var eventName = @event.RoutingKey;
//        //    var routingKey = @event.RoutingKey;
//        //    //if (string.IsNullOrEmpty(@event.RoutingKey))
//        //    //{

//        //    //}
//        //    //else
//        //    //{
//        //    //    routingKey = eventName + "." + @event.RoutingKey;
//        //    //}
//        //    //_logger.Info("Publish RoutingKey: " + routingKey);
//        //    channel.ExchangeDeclare(_exchange, _type);
//        //    //channel.ExchangeDeclare(BrokerName, _type);
//        //    //channel.ExchangeDeclare(BrokerName,"direct");
//        //    //channel.ExchangeDeclare(BrokerName,"direct", true, true);

//        //    //var message = JsonConvert.SerializeObject(@event);
//        //    //var body = Encoding.UTF8.GetBytes(message);

//        //    var ms = new MemoryStream();
//        //    Serializer.Serialize(ms, @event);
//        //    var body = ms.ToArray();

//        //    policy.Execute(() =>
//        //    {
//        //        var properties = channel.CreateBasicProperties();
//        //        properties.DeliveryMode = 1; //2 = persistent, write on disk
//        //        //properties.ReplyTo

//        //        //channel.BasicPublish(BrokerName,eventName,true,properties,body);
//        //        //channel.BasicPublish(BrokerName,routingKey,true,properties,body);
//        //        channel.BasicPublish(_exchange, routingKey, true, properties, body);
//        //    });
//        //}

//        //public void PublishRpc(IIntegrationEvent @event)
//        //{
//        //    if (!_persistentConnection.IsConnected)
//        //    {
//        //        _persistentConnection.TryConnect();
//        //    }

//        //    var policy = Policy.Handle<BrokerUnreachableException>()
//        //        .Or<SocketException>()
//        //        .Or<Exception>()
//        //        .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
//        //        {
//        //            //_logger.LogWarning(ex.ToString());
//        //            _logger.LogWarning(ex.ToString());
//        //        });

//        //    using (var channel = _persistentConnection.CreateModel())
//        //    {
//        //        var routingKey = @event.RoutingKey;
//        //        var replyQueueName = channel.QueueDeclare().QueueName;

//        //        channel.ExchangeDeclare(_exchange, _type);
//        //        //channel.ExchangeDeclare(BrokerName, _type);
//        //        //channel.ExchangeDeclare(BrokerName,"direct");
//        //        //channel.ExchangeDeclare(BrokerName,"direct", true, true);

//        //        var message = JsonConvert.SerializeObject(@event);
//        //        var body = Encoding.UTF8.GetBytes(message);
//        //        var correlationId = Guid.NewGuid().ToString();

//        //        policy.Execute(() =>
//        //        {
//        //            var properties = channel.CreateBasicProperties();
//        //            properties.DeliveryMode = 1; //2 = persistent, write on disk
//        //            properties.CorrelationId = correlationId;
//        //            properties.ReplyTo = replyQueueName;

//        //            //properties.ReplyTo

//        //            //channel.BasicPublish(BrokerName,eventName,true,properties,body);
//        //            //channel.BasicPublish(BrokerName,routingKey,true,properties,body);
//        //            channel.BasicPublish(_exchange, routingKey, true, properties, body);
//        //        });
//        //    }
//        //}

//        #region [Subscribe]

//        public void SubscribeDynamic<TH>(string eventName)
//            where TH : IDynamicIntegrationEventHandler
//        {
//            DoInternalSubscription(eventName);
//            _subsManager.AddDynamicSubscription<TH>(eventName);
//        }

//        public void Subscribe<T, TH>()
//            where T : IIntegrationEvent
//            where TH : IIntegrationEventHandler<T>
//        {

//            var eventName = _subsManager.GetEventKey<T>();
//            DoInternalSubscription(eventName);
//            _subsManager.AddSubscription<T, TH>();
//        }

//        public void Subscribe<T, TH>(string routingKey)
//            where T : IIntegrationEvent
//            where TH : IIntegrationEventHandler<T>
//        {

//            var eventName = _subsManager.GetEventKey<T>();
//            DoInternalSubscription(eventName + "." + routingKey);
//            _subsManager.AddSubscription<T, TH>(eventName + "." + routingKey);
//        }

//        public void SubscribeQueue<T, TH>(string routingKey)
//            where T : IIntegrationEvent
//            where TH : IIntegrationQueueHandler<T>
//        {
//            var eventName = _subsManager.GetEventKey<T>();
//            DoInternalSubscription(eventName + "." + routingKey);
//            _subsManager.AddSubscriptionQueue<T, TH>(eventName + "." + routingKey);
//        }

//        public void SubscribeRpc<T, TR, TH>(string routingKey)
//            where T : IIntegrationEvent
//            where TR : IIntegrationEventReply
//            where TH : IIntegrationRpcHandler<T, TR>
//        {
//            var eventName = _subsManager.GetEventKey<T>();
//            var eventNameResult = _subsManager.GetEventReplyKey<TR>();
//            _logger.LogDebug("SubscribeRpc: eventName: " + eventName + "." + routingKey + " eventNameResult: " + eventNameResult + "." + routingKey);
//            DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey);
//            _subsManager.AddSubscriptionRpc<T, TR, TH>(eventName + "." + routingKey, eventNameResult + "." + routingKey);
//        }

//        private void DoInternalSubscription(string eventName)
//        {
//            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
//            if (containsKey) return;
//            if (!_persistentConnection.IsConnected)
//            {
//                _persistentConnection.TryConnect();
//            }

//            using var channel = _persistentConnection.CreateModel();
//            channel.QueueBind(_queueName, _exchange, eventName);
//        }

//        private void DoInternalSubscriptionRpc(string eventName, string eventNameResult)
//        {
//            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
//            if (containsKey) return;
//            if (!_persistentConnection.IsConnected)
//            {
//                _persistentConnection.TryConnect();
//            }

//            using var channel = _persistentConnection.CreateModel();
//            channel.QueueBind(_queueName, _exchange, eventName);
//            channel.QueueBind(_queueNameReply, _exchange, eventNameResult);
//        }

//        #endregion

//        #region [Unsubscribe]

//        public void Unsubscribe<T, TH>()
//            where TH : IIntegrationEventHandler<T>
//            where T : IIntegrationEvent
//        {
//            _subsManager.RemoveSubscription<T, TH>();
//        }

//        public void Unsubscribe<T, TH>(string routingKey)
//            where TH : IIntegrationEventHandler<T>
//            where T : IIntegrationEvent
//        {
//            _subsManager.RemoveSubscription<T, TH>(routingKey);
//        }

//        public void UnsubscribeQueue<T, TH>(string routingKey)
//            where TH : IIntegrationQueueHandler<T>
//            where T : IIntegrationEvent
//        {
//            _subsManager.RemoveSubscriptionQueue<T, TH>(routingKey);
//        }

//        public void UnsubscribeRpc<T, TR, TH>(string routingKey)
//            where TH : IIntegrationRpcHandler<T, TR>
//            where T : IIntegrationEvent
//            where TR : IIntegrationEventReply
//        {
//            _subsManager.RemoveSubscriptionRpc<T, TR, TH>(routingKey);
//        }

//        public void UnsubscribeDynamic<TH>(string eventName)
//            where TH : IDynamicIntegrationEventHandler
//        {
//            _subsManager.RemoveDynamicSubscription<TH>(eventName);
//        }

//        #endregion

//        protected override void DisposeManagedResources()
//        {
//            _consumerChannelReply?.Dispose();
//            _consumerChannel?.Dispose();
//            _subsManager?.Clear();
//            _subsManager?.ClearReply();
//        }

//        private IModel CreateConsumerChannel(CancellationToken cancel = default)
//        {
//            if (!_persistentConnection.IsConnected)
//            {
//                _persistentConnection.TryConnect();
//            }

//            var channel = _persistentConnection.CreateModel();

//            channel.ExchangeDeclare(_exchange, _type);

//            channel.QueueDeclare(_queueName, true, false, false, null);
//            channel.BasicQos(0, 1, false);

//            var consumer = new EventingBasicConsumer(channel);

//            consumer.Received += async (model, ea) =>
//            {
//                try
//                {
//                    string[] result = ea.RoutingKey.Split('.');
//                    var eventName = result.Length > 1 ? result[0] : ea.RoutingKey;

//                    //_logger.LogTrace("CreateConsumerChannel eventName: " + eventName);
                    
//                    if (_rpcFlag)
//                    {
//                        try
//                        {
//                            var props = ea.BasicProperties;
//                            //_logger.LogCritical("1");
//                            var replyProps = channel.CreateBasicProperties();
//                            //_logger.LogCritical("2");
//                            replyProps.CorrelationId = props.CorrelationId;
//                            //_logger.LogCritical("3");

//                            var response = await ProcessEventRpc(ea.RoutingKey, eventName, ea.Body, cancel);
//                            //if (response is null)
//                            //{
//                            //    _logger.LogCritical("4 response null");
//                            //}
//                            //_logger.LogCritical("4" );

//                            //safeValue
//                            //ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(IIntegrationEventResult), true)
//                            //    .AddSubType(612, response.GetType());
//                            //_logger.LogInformation("CreateConsumerChannel RPC Received: ");
//                            var ms = new MemoryStream();
//                            //_logger.LogCritical("5");
//                            Serializer.Serialize<IIntegrationEventReply>(ms, response);
//                            //_logger.LogCritical("6");
//                            var body = ms.ToArray();
//                            //_logger.LogCritical("7");
//                            //_logger.LogInformation("CreateConsumerChannel RPC Received: " + props.ReplyTo + " - " + props.CorrelationId + " " + response.GetType());
//                            //_logger.LogInformation("CreateConsumerChannel RPC Received: " + props.ReplyTo + " - " + props.CorrelationId + " " + (string) response.RoutingKey);
//                            //channel.BasicPublish(_exchange, props.ReplyTo, replyProps, body);
//                            channel.BasicPublish(_exchange, (string) response.RoutingKey, replyProps, body);
//                        }
//                        catch (Exception ex)
//                        {
//                            _logger.LogError("CreateConsumerChannel RPC Received: " + ex.Message + " - " + ex.StackTrace);
//                        }
//                    }
//                    else
//                    {
//                        await ProcessEvent(ea.RoutingKey, eventName, ea.Body, cancel);
//                    }

//                    channel.BasicAck(ea.DeliveryTag, false);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError("CreateConsumerChannel Received: " + ex.Message + " - " + ex.StackTrace);
//                }
//            };

//            channel.BasicConsume(_queueName, false, consumer);
//            //channel.BasicConsume(_queueName,true,consumer);

//            channel.CallbackException += (sender, ea) =>
//            {
//                _logger.LogError("CallbackException: " + ea.Exception.Message);
//                _consumerChannel.Dispose();
//                _consumerChannel = CreateConsumerChannel(cancel);
//            };

//            return channel;
//        }

//        private IModel CreateConsumerChannelReply(CancellationToken cancel = default)
//        {
//            if (!_persistentConnection.IsConnected)
//            {
//                _persistentConnection.TryConnect();
//            }

//            var channel = _persistentConnection.CreateModel();

//            channel.ExchangeDeclare(_exchange, _type);

//            channel.QueueDeclare(_queueNameReply, true, false, false, null);
//            channel.BasicQos(0, 1, false);

//            var consumer = new EventingBasicConsumer(channel);

//            consumer.Received += async (model, ea) =>
//            {
//                try
//                {
//                    string[] result = ea.RoutingKey.Split('.');
//                    var eventName = result.Length > 1 ? result[0] : ea.RoutingKey;

//                    //_logger.LogCritical("CreateConsumerChannelRpc eventName: " + eventName);

//                    //_logger.LogInformation("CreateConsumerChannelRpc: " + ea.BasicProperties.CorrelationId + " - " + _correlationId);
//                    if (!ea.BasicProperties.CorrelationId.Equals(_correlationId)) return;
//                    //_logger.LogInformation("CreateConsumerChannelRpc: " + ea.BasicProperties.CorrelationId + " " + ea.RoutingKey);
//                    await ProcessEventReply(ea.RoutingKey, eventName, ea.Body, cancel);
//                    channel.BasicAck(ea.DeliveryTag, false);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError("CreateConsumerChannelRpc Received: " + ex.Message + " - " + ex.StackTrace);
//                }
//            };

//            channel.BasicConsume(_queueNameReply, false, consumer);

//            channel.CallbackException += (sender, ea) =>
//            {
//                _logger.LogError("CallbackException Rpc: " + ea.Exception.Message);
//                _consumerChannelReply.Dispose();
//                _consumerChannelReply = CreateConsumerChannelReply(cancel);
//            };

//            return channel;
//        }

//        //private IModel CreateConsumerChannelRpc(CancellationToken cancel)
//        //{
//        //    if (!_persistentConnection.IsConnected)
//        //    {
//        //        _persistentConnection.TryConnect();
//        //    }

//        //    var channel = _persistentConnection.CreateModel();

//        //    channel.ExchangeDeclare(_exchange, _type);
//        //    //channel.ExchangeDeclare(BrokerName, _type);
//        //    //channel.ExchangeDeclare(BrokerName,"direct");
//        //    //channel.ExchangeDeclare(BrokerName,"direct", true, true);

//        //    channel.QueueDeclare(_queueName, true, false, false, null);
//        //    channel.BasicQos(0, 1, false);

//        //    //channel.QueueBind(_queueName, _exchange, "");

//        //    var consumer = new EventingBasicConsumer(channel);


//        //    consumer.Received += async (model, ea) =>
//        //    {
//        //        //ToDo
//        //        try
//        //        {
//        //            //string[] test = ea.RoutingKey.Split('.');
//        //            //var eventName = ea.RoutingKey;

//        //            string[] result = ea.RoutingKey.Split('.');
//        //            var eventName = result.Length > 1 ? result[0] : ea.RoutingKey;
//        //            //_logger.LogTrace("CreateConsumerChannelRpc eventName: " + eventName);
//        //            //_logger.Info("CreateConsumerChannel eventName: " + eventName);
//        //            //if (test.Length > 1)
//        //            //{
//        //            //    eventName = test[0];
//        //            //}

//        //            //var message = Encoding.UTF8.GetString(ea.Body);

//        //            var props = ea.BasicProperties;
//        //            var replyProps = channel.CreateBasicProperties();
//        //            replyProps.CorrelationId = props.CorrelationId;

//        //            var response = await ProcessEventRpc(ea.RoutingKey, eventName, ea.Body/*message*/, cancel);
//        //            //var messageResponse = JsonConvert.SerializeObject(response);
//        //            //var body = Encoding.UTF8.GetBytes(messageResponse);

//        //            var ms = new MemoryStream();
//        //            Serializer.Serialize(ms, response);
//        //            var body = ms.ToArray();

//        //            //ToDo
//        //            //var responseBytes = Encoding.UTF8.GetBytes(response);
//        //            //Response
//        //            //var responseByte = Encoding.UTF8.GetBytes("");

//        //            channel.BasicPublish(_exchange, props.ReplyTo, replyProps, body);

//        //            channel.BasicAck(ea.DeliveryTag, false);
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            _logger.LogError("CreateConsumerChannelRpc: " + ex.Message + " - " + ex.StackTrace);
//        //        }
//        //    };

//        //    channel.BasicConsume(_queueName, false, consumer);
//        //    //channel.BasicConsume(_queueName,true,consumer);

//        //    channel.CallbackException += (sender, ea) =>
//        //    {
//        //        _consumerChannel.Dispose();
//        //        _consumerChannel = CreateConsumerChannelRpc(cancel);
//        //    };

//        //    return channel;
//        //}

//        //private async ValueTask ProcessEvent(string routingKey, string eventName, string message, CancellationToken cancel)

//        private async ValueTask ProcessEventReply(string routingKey, string eventName, byte[] message,
//            CancellationToken cancel)
//        {
//            if (_subsManager.HasSubscriptionsForEventReply(routingKey))
//            {
//                var subscriptions = _subsManager.GetHandlersForEventReply(routingKey);
//                if (!subscriptions.Any())
//                {
//                    _logger.LogError("ProcessEventReply subscriptions no items! " + routingKey);
//                }
//                foreach (var subscription in subscriptions)
//                {
//                    switch (subscription.SubscriptionManagerType)
//                    {
//                        case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Rpc:
//                            try
//                            {
//                                if (EventHandler is null)
//                                {
//                                    _logger.LogError("ProcessEventReply _eventHandler is null!");
//                                }
//                                else
//                                {

//                                    var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                    if (eventType is null)
//                                    {
//                                        _logger.LogError("ProcessEventReply: eventType is null! " + routingKey);
//                                        return;
//                                    }

//                                    //_logger.LogWarning("ProcessEventReply: " + eventType.FullName);
//                                    var eventResultType = _subsManager.GetEventReplyTypeByName(routingKey);
//                                    if (eventResultType is null)
//                                    {
//                                        _logger.LogError("ProcessEventReply: eventResultType is null! " + routingKey);
//                                        return;
//                                    }
//                                    //_logger.LogWarning("ProcessEventReply: " + eventResultType.FullName);
//                                    //var eventReplyType = _subsManager.GetEventReplyTypeByName(routingKey);
//                                    //_logger.LogWarning("ProcessEventRpc: " + eventReplyType.FullName);
//                                    await using var ms = new MemoryStream(message);
//                                    //_logger.LogWarning("ProcessEventReply: ms");
//                                    var integrationEvent = Serializer.Deserialize(eventResultType, ms);
//                                    //var integrationEvent2 = Serializer.Deserialize<T>(ms);
//                                    //var integrationEvent = Serializer.Deserialize(eventType, ms);
//                                    //_logger.LogWarning("ProcessEventReply: integrationEvent " + integrationEvent.GetType().FullName);
//                                    var concreteType = typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType, eventResultType);
//                                    //_logger.LogWarning("ProcessEventReply: " + concreteType.FullName);
//                                    await (ValueTask)concreteType.GetMethod("HandleReply")
//                                        .Invoke(EventHandler, new[] { integrationEvent, cancel });

//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                _logger.LogError("ProcessEventReply: " + ex.Message + " - " + ex.StackTrace);
//                            }
//                            break;

//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//                }
//            }
//            else
//            {
//                _logger.LogError("ProcessEventReply HasSubscriptionsForEventReply " + routingKey + " No Subscriptions!");
//            }
//        }

//        private async ValueTask ProcessEvent(string routingKey, string eventName, byte[] message, CancellationToken cancel)
//        {
//            if (_subsManager.HasSubscriptionsForEvent(routingKey))
//            {
//                if (_autofac != null)
//                {
//                    await using var scope = _autofac.BeginLifetimeScope(AutofacScopeName);
//                    var subscriptions = _subsManager.GetHandlersForEvent(routingKey);
//                    foreach (var subscription in subscriptions)
//                    {
//                        //if (subscription.IsDynamic)
//                        //{
//                        //    var handler =
//                        //        scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;

//                        //    //ToDo
//                        //    //dynamic eventData = JObject.Parse(message);

//                        //    //await using var ms = new MemoryStream(message);
//                        //    //var integrationEvent = Serializer.Deserialize(eventType, ms);

//                        //    //await handler.Handle(eventData);
//                        //}
//                        //else
//                        //{
//                        //    var eventType = _subsManager.GetEventTypeByName(routingKey);
//                        //    //var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

//                        //    await using var ms = new MemoryStream(message);
//                        //    var integrationEvent = Serializer.Deserialize(eventType, ms);

//                        //    var handler = scope.ResolveOptional(subscription.HandlerType);
//                        //    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
//                        //    await (ValueTask) concreteType.GetMethod("Handle")
//                        //        .Invoke(handler, new[] {integrationEvent});
//                        //}

//                        switch (subscription.SubscriptionManagerType)
//                        {
//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Dynamic:
//                                //var handler =
//                                //    scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;

//                                //ToDo
//                                //dynamic eventData = JObject.Parse(message);

//                                //await using var ms = new MemoryStream(message);
//                                //var integrationEvent = Serializer.Deserialize(eventType, ms);

//                                //await handler.Handle(eventData);
//                                break;

//                            default:
//                            {
//                                var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                //var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

//                                await using var ms = new MemoryStream(message);
//                                var integrationEvent = Serializer.Deserialize(eventType, ms);

//                                var handler = scope.ResolveOptional(subscription.HandlerType);
//                                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
//                                await (ValueTask)concreteType.GetMethod("Handle")
//                                    .Invoke(handler, new[] { integrationEvent, cancel });
//                                break;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    var subscriptions = _subsManager.GetHandlersForEvent(routingKey);
//                    foreach (var subscription in subscriptions)
//                    {

//                        switch (subscription.SubscriptionManagerType)
//                        {
//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Dynamic:
//                                break;

//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Typed:
//                                try
//                                {
//                                    if (EventHandler is null)
//                                    {

//                                    }
//                                    else
//                                    {
//                                        var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                        await using var ms = new MemoryStream(message);
//                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
//                                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
//                                        await (ValueTask)concreteType.GetMethod("Handle")
//                                            .Invoke(EventHandler, new[] { integrationEvent, cancel });

//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    _logger.LogError("ProcessEvent: " + ex.Message + " - " + ex.StackTrace);
//                                }
//                                break;

//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Queue:
//                                try
//                                {
//                                    if (EventHandler is null)
//                                    {

//                                    }
//                                    else
//                                    {
//                                        var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                        await using var ms = new MemoryStream(message);
//                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
//                                        var concreteType = typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);
//                                        await (ValueTask<bool>)concreteType.GetMethod("Enqueue")
//                                            .Invoke(EventHandler, new[] { integrationEvent, cancel });
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    _logger.LogError("ProcessQueue: " + ex.Message + " - " + ex.StackTrace);
//                                }
//                                break;

//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Rpc:
//                                try
//                                {
//                                    if (EventHandler is null)
//                                    {

//                                    }
//                                    else
//                                    {
//                                        var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                        if (eventType is null)
//                                        {
//                                            _logger.LogError("ProcessEvent: eventType is null! " + routingKey);
//                                            return;
//                                        }

//                                        //_logger.LogWarning("ProcessEvent: " + eventType.FullName);
//                                        var eventResultType = _subsManager.GetEventReplyTypeByName(routingKey);
//                                        if (eventResultType is null)
//                                        {
//                                            _logger.LogError("ProcessEvent: eventResultType is null! " + routingKey);
//                                            return;
//                                        }
//                                        //_logger.LogWarning("ProcessEventRpc: " + eventResultType.FullName);
//                                        //var eventReplyType = _subsManager.GetEventReplyTypeByName(routingKey);
//                                        //_logger.LogWarning("ProcessEventRpc: " + eventReplyType.FullName);
//                                        await using var ms = new MemoryStream(message);
//                                        //_logger.LogWarning("ProcessEventRpc: ms");
//                                        var integrationEvent = Serializer.Deserialize(eventResultType, ms);
//                                        //var integrationEvent2 = Serializer.Deserialize<T>(ms);
//                                        //var integrationEvent = Serializer.Deserialize(eventType, ms);
//                                        //_logger.LogWarning("ProcessEventRpc: integrationEvent " + integrationEvent.GetType().FullName);
//                                        var concreteType = typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType, eventResultType);
//                                        //_logger.LogWarning("ProcessEvent: " + concreteType.FullName);
//                                        await (ValueTask)concreteType.GetMethod("HandleReply")
//                                            .Invoke(EventHandler, new[] { integrationEvent, cancel });

//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    _logger.LogError("ProcessEvent: " + ex.Message + " - " + ex.StackTrace);
//                                }
//                                break;

//                            default:
//                                throw new ArgumentOutOfRangeException();
//                        }
//                    }
//                }
//            }
//        }//ProcessEvent.

//        private async ValueTask<dynamic> ProcessEventRpc(string routingKey, string eventName, byte[] message, CancellationToken cancel)
//        {
//            dynamic output = null;

//            if (_subsManager.HasSubscriptionsForEvent(routingKey))
//            {
//                if (_autofac != null)
//                {
//                    await using var scope = _autofac.BeginLifetimeScope(AutofacScopeName);
//                    var subscriptions = _subsManager.GetHandlersForEvent(routingKey);
//                    foreach (var subscription in subscriptions)
//                    {
//                        //if (subscription.IsDynamic)
//                        //{
//                        //    //ToDo
//                        //    //var handler =
//                        //    //    scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
//                        //    //dynamic eventData = JObject.Parse(message);
//                        //    //await handler.Handle(eventData);
//                        //}
//                        //else
//                        //{
//                        //    var eventType = _subsManager.GetEventTypeByName(routingKey);
//                        //    //var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
//                        //    await using var ms = new MemoryStream(message);
//                        //    var integrationEvent = Serializer.Deserialize(eventType, ms);

//                        //    var handler = scope.ResolveOptional(subscription.HandlerType);
//                        //    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
//                        //    await (ValueTask)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent, cancel });
//                        //}

//                        switch (subscription.SubscriptionManagerType)
//                        {
//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Dynamic:
//                                //var handler =
//                                //    scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;

//                                //ToDo
//                                //dynamic eventData = JObject.Parse(message);

//                                //await using var ms = new MemoryStream(message);
//                                //var integrationEvent = Serializer.Deserialize(eventType, ms);

//                                //await handler.Handle(eventData);
//                                break;

//                            default:
//                            {
//                                var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                //var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

//                                await using var ms = new MemoryStream(message);
//                                var integrationEvent = Serializer.Deserialize(eventType, ms);

//                                var handler = scope.ResolveOptional(subscription.HandlerType);
//                                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
//                                await (ValueTask)concreteType.GetMethod("Handle")
//                                    .Invoke(handler, new[] { integrationEvent, cancel });
//                                break;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    var subscriptions = _subsManager.GetHandlersForEvent(routingKey);

//                    if (!subscriptions.Any())
//                    {
//                        _logger.LogError("ProcessEventRpc subscriptions no items! " + routingKey);
//                    }
//                    foreach (var subscription in subscriptions)
//                    {
//                        switch (subscription.SubscriptionManagerType)
//                        {
//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Dynamic:
//                                break;

//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Typed:
//                                try
//                                {
//                                    if (EventHandler is null)
//                                    {

//                                    }
//                                    else
//                                    {
//                                        var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                        await using var ms = new MemoryStream(message);
//                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
//                                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
//                                        await (ValueTask)concreteType.GetMethod("Handle")
//                                            .Invoke(EventHandler, new[] { integrationEvent, cancel });

//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    _logger.LogError("ProcessEvent: " + ex.Message + " - " + ex.StackTrace);
//                                }
//                                break;

//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Queue:
//                                try
//                                {
//                                    if (EventHandler is null)
//                                    {

//                                    }
//                                    else
//                                    {
//                                        var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                        await using var ms = new MemoryStream(message);
//                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
//                                        var concreteType = typeof(IIntegrationQueueHandler<>).MakeGenericType(eventType);

//                                        await (Task<bool>)concreteType.GetMethod("Enqueue")
//                                            .Invoke(EventHandler, new[] { integrationEvent, cancel });
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    _logger.LogError("ProcessQueue: " + ex.Message + " - " + ex.StackTrace);
//                                }
//                                break;

//                            case InMemoryEventBusSubscriptionsManager.SubscriptionManagerType.Rpc:
//                                try
//                                {
//                                    if (EventHandler is null)
//                                    {
//                                        _logger.LogError("ProcessEventRpc _eventHandler is null!");
//                                    }
//                                    else
//                                    {
//                                        var eventType = _subsManager.GetEventTypeByName(routingKey);
//                                        if (eventType is null)
//                                        {
//                                            _logger.LogError("ProcessEventRpc: eventType is null! " + routingKey);
//                                            return null;
//                                        }
//                                        var eventReplyType = _subsManager.GetEventReplyTypeByName(routingKey);
//                                        if (eventReplyType is null)
//                                        {
//                                            _logger.LogError("ProcessEventRpc: eventReplyType is null! " + routingKey);
//                                            return null;
//                                        }

//                                        await using var ms = new MemoryStream(message);
//                                        var integrationEvent = Serializer.Deserialize(eventType, ms);
//                                        var concreteType = typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType, eventReplyType);

//                                        output = await (dynamic)concreteType.GetMethod("HandleRpc").Invoke(EventHandler, new[] { integrationEvent, cancel });

//                                        if (output is null)
//                                        {
//                                            _logger.LogError("ProcessEventRpc output is null!");
//                                        }
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    _logger.LogError("ProcessEventRpc: " + ex.Message + " - " + ex.StackTrace);
//                                }
//                                break;

//                            default:
//                                throw new ArgumentOutOfRangeException();
//                        }
//                    }
//                }
//            }
//            else
//            {
//                _logger.LogError("ProcessEventRpc HasSubscriptionsForEvent " + routingKey + " No Subscriptions!");
//            }
//            return output;
//        }//ProcessEventRpc.
//    }
//}
