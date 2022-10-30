using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.EventBus;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.EventBus;
using KSociety.Base.EventBus.Abstractions.Handler;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.Extensions.Logging;
using Polly;
using ProtoBuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KSociety.Base.EventBusRabbitMQ;

public sealed class EventBusRabbitMqRpc : EventBusRabbitMq, IEventBusRpc
{
    private AsyncLazy<IModel> _consumerChannelReply;
    private string _queueNameReply;

    private readonly string _correlationId;

    #region [Constructor]

    public EventBusRabbitMqRpc(IRabbitMqPersistentConnection persistentConnection, ILoggerFactory loggerFactory,
        IIntegrationGeneralHandler eventHandler, IEventBusSubscriptionsManager subsManager,
        IEventBusParameters eventBusParameters,
        string queueName = null)
        : base(persistentConnection, loggerFactory, eventHandler, subsManager, eventBusParameters, queueName)
    {
        _correlationId = Guid.NewGuid().ToString();
    }

    #endregion

    public override void Initialize(CancellationToken cancel = default)
    {
        Logger.LogTrace("EventBusRabbitMqRpc Initialize.");
        SubsManager.OnEventReplyRemoved += SubsManager_OnEventReplyRemoved;
        ConsumerChannel = new AsyncLazy<IModel>(async () => await CreateConsumerChannelAsync(cancel));
        _queueNameReply = QueueName + "_Reply";
        _consumerChannelReply =
            new AsyncLazy<IModel>(async () => await CreateConsumerChannelReplyAsync(cancel));
    }

    public IIntegrationRpcHandler<T, TR> GetIntegrationRpcHandler<T, TR>()
        where T : IIntegrationEvent
        where TR : IIntegrationEventReply
    {
        if (EventHandler is IIntegrationRpcHandler<T, TR> queue)
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
            channel.QueueUnbind(_queueNameReply, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
        }

        if (!SubsManager.IsReplyEmpty) return;

        _queueNameReply = string.Empty;
        (await _consumerChannelReply).Close();
    }

    public override async ValueTask Publish(IIntegrationEvent @event)
    {
        if (!PersistentConnection.IsConnected)
        {
            await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .Or<Exception>()
            .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                Logger.LogWarning(ex.ToString());
            });

        using var channel = PersistentConnection.CreateModel();
        var routingKey = @event.RoutingKey;

