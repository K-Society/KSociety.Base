// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Abstractions.Handler;

    ///<inheritdoc/>
    public partial class InMemoryEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly Dictionary<string, List<SubscriptionInfo>> _replyHandlers;

        private readonly Dictionary<string, Type> _eventTypes;
        private readonly Dictionary<string, Type> _eventReplyTypes;

        ///<inheritdoc/>
        public event EventHandler<string>? OnEventRemoved;

        ///<inheritdoc/>
        public event EventHandler<string>? OnEventReplyRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            this._handlers = new Dictionary<string, List<SubscriptionInfo>>();
            this._replyHandlers = new Dictionary<string, List<SubscriptionInfo>>();
            this._eventTypes = new Dictionary<string, Type>();
            this._eventReplyTypes = new Dictionary<string, Type>();
        }

        ///<inheritdoc/>
        public bool IsEmpty => !this._handlers.Keys.Any();

        ///<inheritdoc/>
        public bool IsReplyEmpty => !this._replyHandlers.Keys.Any();

        ///<inheritdoc/>
        public void Clear()
        {
            this._handlers.Clear();
        }

        ///<inheritdoc/>
        public void ClearReply()
        {
            this._replyHandlers.Clear();
        }

        #region [AddSubscription]

        ///<inheritdoc/>
        public void AddDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler
        {
            this.DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Dynamic);
        }

        ///<inheritdoc/>
        public void AddSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var routingKey = this.GetEventKey<T>();
            this.DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Typed);
            //this._eventTypes.TryAdd(routingKey, typeof(T));
            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(T));
            }
        }

        ///<inheritdoc/>
        public void AddSubscription<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            this.DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Typed);
            //this._eventTypes.TryAdd(routingKey, typeof(T));
            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(T));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            this.DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Queue);
            //this._eventTypes.TryAdd(routingKey, typeof(T));
            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(T));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionRpc<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>
        {
            this.DoAddSubscriptionReply(typeof(TH), routingKey, SubscriptionManagerType.Rpc, routingReplyKey);

            //this._eventTypes.TryAdd(routingKey, typeof(T));

            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(T));
            }

            //this._eventTypes.TryAdd(routingReplyKey, typeof(T));

            if (!this._eventTypes.ContainsKey(routingReplyKey))
            {
                this._eventTypes.Add(routingReplyKey, typeof(T));
            }

            //this._eventReplyTypes.TryAdd(routingReplyKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingReplyKey))
            {
                this._eventReplyTypes.Add(routingReplyKey, typeof(TR));
            }

            //this._eventReplyTypes.TryAdd(routingKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingKey))
            {
                this._eventReplyTypes.Add(routingKey, typeof(TR));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionRpcClient<TR, TH>(string routingReplyKey)
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TR>
        {
            this.DoAddSubscription(typeof(TH), routingReplyKey, SubscriptionManagerType.RpcClient);

            //this._eventReplyTypes.TryAdd(routingReplyKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingReplyKey))
            {
                this._eventReplyTypes.Add(routingReplyKey, typeof(TR));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionRpcServer<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>
        {
            this.DoAddSubscriptionReply(typeof(TH), routingKey, SubscriptionManagerType.RpcServer, routingReplyKey);

            //this._eventTypes.TryAdd(routingKey, typeof(T));

            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(T));
            }

            //this._eventReplyTypes.TryAdd(routingKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingKey))
            {
                this._eventReplyTypes.Add(routingKey, typeof(TR));
            }
        }

        private void DoAddSubscription(Type handlerType, string eventName,
            SubscriptionManagerType subscriptionManagerType)
        {


            if (subscriptionManagerType.Equals(SubscriptionManagerType.RpcClient))
            {
                if (!this.HasSubscriptionsForEventReply(eventName))
                {
                    this._replyHandlers.Add(eventName, new List<SubscriptionInfo>());
                }

                if (this._replyHandlers[eventName].Any(s => s.HandlerType == handlerType))
                {
                    throw new ArgumentException(
                        $"ReplyHandler Type {handlerType.Name} already registered for '{eventName}'",
                        nameof(handlerType));
                }
            }
            else
            {
                if (!this.HasSubscriptionsForEvent(eventName))
                {
                    this._handlers.Add(eventName, new List<SubscriptionInfo>());
                }

                if (this._handlers[eventName].Any(s => s.HandlerType == handlerType))
                {
                    throw new ArgumentException(
                        $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
                }
            }

            switch (subscriptionManagerType)
            {
                case SubscriptionManagerType.Dynamic:
                    this._handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
                    break;

                case SubscriptionManagerType.Typed:
                    this._handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
                    break;

                case SubscriptionManagerType.Queue:
                    this._handlers[eventName].Add(SubscriptionInfo.Queue(handlerType));
                    break;

                case SubscriptionManagerType.Rpc:
                    break;
                case SubscriptionManagerType.RpcClient:
                    this._replyHandlers[eventName].Add(SubscriptionInfo.RpcClient(handlerType));
                    break;
                case SubscriptionManagerType.RpcServer:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionManagerType), subscriptionManagerType,
                        null);
            }
        }

        private void DoAddSubscriptionReply(Type handlerType, string eventName,
            SubscriptionManagerType subscriptionManagerType, string eventReplyName)
        {
            if (!this.HasSubscriptionsForEvent(eventName))
            {
                this._handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (this._handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            if (!String.IsNullOrEmpty(eventReplyName))
            {
                if (!this.HasSubscriptionsForEventReply(eventReplyName))
                {
                    this._replyHandlers.Add(eventReplyName, new List<SubscriptionInfo>());
                }

                if (this._replyHandlers[eventReplyName].Any(s => s.HandlerType == handlerType))
                {
                    throw new ArgumentException(
                        $"ReplyHandler Type {handlerType.Name} already registered for '{eventReplyName}'",
                        nameof(handlerType));
                }
            }

            switch (subscriptionManagerType)
            {
                case SubscriptionManagerType.Dynamic:
                    break;

                case SubscriptionManagerType.Typed:
                    break;

                case SubscriptionManagerType.Queue:
                    break;

                case SubscriptionManagerType.Rpc:
                    this._handlers[eventName].Add(SubscriptionInfo.Rpc(handlerType));
                    if (!String.IsNullOrEmpty(eventReplyName))
                    {
                        this._replyHandlers[eventReplyName].Add(SubscriptionInfo.Rpc(handlerType));
                    }

                    break;
                case SubscriptionManagerType.RpcClient:
                    break;
                case SubscriptionManagerType.RpcServer:
                    this._handlers[eventName].Add(SubscriptionInfo.RpcServer(handlerType));
                    if (!String.IsNullOrEmpty(eventReplyName))
                    {
                        this._replyHandlers[eventReplyName].Add(SubscriptionInfo.RpcServer(handlerType));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionManagerType), subscriptionManagerType,
                        null);
            }
        }

        #endregion

        #region [RemoveSubscription]

        ///<inheritdoc/>
        public void RemoveDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = this.FindDynamicSubscriptionToRemove<TH>(routingKey);
            this.DoRemoveHandler(routingKey, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent
        {
            var handlerToRemove = this.FindSubscriptionToRemove<T, TH>();
            var eventName = this.GetEventKey<T>();
            this.DoRemoveHandler(eventName, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscription<T, TH>(string routingKey)
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent
        {
            var eventName = this.GetEventKey<T>();
            var handlerToRemove = this.FindSubscriptionToRemove<T, TH>(eventName + "." + routingKey);

            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionQueue<T, TH>(string routingKey)
            where TH : IIntegrationQueueHandler<T>
            where T : IIntegrationEvent
        {
            var eventName = this.GetEventKey<T>();
            var handlerToRemove = this.FindSubscriptionQueueToRemove<T, TH>(eventName + "." + routingKey);
            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionRpc<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcHandler<T, TR>
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
        {
            var eventName = this.GetEventKey<T>();
            var eventNameResult = this.GetEventReplyKey<TR>();
            var handlerToRemove = this.FindSubscriptionRpcToRemove<T, TR, TH>(eventName + "." + routingKey);
            var handlerToRemoveReply = this.FindSubscriptionRpcToRemoveReply<T, TR, TH>(eventNameResult + "." + routingKey);
            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
            this.DoRemoveHandlerReply(eventNameResult + "." + routingKey, handlerToRemoveReply);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionRpcClient<TR, TH>(string routingKey)
            where TH : IIntegrationRpcClientHandler<TR>
            where TR : IIntegrationEventReply
        {
            var eventNameResult = this.GetEventReplyKey<TR>();
            var handlerToRemoveReply = this.FindSubscriptionRpcClientToRemoveReply<TR, TH>(eventNameResult + "." + routingKey);
            this.DoRemoveHandlerReply(eventNameResult + "." + routingKey, handlerToRemoveReply);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionRpcServer<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcServerHandler<T, TR>
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
        {
            var eventName = this.GetEventKey<T>();
            var handlerToRemove = this.FindSubscriptionRpcServerToRemove<T, TR, TH>(eventName + "." + routingKey);
            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo? subsToRemove)
        {
            if (subsToRemove != null)
            {
                this._handlers[eventName].Remove(subsToRemove);
                if (!this._handlers[eventName].Any())
                {
                    this._handlers.Remove(eventName);

                    if (this._eventTypes.ContainsKey(eventName))
                    {
                        this._eventTypes.Remove(eventName);
                    }

                    this.RaiseOnEventRemoved(eventName);
                }
            }
        }

        private void DoRemoveHandlerReply(string eventReplyName, SubscriptionInfo? subsToRemove)
        {
            if (subsToRemove != null)
            {
                this._replyHandlers[eventReplyName].Remove(subsToRemove);
                if (!this._replyHandlers[eventReplyName].Any())
                {
                    this._replyHandlers.Remove(eventReplyName);

                    if (this._eventReplyTypes.ContainsKey(eventReplyName))
                    {
                        this._eventReplyTypes.Remove(eventReplyName);
                    }

                    this.RaiseOnEventReplyRemoved(eventReplyName);
                }
            }
        }

        #endregion

        ///<inheritdoc/>
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IIntegrationEvent
        {
            var key = this.GetEventKey<T>();
            return this.GetHandlersForEvent(key);
        }

        ///<inheritdoc/>
        public IEnumerable<SubscriptionInfo> GetHandlersForEventReply<TR>() where TR : IIntegrationEventReply
        {
            var key = this.GetEventReplyKey<TR>();
            return this.GetHandlersForEventReply(key);
        }

        ///<inheritdoc/>
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            return this._handlers[eventName];
        }

        ///<inheritdoc/>
        public IEnumerable<SubscriptionInfo> GetHandlersForEventReply(string eventName)
        {
            return this._replyHandlers[eventName];
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            this.OnEventRemoved?.Invoke(this, eventName);
        }

        private void RaiseOnEventReplyRemoved(string eventName)
        {
            this.OnEventReplyRemoved?.Invoke(this, eventName);
        }

        private SubscriptionInfo? FindDynamicSubscriptionToRemove<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            return this.DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private SubscriptionInfo? FindSubscriptionToRemove<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = this.GetEventKey<T>();
            return this.DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private SubscriptionInfo? FindSubscriptionToRemove<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private SubscriptionInfo? FindSubscriptionQueueToRemove<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private SubscriptionInfo? FindSubscriptionRpcToRemove<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private SubscriptionInfo? FindSubscriptionRpcToRemoveReply<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>
        {
            return this.DoFindSubscriptionToRemoveReply(routingKey, typeof(TH));
        }

        private SubscriptionInfo? FindSubscriptionRpcClientToRemoveReply<TR, TH>(string routingKey)
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TR>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private SubscriptionInfo? FindSubscriptionRpcServerToRemove<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private SubscriptionInfo? DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            return !this.HasSubscriptionsForEvent(eventName)
                ? null
                : this._handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        private SubscriptionInfo? DoFindSubscriptionToRemoveReply(string eventReplyName, Type handlerType)
        {
            return !this.HasSubscriptionsForEventReply(eventReplyName)
                ? null
                : this._replyHandlers[eventReplyName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        ///<inheritdoc/>
        public bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent
        {
            var key = this.GetEventKey<T>();
            return this.HasSubscriptionsForEvent(key);
        }

        ///<inheritdoc/>
        public bool HasSubscriptionsForEventReply<TR>() where TR : IIntegrationEventReply
        {

            var key = this.GetEventReplyKey<TR>();
            return this.HasSubscriptionsForEventReply(key);
        }

        ///<inheritdoc/>
        public bool HasSubscriptionsForEvent(string? eventName)
        {
            if (eventName != null)
            {
                return this._handlers.ContainsKey(eventName);
            }

            return false;
        }

        ///<inheritdoc/>
        public bool HasSubscriptionsForEventReply(string eventName)
        {
            return this._replyHandlers.ContainsKey(eventName);
        }

        ///<inheritdoc/>
        public Type? GetEventTypeByName(string eventName)
        {
            return this._eventTypes.TryGetValue(eventName, out var type) ? type : null;
        }

        ///<inheritdoc/>
        public Type? GetEventReplyTypeByName(string eventResultName)
        {
            return this._eventReplyTypes.TryGetValue(eventResultName, out var type) ? type : null;
        }

        ///<inheritdoc/>
        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        ///<inheritdoc/>
        public string GetEventReplyKey<TR>()
        {
            return typeof(TR).Name;
        }
    }
}
