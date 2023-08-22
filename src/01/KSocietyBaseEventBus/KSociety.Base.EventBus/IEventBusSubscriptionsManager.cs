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
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void AddDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void AddSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void AddSubscription<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void AddSubscriptionQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        /// <param name="routingReplyKey"></param>
        void AddSubscriptionRpc<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingReplyKey"></param>
        void AddSubscriptionRpcClient<TR, TH>(string routingReplyKey)
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TR>;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        /// <param name="routingReplyKey"></param>
        void AddSubscriptionRpcServer<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>;

        #endregion

        #region [RemoveSubscription]

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscription<T, TH>(string routingKey)
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionQueue<T, TH>(string routingKey)
            where TH : IIntegrationQueueHandler<T>
            where T : IIntegrationEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionRpc<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcHandler<T, TR>
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionRpcClient<TR, TH>(string routingKey)
            where TH : IIntegrationRpcClientHandler<TR>
            where TR : IIntegrationEventReply;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveSubscriptionRpcServer<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcServerHandler<T, TR>
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TH"></typeparam>
        /// <param name="routingKey"></param>
        void RemoveDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEventReply<TR>() where TR : IIntegrationEventReply;

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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IIntegrationEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEventReply<TR>() where TR : IIntegrationEventReply;

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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetEventKey<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        string GetEventReplyKey<TR>();
    }
}
