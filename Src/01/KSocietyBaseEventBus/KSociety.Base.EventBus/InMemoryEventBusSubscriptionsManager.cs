using System;
using System.Collections.Generic;
using System.Linq;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;

namespace KSociety.Base.EventBus
{
    public partial class InMemoryEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>> _handlers;
        private readonly Dictionary<string, List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>> _replyHandlers;

        private readonly Dictionary<string, Type> _eventTypes;
        private readonly Dictionary<string, Type> _eventReplyTypes;

        public event EventHandler<string> OnEventRemoved;
        public event EventHandler<string> OnEventReplyRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>>();
            _replyHandlers = new Dictionary<string, List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>>();
            _eventTypes = new Dictionary<string, Type>();
            _eventReplyTypes = new Dictionary<string, Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public bool IsReplyEmpty => !_replyHandlers.Keys.Any();
        public void Clear() => _handlers.Clear();
        public void ClearReply() => _replyHandlers.Clear();

        #region [AddSubscription]

        public void AddDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler
        {
            DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Dynamic);
        }

        public void AddSubscription<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var routingKey = GetEventKey<T>();
            DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Typed);
            if (!_eventTypes.ContainsKey(routingKey))
            {
                _eventTypes.Add(routingKey, typeof(T));
            }
        }

        public void AddSubscription<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Typed);
            if (!_eventTypes.ContainsKey(routingKey))
            {
                _eventTypes.Add(routingKey, typeof(T));
            }
        }

        //Queue
        public void AddSubscriptionQueue<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            DoAddSubscription(typeof(TH), routingKey, SubscriptionManagerType.Queue);
            if (!_eventTypes.ContainsKey(routingKey))
            {
                _eventTypes.Add(routingKey, typeof(T));
            }
        }

        //Rpc
        public void AddSubscriptionRpc<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>
        {
            DoAddSubscriptionReply(typeof(TH), routingKey, SubscriptionManagerType.Rpc, routingReplyKey);

            if (!_eventTypes.ContainsKey(routingKey))
            {
                _eventTypes.Add(routingKey, typeof(T));
            }

            if (!_eventTypes.ContainsKey(routingReplyKey))
            {
                _eventTypes.Add(routingReplyKey, typeof(T));
            }

            if (!_eventReplyTypes.ContainsKey(routingReplyKey))
            {
                _eventReplyTypes.Add(routingReplyKey, typeof(TR));
            }

            if (!_eventReplyTypes.ContainsKey(routingKey))
            {
                _eventReplyTypes.Add(routingKey, typeof(TR));
            }
        }

        //RpcClient
        public void AddSubscriptionRpcClient<TR, TH>(/*string routingKey,*/ string routingReplyKey)
            //where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TR>
        {
            DoAddSubscription(typeof(TH), routingReplyKey, SubscriptionManagerType.RpcClient);

            //if (!_eventReplyTypes.ContainsKey(routingReplyKey))
            //{
            //    _eventReplyTypes.Add(routingReplyKey, typeof(TR));
            //}

            //if (!_eventTypes.ContainsKey(routingKey))
            //{
            //    _eventTypes.Add(routingKey, typeof(T));
            //}

            //if (!_eventTypes.ContainsKey(routingReplyKey))
            //{
            //    _eventTypes.Add(routingReplyKey, typeof(T));
            //}

            if (!_eventReplyTypes.ContainsKey(routingReplyKey))
            {
                _eventReplyTypes.Add(routingReplyKey, typeof(TR));
            }

            //if (!_eventReplyTypes.ContainsKey(routingKey))
            //{
            //    _eventReplyTypes.Add(routingKey, typeof(TR));
            //}
        }

        //RpcServer
        public void AddSubscriptionRpcServer<T, TR, TH>(string routingKey, string routingReplyKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>
        {
            DoAddSubscriptionReply(typeof(TH), routingKey, SubscriptionManagerType.RpcServer, routingReplyKey);

            if (!_eventTypes.ContainsKey(routingKey))
            {
                _eventTypes.Add(routingKey, typeof(T));
            }

            //if (!_eventTypes.ContainsKey(routingReplyKey))
            //{
            //    _eventTypes.Add(routingReplyKey, typeof(T));
            //}

            //if (!_eventReplyTypes.ContainsKey(routingReplyKey))
            //{
            //    _eventReplyTypes.Add(routingReplyKey, typeof(TR));
            //}

            if (!_eventReplyTypes.ContainsKey(routingKey))
            {
                _eventReplyTypes.Add(routingKey, typeof(TR));
            }
        }

        private void DoAddSubscription(Type handlerType, string eventName,
            SubscriptionManagerType subscriptionManagerType)
        {
            

            if (subscriptionManagerType.Equals(SubscriptionManagerType.RpcClient))
            {
                if (!HasSubscriptionsForEventReply(eventName))
                {
                    _replyHandlers.Add(eventName, new List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>());
                }

                if (_replyHandlers[eventName].Any(s => s.HandlerType == handlerType))
                {
                    throw new ArgumentException(
                        $"ReplyHandler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
                }
            }
            else
            {
                if (!HasSubscriptionsForEvent(eventName))
                {
                    _handlers.Add(eventName, new List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>());
                }

                if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
                {
                    throw new ArgumentException(
                        $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
                }
            }

            switch (subscriptionManagerType)
            {
                case SubscriptionManagerType.Dynamic:
                    _handlers[eventName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.Dynamic(handlerType));
                    break;

                case SubscriptionManagerType.Typed:
                    _handlers[eventName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.Typed(handlerType));
                    break;

                case SubscriptionManagerType.Queue:
                    _handlers[eventName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.Queue(handlerType));
                    break;

                case SubscriptionManagerType.Rpc:
                    break;
                case SubscriptionManagerType.RpcClient:
                    _replyHandlers[eventName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.RpcClient(handlerType));
                    break;
                case SubscriptionManagerType.RpcServer:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionManagerType), subscriptionManagerType, null);
            }
        }

        private void DoAddSubscriptionReply(Type handlerType, string eventName,
            SubscriptionManagerType subscriptionManagerType, string eventReplyName)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            if (!string.IsNullOrEmpty(eventReplyName))
            {
                if (!HasSubscriptionsForEventReply(eventReplyName))
                {
                    _replyHandlers.Add(eventReplyName, new List<InMemoryEventBusSubscriptionsManager.SubscriptionInfo>());
                }

                if (_replyHandlers[eventReplyName].Any(s => s.HandlerType == handlerType))
                {
                    throw new ArgumentException(
                        $"ReplyHandler Type {handlerType.Name} already registered for '{eventReplyName}'", nameof(handlerType));
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
                    _handlers[eventName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.Rpc(handlerType));
                    if (!string.IsNullOrEmpty(eventReplyName))
                    {
                        _replyHandlers[eventReplyName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.Rpc(handlerType));
                    }
                    break;
                case SubscriptionManagerType.RpcClient:
                    break;
                case SubscriptionManagerType.RpcServer:
                    _handlers[eventName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.RpcServer(handlerType));
                    if (!string.IsNullOrEmpty(eventReplyName))
                    {
                        _replyHandlers[eventReplyName].Add(InMemoryEventBusSubscriptionsManager.SubscriptionInfo.RpcServer(handlerType));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionManagerType), subscriptionManagerType, null);
            }
        }
        #endregion

        #region [RemoveSubscription]

        public void RemoveDynamicSubscription<TH>(string routingKey)
            where TH : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = FindDynamicSubscriptionToRemove<TH>(routingKey);
            DoRemoveHandler(routingKey, handlerToRemove);
        }

        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();
            var eventName = GetEventKey<T>();
            DoRemoveHandler(eventName, handlerToRemove);
        }

        public void RemoveSubscription<T, TH>(string routingKey)
            where TH : IIntegrationEventHandler<T>
            where T : IIntegrationEvent
        {
            var eventName = GetEventKey<T>();
            var handlerToRemove = FindSubscriptionToRemove<T, TH>(eventName + "." + routingKey);
            
            DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        public void RemoveSubscriptionQueue<T, TH>(string routingKey)
            where TH : IIntegrationQueueHandler<T>
            where T : IIntegrationEvent
        {
            var eventName = GetEventKey<T>();
            var handlerToRemove = FindSubscriptionQueueToRemove<T, TH>(eventName + "." + routingKey);
            DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        public void RemoveSubscriptionRpc<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcHandler<T, TR>
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
        {
            var eventName = GetEventKey<T>();
            var eventNameResult = GetEventReplyKey<TR>();
            var handlerToRemove = FindSubscriptionRpcToRemove<T, TR, TH>(eventName + "." + routingKey);
            var handlerToRemoveReply = FindSubscriptionRpcToRemoveReply<T, TR, TH>(eventNameResult + "." + routingKey);
            DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
            DoRemoveHandlerReply(eventNameResult + "." + routingKey, handlerToRemoveReply);
        }

        public void RemoveSubscriptionRpcClient<TR, TH>(string routingKey)
            where TH : IIntegrationRpcClientHandler<TR>
            where TR : IIntegrationEventReply
        {
            var eventNameResult = GetEventReplyKey<TR>();
            var handlerToRemoveReply = FindSubscriptionRpcClientToRemoveReply<TR, TH>(eventNameResult + "." + routingKey);
            DoRemoveHandlerReply(eventNameResult + "." + routingKey, handlerToRemoveReply);
        }

        public void RemoveSubscriptionRpcServer<T, TR, TH>(string routingKey)
            where TH : IIntegrationRpcServerHandler<T, TR>
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
        {
            var eventName = GetEventKey<T>();
            var handlerToRemove = FindSubscriptionRpcServerToRemove<T, TR, TH>(eventName + "." + routingKey);
            DoRemoveHandler(eventName + "." + routingKey, handlerToRemove);
        }

        private void DoRemoveHandler(string eventName, InMemoryEventBusSubscriptionsManager.SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _handlers[eventName].Remove(subsToRemove);
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);

                    if (_eventTypes.ContainsKey(eventName))
                    {
                        _eventTypes.Remove(eventName);
                    }

                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        private void DoRemoveHandlerReply(string eventReplyName, InMemoryEventBusSubscriptionsManager.SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)
            {
                _replyHandlers[eventReplyName].Remove(subsToRemove);
                if (!_replyHandlers[eventReplyName].Any())
                {
                    _replyHandlers.Remove(eventReplyName);

                    if (_eventReplyTypes.ContainsKey(eventReplyName))
                    {
                        _eventReplyTypes.Remove(eventReplyName);
                    }

                    RaiseOnEventReplyRemoved(eventReplyName);
                }
            }
        }

        #endregion

        public IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent<T>() where T : IIntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEventReply<TR>() where TR : IIntegrationEventReply
        {
            var key = GetEventReplyKey<TR>();
            return GetHandlersForEventReply(key);
        }

        public IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEventReply(string eventName) => _replyHandlers[eventName];

        private void RaiseOnEventRemoved(string eventName)
        {
            OnEventRemoved?.Invoke(this, eventName);
        }

        private void RaiseOnEventReplyRemoved(string eventName)
        {
            OnEventReplyRemoved?.Invoke(this, eventName);
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindDynamicSubscriptionToRemove<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindSubscriptionToRemove<T, TH>()
             where T : IIntegrationEvent
             where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindSubscriptionToRemove<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            return DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindSubscriptionQueueToRemove<T, TH>(string routingKey)
            where T : IIntegrationEvent
            where TH : IIntegrationQueueHandler<T>
        {
            return DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindSubscriptionRpcToRemove<T, TR, TH>(string routingKey)
            where T :IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>
        {
            return DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindSubscriptionRpcToRemoveReply<T, TR, TH>(string routingKey)
            where T : IIntegrationEvent
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcHandler<T, TR>
        {
            return DoFindSubscriptionToRemoveReply(routingKey, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindSubscriptionRpcClientToRemoveReply<TR, TH>(string routingKey)
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcClientHandler<TR>
        {
            return DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo FindSubscriptionRpcServerToRemove<T, TR, TH>(string routingKey)
            where T : IIntegrationEventRpc
            where TR : IIntegrationEventReply
            where TH : IIntegrationRpcServerHandler<T, TR>
        {
            return DoFindSubscriptionToRemove(routingKey, typeof(TH));
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            return !HasSubscriptionsForEvent(eventName) ? null : _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        private InMemoryEventBusSubscriptionsManager.SubscriptionInfo DoFindSubscriptionToRemoveReply(string eventReplyName, Type handlerType)
        {
            return !HasSubscriptionsForEventReply(eventReplyName) ? null : _replyHandlers[eventReplyName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        public bool HasSubscriptionsForEvent<T>() where T : IIntegrationEvent
        {
            
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }

        public bool HasSubscriptionsForEventReply<TR>() where TR : IIntegrationEventReply
        {

            var key = GetEventReplyKey<TR>();
            return HasSubscriptionsForEventReply(key);
        }

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public bool HasSubscriptionsForEventReply(string eventName) => _replyHandlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName)
        {
            return _eventTypes.ContainsKey(eventName) ? _eventTypes[eventName] : null;
        }

        public Type GetEventReplyTypeByName(string eventResultName)
        {
            return _eventReplyTypes.ContainsKey(eventResultName) ? _eventReplyTypes[eventResultName] : null;
        }

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        public string GetEventReplyKey<TR>()
        {
            return typeof(TR).Name;
        }

    }
}
