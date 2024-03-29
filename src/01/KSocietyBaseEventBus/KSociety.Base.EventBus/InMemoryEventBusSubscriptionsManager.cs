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
        public event EventHandler<string> OnEventRemoved;

        ///<inheritdoc/>
        public event EventHandler<string> OnEventReplyRemoved;

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
        public void AddDynamicSubscription<TDynamicIntegrationEventHandler>(string routingKey)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            this.DoAddSubscription(typeof(TDynamicIntegrationEventHandler), routingKey, SubscriptionManagerType.Dynamic);
        }

        ///<inheritdoc/>
        public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var routingKey = this.GetEventKey<TIntegrationEvent>();
            this.DoAddSubscription(typeof(TIntegrationEventHandler), routingKey, SubscriptionManagerType.Typed);
            //this._eventTypes.TryAdd(routingKey, typeof(T));
            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(TIntegrationEvent));
            }
        }

        ///<inheritdoc/>
        public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            this.DoAddSubscription(typeof(TIntegrationEventHandler), routingKey, SubscriptionManagerType.Typed);
            //this._eventTypes.TryAdd(routingKey, typeof(T));
            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(TIntegrationEvent));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>
        {
            this.DoAddSubscription(typeof(TIntegrationQueueHandler), routingKey, SubscriptionManagerType.Queue);
            //this._eventTypes.TryAdd(routingKey, typeof(T));
            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(TIntegrationEvent));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey, string routingReplyKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        {
            this.DoAddSubscriptionReply(typeof(TIntegrationRpcHandler), routingKey, SubscriptionManagerType.Rpc, routingReplyKey);

            //this._eventTypes.TryAdd(routingKey, typeof(T));

            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(TIntegrationEvent));
            }

            //this._eventTypes.TryAdd(routingReplyKey, typeof(T));

            if (!this._eventTypes.ContainsKey(routingReplyKey))
            {
                this._eventTypes.Add(routingReplyKey, typeof(TIntegrationEvent));
            }

            //this._eventReplyTypes.TryAdd(routingReplyKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingReplyKey))
            {
                this._eventReplyTypes.Add(routingReplyKey, typeof(TIntegrationEventReply));
            }

            //this._eventReplyTypes.TryAdd(routingKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingKey))
            {
                this._eventReplyTypes.Add(routingKey, typeof(TIntegrationEventReply));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(string routingReplyKey)
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            this.DoAddSubscription(typeof(TIntegrationRpcClientHandler), routingReplyKey, SubscriptionManagerType.RpcClient);

            //this._eventReplyTypes.TryAdd(routingReplyKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingReplyKey))
            {
                this._eventReplyTypes.Add(routingReplyKey, typeof(TIntegrationEventReply));
            }
        }

        ///<inheritdoc/>
        public void AddSubscriptionRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey, string routingReplyKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
        {
            this.DoAddSubscriptionReply(typeof(TIntegrationRpcServerHandler), routingKey, SubscriptionManagerType.RpcServer, routingReplyKey);

            //this._eventTypes.TryAdd(routingKey, typeof(T));

            if (!this._eventTypes.ContainsKey(routingKey))
            {
                this._eventTypes.Add(routingKey, typeof(TIntegrationEventRpc));
            }

            //this._eventReplyTypes.TryAdd(routingKey, typeof(TR));

            if (!this._eventReplyTypes.ContainsKey(routingKey))
            {
                this._eventReplyTypes.Add(routingKey, typeof(TIntegrationEventReply));
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
        public void RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(string routingKey)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = this.FindDynamicSubscriptionToRemove<TDynamicIntegrationEventHandler>(routingKey);
            this.DoRemoveHandler(routingKey, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var handlerToRemove = this.FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>();
            var eventName = this.GetEventKey<TIntegrationEvent>();
            this.DoRemoveHandler(eventName, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = this.GetEventKey<TIntegrationEvent>();
            var handlerToRemove = this.FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>(eventName + "." + routingKey);

            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionQueue<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>
        {
            var eventName = this.GetEventKey<TIntegrationEvent>();
            var handlerToRemove = this.FindSubscriptionQueueToRemove<TIntegrationEvent, TIntegrationQueueHandler>(eventName + "." + routingKey);
            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionRpc<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        {
            var eventName = this.GetEventKey<TIntegrationEvent>();
            var eventNameResult = this.GetEventReplyKey<TIntegrationEventReply>();
            var handlerToRemove = this.FindSubscriptionRpcToRemove<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(eventName + "." + routingKey);
            var handlerToRemoveReply = this.FindSubscriptionRpcToRemoveReply<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(eventNameResult + "." + routingKey);
            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
            this.DoRemoveHandlerReply(eventNameResult + "." + routingKey, handlerToRemoveReply);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionRpcClient<TIntegrationEventReply, TIntegrationRpcClientHandler>(string routingKey)
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            var eventNameResult = this.GetEventReplyKey<TIntegrationEventReply>();
            var handlerToRemoveReply = this.FindSubscriptionRpcClientToRemoveReply<TIntegrationEventReply, TIntegrationRpcClientHandler>(eventNameResult + "." + routingKey);
            this.DoRemoveHandlerReply(eventNameResult + "." + routingKey, handlerToRemoveReply);
        }

        ///<inheritdoc/>
        public void RemoveSubscriptionRpcServer<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
        {
            var eventName = this.GetEventKey<TIntegrationEventRpc>();
            var handlerToRemove = this.FindSubscriptionRpcServerToRemove<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(eventName + "." + routingKey);
            this.DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo subsToRemove)
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

        private void DoRemoveHandlerReply(string eventReplyName, SubscriptionInfo subsToRemove)
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
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IIntegrationEvent, new()
        {
            var key = this.GetEventKey<TIntegrationEvent>();
            return this.GetHandlersForEvent(key);
        }

        ///<inheritdoc/>
        public IEnumerable<SubscriptionInfo> GetHandlersForEventReply<TIntegrationEventReply>() where TIntegrationEventReply : IIntegrationEventReply, new()
        {
            var key = this.GetEventReplyKey<TIntegrationEventReply>();
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

        private SubscriptionInfo FindDynamicSubscriptionToRemove<TDynamicIntegrationEventHandler>(string eventName)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            return this.DoFindSubscriptionToRemove(eventName, typeof(TDynamicIntegrationEventHandler));
        }

        private SubscriptionInfo FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = this.GetEventKey<TIntegrationEvent>();
            return this.DoFindSubscriptionToRemove(eventName, typeof(TIntegrationEventHandler));
        }

        private SubscriptionInfo FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TIntegrationEventHandler));
        }

        private SubscriptionInfo FindSubscriptionQueueToRemove<TIntegrationEvent, TIntegrationQueueHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationQueueHandler : IIntegrationQueueHandler<TIntegrationEvent>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TIntegrationQueueHandler));
        }

        private SubscriptionInfo FindSubscriptionRpcToRemove<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TIntegrationRpcHandler));
        }

        private SubscriptionInfo FindSubscriptionRpcToRemoveReply<TIntegrationEvent, TIntegrationEventReply, TIntegrationRpcHandler>(string routingKey)
            where TIntegrationEvent : IIntegrationEvent, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcHandler : IIntegrationRpcHandler<TIntegrationEvent, TIntegrationEventReply>
        {
            return this.DoFindSubscriptionToRemoveReply(routingKey, typeof(TIntegrationRpcHandler));
        }

        private SubscriptionInfo FindSubscriptionRpcClientToRemoveReply<TIntegrationEventReply, TIntegrationRpcClientHandler>(string routingKey)
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcClientHandler : IIntegrationRpcClientHandler<TIntegrationEventReply>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TIntegrationRpcClientHandler));
        }

        private SubscriptionInfo FindSubscriptionRpcServerToRemove<TIntegrationEventRpc, TIntegrationEventReply, TIntegrationRpcServerHandler>(string routingKey)
            where TIntegrationEventRpc : IIntegrationEventRpc, new()
            where TIntegrationEventReply : IIntegrationEventReply, new()
            where TIntegrationRpcServerHandler : IIntegrationRpcServerHandler<TIntegrationEventRpc, TIntegrationEventReply>
        {
            return this.DoFindSubscriptionToRemove(routingKey, typeof(TIntegrationRpcServerHandler));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            return !this.HasSubscriptionsForEvent(eventName)
                ? null
                : this._handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        private SubscriptionInfo DoFindSubscriptionToRemoveReply(string eventReplyName, Type handlerType)
        {
            return !this.HasSubscriptionsForEventReply(eventReplyName)
                ? null
                : this._replyHandlers[eventReplyName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        ///<inheritdoc/>
        public bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IIntegrationEvent, new()
        {
            var key = this.GetEventKey<TIntegrationEvent>();
            return this.HasSubscriptionsForEvent(key);
        }

        ///<inheritdoc/>
        public bool HasSubscriptionsForEventReply<TIntegrationEventReply>() where TIntegrationEventReply : IIntegrationEventReply, new()
        {

            var key = this.GetEventReplyKey<TIntegrationEventReply>();
            return this.HasSubscriptionsForEventReply(key);
        }

        ///<inheritdoc/>
        public bool HasSubscriptionsForEvent(string eventName)
        {
            if (!String.IsNullOrEmpty(eventName))
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
        public Type GetEventTypeByName(string eventName)
        {
            return this._eventTypes.TryGetValue(eventName, out var type) ? type : null;
        }

        ///<inheritdoc/>
        public Type GetEventReplyTypeByName(string eventResultName)
        {
            return this._eventReplyTypes.TryGetValue(eventResultName, out var type) ? type : null;
        }

        ///<inheritdoc/>
        public string GetEventKey<TIntegrationEvent>()
        {
            return typeof(TIntegrationEvent).Name;
        }

        ///<inheritdoc/>
        public string GetEventReplyKey<TIntegrationEventReply>()
        {
            return typeof(TIntegrationEventReply).Name;
        }
    }
}
