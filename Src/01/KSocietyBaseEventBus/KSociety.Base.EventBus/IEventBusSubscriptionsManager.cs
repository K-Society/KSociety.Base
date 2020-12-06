using System;
using System.Collections.Generic;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using static KSociety.Base.EventBus.InMemoryEventBusSubscriptionsManager;

namespace KSociety.Base.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        bool IsReplyEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        event EventHandler<string> OnEventReplyRemoved;

        void AddDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;

        void AddSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void AddSubscription<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void AddSubscriptionQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>;

        void AddSubscriptionRpc<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>;

        void AddSubscriptionRpcClient<TR, TH>(/*string routingKey,*/ string routingReplyKey)
            //where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TR>;

        void AddSubscriptionRpcServer<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>;

        void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent;

        void RemoveSubscription<T, TH>(string routingKey)
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent;

        void RemoveSubscriptionQueue<T, TH>(string routingKey)
            where TH : IIntegrationQueueHandler<T>
            where T : IIntegrationEvent;

        void RemoveSubscriptionRpc<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcHandler<T, TR>
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply;

        void RemoveSubscriptionRpcClient<TR, TH>(string routingKey)
            where TH : IIntegrationRpcClientHandler<TR>
            where TR : IIntegrationEventReply;

        void RemoveSubscriptionRpcServer<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcServerHandler<T, TR>
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply;

        void RemoveDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;

        bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent;

        bool HasSubscriptionsForEventReply<TR>() where TR : IIntegrationEventReply;

        bool HasSubscriptionsForEvent(string eventName);

        bool HasSubscriptionsForEventReply(string eventName);

        Type GetEventTypeByName(string eventName);

        Type GetEventReplyTypeByName(string eventName);

        void Clear();

        void ClearReply();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IIntegrationEvent;

        IEnumerable<SubscriptionInfo> GetHandlersForEventReply<TR>() where TR : IIntegrationEventReply;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        IEnumerable<SubscriptionInfo> GetHandlersForEventReply(string eventName);

        string GetEventKey<T>();

        string GetEventReplyKey<TR>();
    }
}
