// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus
{
    using System;

    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        public class SubscriptionInfo
        {
            public SubscriptionManagerType SubscriptionManagerType { get; }

            public Type HandlerType { get; }

            private SubscriptionInfo(SubscriptionManagerType subscriptionManagerType, Type handlerType)
            {
                this.SubscriptionManagerType = subscriptionManagerType;
                this.HandlerType = handlerType;
            }

            public static SubscriptionInfo Dynamic(Type handlerType)
            {
                return new SubscriptionInfo(SubscriptionManagerType.Dynamic, handlerType);
            }

            public static SubscriptionInfo Typed(Type handlerType)
            {
                return new SubscriptionInfo(SubscriptionManagerType.Typed, handlerType);
            }

            public static SubscriptionInfo Queue(Type handlerType)
            {
                return new SubscriptionInfo(SubscriptionManagerType.Queue, handlerType);
            }

            public static SubscriptionInfo Rpc(Type handlerType)
            {
                return new SubscriptionInfo(SubscriptionManagerType.Rpc, handlerType);
            }

            public static SubscriptionInfo RpcClient(Type handlerType)
            {
                return new SubscriptionInfo(SubscriptionManagerType.RpcClient, handlerType);
            }

            public static SubscriptionInfo RpcServer(Type handlerType)
            {
                return new SubscriptionInfo(SubscriptionManagerType.RpcServer, handlerType);
            }
        }
    }
}
