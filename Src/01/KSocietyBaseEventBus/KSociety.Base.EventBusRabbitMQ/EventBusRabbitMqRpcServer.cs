using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KSociety.Base.EventBusRabbitMQ
{
    public sealed class EventBusRabbitMqRpcServer : EventBusRabbitMq, IEventBusRpcServer
    {
        private IModel _consumerChannelReply;
        private string _queueNameReply;

        #region [Constructor]

        public EventBusRabbitMqRpcServer(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
            IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
            IExchangeDeclareParameters exchangeDeclareParameters,
            IQueueDeclareParameters queueDeclareParameters,
            string queueName = null,
            CancellationToken cancel = default)
            : base(persistentConnection, loggerFactory, eventHandler, subsManager, exchangeDeclareParameters, queueDeclareParameters, queueName, cancel)
        {
            SubsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;
            ConsumerChannel = CreateConsumerChannel(cancel);
            _queueNameReply = QueueName + "_Reply";
            _consumerChannelReply = CreateConsumerChannelReply(cancel);
        }

        #endregion

        public IIntegrationRpcServerHandler<T, TR> GetIntegrationRpcServerHandler<T, TR>()
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
        {
            if (!(EventHandler is null) && EventHandler is IIntegrationRpcServerHandler<T, TR> queue)
            {
                return queue;
            }

            return null;
        }

        private void SubsManager_OnEventReplyRemoved(object sender, string eventName)
        {
            if (!PersistentConnection.IsConnected)
            {
                PersistentConnection.TryConnect();
            }

            using (var channel = PersistentConnection.CreateModel())
            {
                channel.QueueUnbind(QueueName, ExchangeDeclareParameters.ExchangeName, eventName);

                channel.QueueUnbind(_queueNameReply, ExchangeDeclareParameters.ExchangeName, eventName);
            }

            if (!SubsManager.IsReplyEmpty) return;

            QueueName = string.Empty;
            ConsumerChannel?.Close();

            //ToDo

            _queueNameReply = string.Empty;
            _consumerChannelReply?.Close();

        }

        #region [Subscribe]

        public void SubscribeRpcServer<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>
        {
            var eventName = SubsManager.GetEventKey<T>();
            var eventNameResult = SubsManager.GetEventReplyKey<TR>();
            //Logger.LogDebug("SubscribeRpc: eventName: " + eventName + "." + routingKey + " eventNameResult: " + eventNameResult + "." + routingKey);
            DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey);
            SubsManager.AddSubscriptionRpcServer<T, TR, TH>(eventName + "." + routingKey, eventNameResult + "." + routingKey);
            StartBasicConsume();
            //StartBasicConsumeReply();
        }

        private void DoInternalSubscriptionRpc(string eventName, string eventNameResult)
        {
            var containsKey = SubsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey) return;
            if (!PersistentConnection.IsConnected)
            {
                PersistentConnection.TryConnect();
            }

            using var channel = PersistentConnection.CreateModel();
            channel.QueueBind(QueueName, ExchangeDeclareParameters.ExchangeName, eventName);
            channel.QueueBind(_queueNameReply, ExchangeDeclareParameters.ExchangeName, eventNameResult);
        }

        #endregion

        #region [Unsubscribe]

        public void UnsubscribeRpcServer<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TH : IIntegrationRpcServerHandler<T, TR>
            where TR : IIntegrationEventReply
        {
            SubsManager.RemoveSubscriptionRpcServer<T, TR, TH>(routingKey);
        }

        #endregion

        protected override void DisposeManagedResources()
        {
            _consumerChannelReply?.Dispose();
            ConsumerChannel?.Dispose();
            SubsManager?.Clear();
            SubsManager?.ClearReply();
        }

        protected override void StartBasicConsume()
        {
            Logger.LogTrace("Starting RabbitMQ basic consume");

            if (ConsumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(ConsumerChannel);

                consumer.Received += ConsumerReceivedAsync;

                ConsumerChannel.BasicConsume(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                Logger.LogError("StartBasicConsume can't call on ConsumerChannel == null");
            }
        }

        protected override void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;
            //var test = eventArgs.BasicProperties.ReplyTo;
            try
            {
                var props = eventArgs.BasicProperties;
                var replyProps = ConsumerChannel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var response = ProcessEventRpc(eventArgs.RoutingKey, eventName, eventArgs.Body);
                var ms = new MemoryStream();
                Serializer.Serialize<IIntegrationEventReply>(ms, response);
                var body = ms.ToArray();
                //ConsumerChannel.BasicPublish(ExchangeDeclareParameters.ExchangeName, (string)response.RoutingKey, replyProps, body);
                _consumerChannelReply.BasicPublish(ExchangeDeclareParameters.ExchangeName, (string)response.RoutingKey, replyProps, body);
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateConsumerChannel RPC Received: " + ex.Message + " - " + ex.StackTrace);
            }


            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateConsumerChannel RPC Received 2: " + ex.Message + " - " + ex.StackTrace);
            }
        }

        protected override async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            string[] result = eventArgs.RoutingKey.Split('.');
            var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;
            //var test = eventArgs.BasicProperties.ReplyTo;
            //ConsumerChannel.
            try
            {
                var props = eventArgs.BasicProperties;
                var replyProps = ConsumerChannel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                    
                var response = await ProcessEventRpcAsync(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
                var ms = new MemoryStream();
                Serializer.Serialize<IIntegrationEventReply>(ms, response);
                //Serializer.Serialize(ms, response);
                var body = ms.ToArray();
                //ConsumerChannel.BasicPublish(ExchangeDeclareParameters.ExchangeName, (string)response.RoutingKey, replyProps, body);
                _consumerChannelReply.BasicPublish(ExchangeDeclareParameters.ExchangeName, (string)response.RoutingKey, replyProps, body);
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateConsumerChannel RPC Received: " + ex.Message + " - " + ex.StackTrace);
            }

            try
            {

                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError("CreateConsumerChannel RPC Received 2: " + ex.Message + " - " + ex.StackTrace);
            }
        }

        protected override IModel CreateConsumerChannel(CancellationToken cancel = default)
        {
            if (!PersistentConnection.IsConnected)
            {
                PersistentConnection.TryConnect();
            }

            var channel = PersistentConnection.CreateModel();

            channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType, ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

            channel.QueueDeclare(QueueName, QueueDeclareParameters.QueueDurable, QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
            channel.BasicQos(0, 1, false);

            channel.CallbackException += (sender, ea) =>
            {
                Logger.LogError("CallbackException: " + ea.Exception.Message);
                ConsumerChannel.Dispose();
                ConsumerChannel = CreateConsumerChannel(cancel);
                StartBasicConsume();
            };

            return channel;
        }

        private IModel CreateConsumerChannelReply(CancellationToken cancel = default)
        {
            if (!PersistentConnection.IsConnected)
            {
                PersistentConnection.TryConnect();
            }

            var channel = PersistentConnection.CreateModel();

            channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType, ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

            channel.QueueDeclare(_queueNameReply, QueueDeclareParameters.QueueDurable, QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
            channel.BasicQos(0, 1, false);

            channel.CallbackException += (sender, ea) =>
            {
                Logger.LogError("CallbackException Rpc: " + ea.Exception.Message);
                _consumerChannelReply.Dispose();
                _consumerChannelReply = CreateConsumerChannelReply(cancel);
                //StartBasicConsumeReply();
            };

            return channel;
        }

        private dynamic ProcessEventRpc(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            dynamic output = null;

            if (SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                var subscriptions = SubsManager.GetHandlersForEvent(routingKey);

                if (!subscriptions.Any())
                {
                    Logger.LogError("ProcessEventRpc subscriptions no items! " + routingKey);
                }
                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Rpc:
                            break;
                        case SubscriptionManagerType.RpcClient:
                            break;
                        case SubscriptionManagerType.RpcServer:
                            try
                            {
                                if (EventHandler is null)
                                {
                                    Logger.LogError("ProcessEventRpcServer _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        Logger.LogError("ProcessEventRpcServer: eventType is null! " + routingKey);
                                        return null;
                                    }

                                    //Logger.LogInformation("ProcessEventRpcServer: eventType is: " + eventType.FullName);

                                    var eventReplyType = SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        Logger.LogError("ProcessEventRpcServer: eventReplyType is null! " + routingKey);
                                        return null;
                                    }

                                    //Logger.LogInformation("ProcessEventRpcServer: eventReplyType is: " + eventReplyType.FullName);

                                    using var ms = new MemoryStream(message.ToArray());
                                    var integrationEvent = Serializer.Deserialize(eventType, ms);
                                    var concreteType =
                                        typeof(IIntegrationRpcServerHandler<,>).MakeGenericType(eventType,
                                            eventReplyType);

                                    //output = (dynamic)concreteType.GetMethod("HandleRpc")
                                    //    .Invoke(EventHandler, new[] { integrationEvent, cancel });

                                    output = concreteType.GetMethod("HandleRpc")
                                        .Invoke(EventHandler, new[] { integrationEvent, cancel });

                                    if (output is null)
                                    {
                                        Logger.LogError("ProcessEventRpcServer output is null!");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("ProcessEventRpcServer: " + ex.Message + " - " + ex.StackTrace);
                            }
                            break;

                        case SubscriptionManagerType.Dynamic:
                            break;
                        case SubscriptionManagerType.Typed:
                            break;
                        case SubscriptionManagerType.Queue:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

            }
            else
            {
                Logger.LogError("ProcessEventRpc HasSubscriptionsForEvent " + routingKey + " No Subscriptions!");
            }
            return output;
        }//ProcessEventRpc.

        private async ValueTask<dynamic> ProcessEventRpcAsync(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
        {
            dynamic output = null;

            if (SubsManager.HasSubscriptionsForEvent(routingKey))
            {
                var subscriptions = SubsManager.GetHandlersForEvent(routingKey);

                if (!subscriptions.Any())
                {
                    Logger.LogError("ProcessEventRpc subscriptions no items! " + routingKey);
                }
                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SubscriptionManagerType)
                    {
                        case SubscriptionManagerType.Rpc:
                            break;
                        case SubscriptionManagerType.RpcClient:
                            break;
                        case SubscriptionManagerType.RpcServer:
                            try
                            {
                                if (EventHandler is null)
                                {
                                    Logger.LogError("ProcessEventRpcServer _eventHandler is null!");
                                }
                                else
                                {
                                    var eventType = SubsManager.GetEventTypeByName(routingKey);
                                    if (eventType is null)
                                    {
                                        Logger.LogError("ProcessEventRpcServer: eventType is null! " + routingKey);
                                        return null;
                                    }
                                    //Logger.LogInformation("ProcessEventRpcServer: eventType is: " + eventType.FullName);

                                    var eventReplyType = SubsManager.GetEventReplyTypeByName(routingKey);
                                    if (eventReplyType is null)
                                    {
                                        Logger.LogError("ProcessEventRpcServer: eventReplyType is null! " + routingKey);
                                        return null;
                                    }
                                    //Logger.LogInformation("ProcessEventRpcServer: eventReplyType is: " + eventReplyType.FullName);

                                    await using var ms = new MemoryStream(message.ToArray());
                                    var integrationEvent = Serializer.Deserialize(eventType, ms);
                                    //Logger.LogInformation("integrationEvent: " 
                                    //                      + ((IIntegrationEventRpc)integrationEvent).RoutingKey + " "
                                    //                      + ((IIntegrationEventRpc)integrationEvent).ReplyRoutingKey);

                                    var concreteType =
                                        typeof(IIntegrationRpcServerHandler<,>).MakeGenericType(eventType,
                                            eventReplyType);

                                    output = await ((dynamic)concreteType.GetMethod("HandleRpcAsync")
                                        .Invoke(EventHandler, new[] { integrationEvent, cancel })).ConfigureAwait(false);
                                    
                                    if (output is null)
                                    {
                                        Logger.LogError("ProcessEventRpcServer output is null!");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("ProcessEventRpcServer: " + ex.Message + " - " + ex.StackTrace);
                            }
                            break;

                        case SubscriptionManagerType.Dynamic:
                            break;
                        case SubscriptionManagerType.Typed:
                            break;
                        case SubscriptionManagerType.Queue:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

            }
            else
            {
                Logger.LogError("ProcessEventRpc HasSubscriptionsForEvent " + routingKey + " No Subscriptions!");
            }
            return output;
        }//ProcessEventRpcAsync.
    }
}