        channel.ExchangeDeclare(EventBusParameters.ExchangeDeclareParameters.ExchangeName, 
            EventBusParameters.ExchangeDeclareParameters.ExchangeType, EventBusParameters.ExchangeDeclareParameters.ExchangeDurable, 
            EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

        await using var ms = new MemoryStream();
        Serializer.Serialize(ms, @event);
        var body = ms.ToArray();

        policy.Execute(() =>
        {
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 1; //2 = persistent, write on disk
            properties.CorrelationId = _correlationId;
            properties.ReplyTo = _queueNameReply;
            channel.BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName, routingKey, true, properties, body);
        });
    }

    protected override void QueueInitialize(IModel channel)
    {
        try
        {
            channel.ExchangeDeclare(
                EventBusParameters.ExchangeDeclareParameters.ExchangeName, EventBusParameters.ExchangeDeclareParameters.ExchangeType, 
                EventBusParameters.ExchangeDeclareParameters.ExchangeDurable, EventBusParameters.ExchangeDeclareParameters.ExchangeAutoDelete);

            channel.QueueDeclare(
                QueueName, EventBusParameters.QueueDeclareParameters.QueueDurable,
                EventBusParameters.QueueDeclareParameters.QueueExclusive, EventBusParameters.QueueDeclareParameters.QueueAutoDelete, null);
        }
        catch (RabbitMQClientException rex)
        {
            Logger.LogError(rex, "EventBusRabbitMqRpc RabbitMQClientException QueueInitialize: ");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "EventBusRabbitMqRpc QueueInitialize: ");
        }
    }

    #region [Subscribe]

    public void SubscribeRpc<T, TR, TH>(string routingKey)
        where T : IIntegrationEvent
        where TR : IIntegrationEventReply
        where TH : IIntegrationRpcHandler<T, TR>
    {
        var eventName = SubsManager.GetEventKey<T>();
        var eventNameResult = SubsManager.GetEventReplyKey<TR>();
        Logger.LogDebug("SubscribeRpc: eventName: {0}.{1} eventNameResult: {2}.{3}", eventName, routingKey, eventNameResult, routingKey);
        DoInternalSubscriptionRpc(eventName + "." + routingKey, eventNameResult + "." + routingKey);
        SubsManager.AddSubscriptionRpc<T, TR, TH>(eventName + "." + routingKey, eventNameResult + "." + routingKey);
        StartBasicConsume();
        StartBasicConsumeReply();
    }

    private async void DoInternalSubscriptionRpc(string eventName, string eventNameResult)
    {
        var containsKey = SubsManager.HasSubscriptionsForEvent(eventName);
        if (containsKey) return;
        if (!PersistentConnection.IsConnected)
        {
            await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
        }

        using var channel = PersistentConnection.CreateModel();
        QueueInitialize(channel);
        channel.QueueBind(QueueName, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventName);
        channel.QueueBind(_queueNameReply, EventBusParameters.ExchangeDeclareParameters.ExchangeName, eventNameResult);
    }

    #endregion

    #region [Unsubscribe]
        
    public void UnsubscribeRpc<T, TR, TH>(string routingKey)
        where T : IIntegrationEvent
        where TH : IIntegrationRpcHandler<T, TR>
        where TR : IIntegrationEventReply
    {
        SubsManager.RemoveSubscriptionRpc<T, TR, TH>(routingKey);
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
        Logger.LogTrace("Starting RabbitMQ basic consume");
        try
        {


            if (ConsumerChannel is null)
            {
                Logger.LogWarning("ConsumerChannel is null");
                return false;
            }

            if (ConsumerChannel?.Value is not null)
            {
                var consumer = new AsyncEventingBasicConsumer(await ConsumerChannel);

                consumer.Received += ConsumerReceivedAsync;

                (await ConsumerChannel).BasicConsume(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer);

                return true;
            }
            else
            {
                Logger.LogError("StartBasicConsume can't call on ConsumerChannel == null");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "StartBasicConsume: ");
        }
        return false;
    }

    private async ValueTask StartBasicConsumeReply()
    {
        Logger.LogTrace("Starting RabbitMQ basic consume reply");
        try
        {


            if (_consumerChannelReply is null)
            {
                Logger.LogWarning("ConsumerChannelReply is null");
                return;
            }

            if (_consumerChannelReply?.Value != null)
            {
                var consumer = new AsyncEventingBasicConsumer(await _consumerChannelReply);

                consumer.Received += ConsumerReceivedReply;

                (await _consumerChannelReply).BasicConsume(
                    queue: _queueNameReply,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                Logger.LogError("StartBasicConsumeReply can't call on _consumerChannelReply is null");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "StartBasicConsumeReply: ");
        }
    }

    protected override async Task ConsumerReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        string[] result = eventArgs.RoutingKey.Split('.');
        var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

        try
        {
            try
            {
                var props = eventArgs.BasicProperties;
                var replyProps = (await ConsumerChannel).CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var response = await ProcessEventRpc(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
                var ms = new MemoryStream();
                Serializer.Serialize<IIntegrationEventReply>(ms, response);
                var body = ms.ToArray();
                (await ConsumerChannel).BasicPublish(EventBusParameters.ExchangeDeclareParameters.ExchangeName, (string)response.RoutingKey, replyProps, body);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CreateConsumerChannel RPC Received: ");
            }

        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "ConsumerReceived: {0}", eventName);
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

    private async Task ConsumerReceivedReply(object sender, BasicDeliverEventArgs eventArgs)
    {
        string[] result = eventArgs.RoutingKey.Split('.');
        var eventName = result.Length > 1 ? result[0] : eventArgs.RoutingKey;

        try
        {
            if (!eventArgs.BasicProperties.CorrelationId.Equals(_correlationId)) return;
            await ProcessEventReply(eventArgs.RoutingKey, eventName, eventArgs.Body).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "ConsumerReceivedReply: {0}", eventName);
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
        if (!PersistentConnection.IsConnected)
        {
            await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
        }

        var channel = PersistentConnection.CreateModel();

        QueueInitialize(channel);
        channel.BasicQos(0, 1, false);

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
    //        StartBasicConsumeReply();
    //    };

    //    return channel;
    //}

    private async ValueTask<IModel> CreateConsumerChannelReplyAsync(CancellationToken cancel = default)
    {
        if (!PersistentConnection.IsConnected)
        {
            await PersistentConnection.TryConnectAsync().ConfigureAwait(false);
        }

        var channel = PersistentConnection.CreateModel();

        QueueInitialize(channel);
        channel.BasicQos(0, 1, false);

        channel.CallbackException += async (sender, ea) =>
        {
            Logger.LogError(ea.Exception, "CallbackException Rpc: ");
            _consumerChannelReply?.Value.Dispose();
            _consumerChannelReply = new AsyncLazy<IModel>(async () => await CreateConsumerChannelReplyAsync(cancel));
            await StartBasicConsumeReply();
        };

        return channel;
    }

    private async ValueTask ProcessEventReply(string routingKey, string eventName, ReadOnlyMemory<byte> message,
        CancellationToken cancel = default)
    {
        if (SubsManager.HasSubscriptionsForEventReply(routingKey))
        {
            var subscriptions = SubsManager.GetHandlersForEventReply(routingKey);
            if (!subscriptions.Any())
            {
                Logger.LogError("ProcessEventReply subscriptions no items! " + routingKey);
            }
            foreach (var subscription in subscriptions)
            {
                switch (subscription.SubscriptionManagerType)
                {
                    case SubscriptionManagerType.Rpc:
                        try
                        {
                            if (EventHandler is null)
                            {
                                Logger.LogError("ProcessEventReply _eventHandler is null!");
                            }
                            else
                            {

                                var eventType = SubsManager.GetEventTypeByName(routingKey);
                                if (eventType is null)
                                {
                                    Logger.LogError("ProcessEventReply: eventType is null! " + routingKey);
                                    return;
                                }

                                var eventResultType = SubsManager.GetEventReplyTypeByName(routingKey);
                                if (eventResultType is null)
                                {
                                    Logger.LogError("ProcessEventReply: eventResultType is null! " + routingKey);
                                    return;
                                }

                                await using var ms = new MemoryStream(message.ToArray());
                                var integrationEvent = Serializer.Deserialize(eventResultType, ms);
                                var concreteType =
                                    typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType,
                                        eventResultType);
                                await ((ValueTask) concreteType.GetMethod("HandleReply")
                                    .Invoke(EventHandler, new[] {integrationEvent, cancel})).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "ProcessEventReply: ");
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        else
        {
            Logger.LogError("ProcessEventReply HasSubscriptionsForEventReply {0} No Subscriptions!", routingKey);
        }
    }

    private async ValueTask<dynamic> ProcessEventRpc(string routingKey, string eventName, ReadOnlyMemory<byte> message, CancellationToken cancel = default)
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
                        try
                        {
                            if (EventHandler is null)
                            {
                                Logger.LogError("ProcessEventRpc _eventHandler is null!");
                            }
                            else
                            {
                                var eventType = SubsManager.GetEventTypeByName(routingKey);
                                if (eventType is null)
                                {
                                    Logger.LogError("ProcessEventRpc: eventType is null! {0}", routingKey);
                                    return null;
                                }
                                var eventReplyType = SubsManager.GetEventReplyTypeByName(routingKey);
                                if (eventReplyType is null)
                                {
                                    Logger.LogError("ProcessEventRpc: eventReplyType is null! {0}", routingKey);
                                    return null;
                                }

                                using (var ms = new MemoryStream(message.ToArray()))
                                {
                                    var integrationEvent = Serializer.Deserialize(eventType, ms);
                                    var concreteType =
                                        typeof(IIntegrationRpcHandler<,>).MakeGenericType(eventType,
                                            eventReplyType);

                                    output = await ((dynamic) concreteType.GetMethod("HandleRpcAsync")
                                        .Invoke(EventHandler, new[] {integrationEvent, cancel})).ConfigureAwait(false);

                                    if (output is null)
                                    {
                                        Logger.LogError("ProcessEventRpc output is null!");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "ProcessEventRpc: ");
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
}