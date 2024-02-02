// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Event;
    using Handlers;
    using Microsoft.Extensions.Logging;

    public class TestRpcHandler : IntegrationRpcHandler<TestIntegrationEventRpc, TestIntegrationEventReply>
    {
        public TestRpcHandler(
            ILoggerFactory loggerFactory,
            IComponentContext componentContext
        )
            : base(loggerFactory, componentContext)
        {
            
        }

        public override async ValueTask<TestIntegrationEventReply> HandleRpcAsync(TestIntegrationEventRpc @event, CancellationToken cancellationToken = default)
        {
            return new TestIntegrationEventReply(@event.ReplyRoutingKey, @event.TestName, @event.ByteArray);
        }
    }
}
