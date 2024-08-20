// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Handlers;
    using Event;
    using Microsoft.Extensions.Logging;

    public class TestRpcServerHandler : IntegrationRpcServerHandler<TestIntegrationEventRpc, TestIntegrationEventReply>
    {
        public TestRpcServerHandler(
            ILoggerFactory loggerFactory,
            IComponentContext componentContext
        )
            : base(loggerFactory, componentContext)
        {
            
        }

        public override TestIntegrationEventReply HandleRpc(TestIntegrationEventRpc @event,
            CancellationToken cancellationToken = default)
        {
            
            return new TestIntegrationEventReply(@event.ReplyRoutingKey, @event.TestName, @event.ByteArray);
        }

        public override async ValueTask<TestIntegrationEventReply> HandleRpcAsync(TestIntegrationEventRpc @event,
            CancellationToken cancellationToken = default)
        {
            return new TestIntegrationEventReply(@event.ReplyRoutingKey, @event.TestName, @event.ByteArray);
        }
    }
}
