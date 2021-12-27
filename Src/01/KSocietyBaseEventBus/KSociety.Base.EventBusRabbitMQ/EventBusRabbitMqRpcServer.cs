using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KSociety.Base.EventBusRabbitMQ;

public sealed class EventBusRabbitMqRpcServer : EventBusRabbitMq, IEventBusRpcServer
{
    private AsyncLazy<IModel> _consumerChannelReply;
    private string _queueNameReply;

    #region [Constructor]

    public EventBusRabbitMqRpcServer(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
        IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
        IEventBusParameters eventBusParameters,
        string queueName = null,
        CancellationToken cancel = default)
        : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName, cancel)
    {

    }

    #endregion

    protected async override ValueTask InitializeAsync(CancellationToken cancel = default)
    {
        Logger.LogTrace("EventBusRabbitMqRpcServer InitializeAsync.");
        SubsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;
        ConsumerChannel = new AsyncLazy<IModel>(async () => await CreateConsumerChannelAsync(cancel));
        _queueNameReply = QueueName + "_Reply";

        _consumerChannelReply =
            new AsyncLazy<IModel>(async () => await CreateConsumerChannelReplyAsync(cancel));
    }

    public IIntegrationRpcServerHandler<T, TR> GetIntegrationRpcServerHandler<T, TR>()
        where T : IIntegrationEventRpc
        where TR : IIntegrationEventReply
    {
        if (EventHandler is not null && EventHandler is IIntegrationRpcServerHandler<T, TR> queue)
        {
            return queue;
        }

        return null;
    }

    private async void SubsManager_OnEventReplyRemoved(object sender, string eventName)
    {
        if (!PersistentConnection.IsConnected)
        {
            await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
        }

        using (var channel = PersistentConnection.CreateModel())
        {
            channel.QueueUnbind(QueueName, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);

            channel.QueueUnbind(_queueNameReply, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
        }

        if (!SubsManager.IsReplyEmpty) return;

        QueueName = string.Empty;
        (await ConsumerChannel).Close();

        //ToDo

        _queueNameReply = string.Empty;
        (await _consumerChannelReply).Close();

    }

    protected override void QueueInitialize(IModel channel)
    {
        try
        {
            channel.ExchangeDeclare(EventBusParameters.ExchangeDeclareParameters.ExchangeName, EventBusParameters.ExchangeDeclareParameters.ExchangeType,
                EventBusParameters.ExchangeDeclareParameters.ExchangeDurable, EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

            channel.QueueDeclare(_queueNameReply, EventBusParameters.QueueDeclareParameters.QueueDurable,
                EventBusParameters.QueueDeclareParameters.QueueExclusive, EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
        }
        catch (RabbitMQClientException rex)
        {
            Logger.LogError(rex, "EventBusRabbitMqRpcServer RabbitMQClientException QueueInitialize: ");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "EventBusRabbitMqRpcServer QueueInitialize: ");
        }
    }

    #region [Subscribe]

    public void SubscribeRpcServer<T, TR, TH>(string routingKey)
        where T : IIntegrationEventRpc
        where TR : IIntegrationEventReply
        where TH : IIntegrationRpcServerHandler<T, TR>
    {
        var eventName = SubsManager.GetEventKey<T>();
        var eventNameResult = SubsManager.GetEventReplyKey<TR>();
        Logger.LogTrace("SubscribeRpcServer routing key: {0}, event name: {1}, event name result: {2}", routingKey, eventName, eventNameResult);
        DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey);
        SubsManager.AddSubscriptionRpcServer<T, TR, TH>(eventName + "." + routingKey, eventNameResult + "." + routingKey);
        StartBasicConsume();
    }

    private async void DoInternalSubscriptionRpc(string eventName, string eventNameResult)
    {
        try
        {
            var containsKey = SubsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey) return;
            if (!PersistentConnection.IsConnected)
            {
                var connection = await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
            }

            using var channel = PersistentConnection.CreateModel();

            QueueInitialize(channel);

            channel.QueueBind(QueueName, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
            channel.QueueBind(_queueNameReply, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventNameResult);
        }
        catch (RabbitMQClientException rex)
        {
            Logger.LogError(rex, "EventBusRabbitMqRpcClient RabbitMQClientException DoInternalSubscriptionRpc: ");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "EventBusRabbitMqRpcServer DoInternalSubscriptionRpc: ");
        }
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
        _consumerChannelReply?.Value.Dispose();
        ConsumerChannel?.Value.Dispose();
        SubsManager?.Clear();
        SubsManager?.ClearReply();
    }

    protected override async ValueTask<bool> StartBasicConsume()
    {
        Logger.LogTrace("EventBusRabbitMqRpcServer Starting RabbitMQ basic consume.");

        try
        {
            if (ConsumerChannel is null) {Logger.LogWarning("EventBusRabbitMqRpcServer ConsumerChannel is null!"); return false;}
            if (ConsumerChannel?.Value is not null)
            {
                var consumer = new AsyncEventingBasicConsumer(await ConsumerChannel);

                consumer.Received += ConsumerReceivedAsync;

                (await ConsumerChannel).BasicConsume(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer);
                Logger.LogInformation("EventBusRabbitMqRpcServer StartBasicConsume done. Queue name: {0}, autoAck: {1}", QueueName, false);

                return true;
            }
            else
            {
                Logger.LogError("StartBasicConsume can't call on ConsumerChannel is null");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "StartBasicConsume: ");
        }
        return false;
    }

    protected override void ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        string[] result = eventArgs.RoutingKey.Split('.');
        var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

        try
        {
            var props = eventArgs.BasicProperties;
            var replyProps = ConsumerChannel?.Value.Result.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            var response = ProcessEventRpc(eventArgs.RoutingKey, eventName, eventArgs.Body);
            var ms = new MemoryStream();
            Serializer.Serialize<IIntegrationEventReply>(ms, response);
            var body = ms.ToArray();

            _consumerChannelReply?.Value.Result.BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName, (string)response.RoutingKey, replyProps, body);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateConsumerChannel RPC Received: ");
        }


        try
        {

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            ConsumerChannel?.Value.Result.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
        }
    }

    protected override async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        string[] result = eventArgs.RoutingKey.Split('.');
        var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

        try
        {
            var props = eventArgs.BasicProperties;
            var replyProps = (await ConsumerChannel).CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;
                    
            var response = await ProcessEventRpcAsync(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
            var ms = new MemoryStream();
            Serializer.Serialize<IIntegrationEventReply>(ms, response);

            var body = ms.ToArray();
            (await _consumerChannelReply).BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName, (string)response.RoutingKey, replyProps, body);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateConsumerChannel RPC Received: ");
        }

        try
        {

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            (await ConsumerChannel).BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateConsumerChannel RPC Received 2: ");
        }
    }

    //protected override IModel CreateConsumerChannel(CancellationToken cancel = default)
    //{
    //    if (!PersistentConnection.IsConnected)
    //    {
    //        PersistentConnection.TryConnect();
    //    }

    //    var channel = PersistentConnection.CreateModel();

    //    channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType, ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

    //    channel.QueueDeclare(QueueName, QueueDeclareParameters.QueueDurable, QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
    //    channel.BasicQos(0, 1, false);

    //    channel.CallbackException += (sender, ea) =>
    //    {
    //        Logger.LogError("CallbackException: " + ea.Exception.Message);
    //        ConsumerChannel.Dispose();
    //        ConsumerChannel = CreateConsumerChannel(cancel);
    //        StartBasicConsume();
    //    };

    //    return channel;
    //}

    protected async override ValueTask<IModel> CreateConsumerChannelAsync(CancellationToken cancel = default)
    {
        Logger.LogTrace("EventBusRabbitMqRpcServer CreateConsumerChannelAsync queue name: {0}", QueueName);
        if (!PersistentConnection.IsConnected)
        {
            await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
        }

        var channel = PersistentConnection.CreateModel();

        try
        {
            QueueInitialize(channel);
            channel.BasicQos(0, 1, false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateConsumerChannelAsync: ");
        }

        channel.CallbackException += async (sender, ea) =>
        {
            Logger.LogError(ea.Exception, "CallbackException: ");
            ConsumerChannel?.Value.Dispose();
            ConsumerChannel = new AsyncLazy<IModel>(async () => await CreateConsumerChannelAsync(cancel));
            await StartBasicConsume();
        };

        return channel;
    }

    //private IModel CreateConsumerChannelReply(CancellationToken cancel = default)
    //{
    //    if (!PersistentConnection.IsConnected)
    //    {
    //        PersistentConnection.TryConnect();
    //    }

    //    var channel = PersistentConnection.CreateModel();

    //    channel.ExchangeDeclare(ExchangeDeclareParameters.ExchangeName, ExchangeDeclareParameters.ExchangeType, ExchangeDeclareParameters.ExchangeDurable, ExchangeDeclareParameters.ExchangeAutoDelete);

    //    channel.QueueDeclare(_queueNameReply, QueueDeclareParameters.QueueDurable, QueueDeclareParameters.QueueExclusive, QueueDeclareParameters.QueueAutoDelete, null);
    //    channel.BasicQos(0, 1, false);

    //    channel.CallbackException += (sender, ea) =>
    //    {
    //        Logger.LogError("CallbackException Rpc: " + ea.Exception.Message);
    //        _consumerChannelReply.Dispose();
    //        _consumerChannelReply = CreateConsumerChannelReply(cancel);
    //        //StartBasicConsumeReply();
    //    };

    //    return channel;
    //}

    private async ValueTask<IModel> CreateConsumerChannelReplyAsync(CancellationToken cancel = default)
    {
        Logger.LogTrace("CreateConsumerChannelReplyAsync queue name: {0}", _queueNameReply);
        if (!PersistentConnection.IsConnected)
        {
            await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
        }

        var channel = PersistentConnection.CreateModel();

        try
        {
            QueueInitialize(channel);
            channel.BasicQos(0, 1, false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateConsumerChannelReplyAsync: ");
        }

        channel.CallbackException += async (sender, ea) =>
        {
            Logger.LogError(ea.Exception, "CallbackException Rpc: ");
            _consumerChannelReply?.Value.Dispose();
            _consumerChannelReply = new AsyncLazy<IModel>(async () => await CreateConsumerChannelReplyAsync(cancel));
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
                Logger.LogError("ProcessEventRpc subscriptions no items! {0}", routingKey);
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
                                    Logger.LogError("ProcessEventRpcServer: eventType is null! {0}", routingKey);
                                    return null;
                                }

                                var eventReplyType = SubsManager.GetEventReplyTypeByName(routingKey);
                                if (eventReplyType is null)
                                {
                                    Logger.LogError("ProcessEventRpcServer: eventReplyType is null! {0}", routingKey);
                                    return null;
                                }

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
                            Logger.LogError(ex, "ProcessEventRpcServer: ");
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
            Logger.LogError("ProcessEventRpc HasSubscriptionsForEvent {0} No Subscriptions!", routingKey);
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
                Logger.LogError("ProcessEventRpc subscriptions no items! {0}", routingKey);
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
                                    Logger.LogError("ProcessEventRpcServer: eventType is null! {0}", routingKey);
                                    return null;
                                }

                                var eventReplyType = SubsManager.GetEventReplyTypeByName(routingKey);
                                if (eventReplyType is null)
                                {
                                    Logger.LogError("ProcessEventRpcServer: eventReplyType is null! {0}", routingKey);
                                    return null;
                                }

                                await using var ms = new MemoryStream(message.ToArray());
                                var integrationEvent = Serializer.Deserialize(eventType, ms);

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
                            Logger.LogError(ex, "ProcessEventRpcServer: ");
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
            Logger.LogError("ProcessEventRpc HasSubscriptionsForEvent {0} No Subscriptions!", routingKey);
        }
        return output;
    }//ProcessEventRpcAsync.
}