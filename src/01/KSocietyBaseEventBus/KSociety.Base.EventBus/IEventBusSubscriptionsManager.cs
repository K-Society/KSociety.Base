// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Abstractions.Handler;
    using static InMemoryEventBusSubscriptionsManager;

    /// <summary>
    /// The EventBusSubscriptionsManager, used for adding or removing an event subscription. 
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsReplyEmpty { get; }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<string> OnEventRemoved;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<string> OnEventReplyRemoved;

        #region [AddSubscription]

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDynamicIntegrationEventHandler"></typeparam>
        /// <param name="routingKey"></param>
        void AddDynamicSubscription<TDynamicIntegrationEventHandler>(string routingKey)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationEventHandler"></typeparam>
        void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationEventHandler"></typeparam>
        /// <param name="routingKey"></param>
        void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationQueueHandler"></typeparam>
        /// <param name="routingKey"></param>
        void AddSubscriptionQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <typeparam name="TIntegrationRpcHandler"></typeparam>
        /// <param name="routingKey"></param>
        /// <param name="routingReplyKey"></param>
        void AddSubscriptionRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey, string routingReplyKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <typeparam name="TIntegrationRpcClientHandler"></typeparam>
        /// <param name="routingReplyKey"></param>
        void AddSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(string routingReplyKey)
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEventRpc"></typeparam>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <typeparam name="TIntegrationRpcServerHandler"></typeparam>
        /// <param name="routingKey"></param>
        /// <param name="routingReplyKey"></param>
        void AddSubscriptionRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey, string routingReplyKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>;

        #endregion

        #region [RemoveSubscription]

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationEventHandler"></typeparam>
        void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationEventHandler"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationQueueHandler"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <typeparam name="TIntegrationRpcHandler"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <typeparam name="TIntegrationRpcClientHandler"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(string routingKey)
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEventRpc"></typeparam>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <typeparam name="TIntegrationRpcServerHandler"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDynamicIntegrationEventHandler"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(string routingKey)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IIntegrationEvent, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEventReply<TIntegrationEventReply>() where TIntegrationEventReply : IIntegrationEventReply, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEventReply(string eventName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Type GetEventReplyTypeByName(string eventName);

        /// <summary>
        /// 
        /// </summary>
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        void ClearReply();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IIntegrationEvent, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEventReply<TIntegrationEventReply>() where TIntegrationEventReply : IIntegrationEventReply, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEventReply(string eventName);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEvent"></typeparam>
        /// <returns></returns>
        string GetEventKey<TIntegrationEvent>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIntegrationEventReply"></typeparam>
        /// <returns></returns>
        string GetEventReplyKey<TIntegrationEventReply>();
    }
}
