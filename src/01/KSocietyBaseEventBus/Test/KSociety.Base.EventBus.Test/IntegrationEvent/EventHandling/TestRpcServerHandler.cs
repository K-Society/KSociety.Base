// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Handlers;
using Event;
using Microsoft.Extensions.Logging;

public class TestRpcServerHandler : IntegrationRpcServerHandler<TestIntegrationEventRpc, TestIntegrationEventReply>
{
    //private readonly IBiz _biz;
    public TestRpcServerHandler(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext
    )
        : base(loggerFactory, componentContext)
    {
        //if (ComponentContext.IsRegistered<IBiz>())
        //{
        //    _biz = ComponentContext.Resolve<IBiz>();
        //}
        //else
        //{
        //    Logger.LogError("IBiz not Registered!");
        //}
    }

    public override TestIntegrationEventReply HandleRpc(TestIntegrationEventRpc @event, CancellationToken cancellationToken = default)
    {
        //Logger.LogTrace("HandleRpcAsync... " + @event.RoutingKey);
        //var tagValue = await _biz.GetTagValueAsync(@event.GroupName, @event.Name).ConfigureAwait(false);
        //Logger.LogTrace("HandleRpcAsync: " + tagValue);
        ;
        return new TestIntegrationEventReply(@event.ReplyRoutingKey, @event.TestName, @event.ByteArray);
    }

    public override async ValueTask<TestIntegrationEventReply> HandleRpcAsync(TestIntegrationEventRpc @event, CancellationToken cancellationToken = default)
    {
        //Logger.LogTrace("HandleRpcAsync... " + @event.RoutingKey);
        //var tagValue = await _biz.GetTagValueAsync(@event.GroupName, @event.Name).ConfigureAwait(false);
        //Logger.LogTrace("HandleRpcAsync: " + tagValue);
        ;
        return new TestIntegrationEventReply(@event.ReplyRoutingKey, @event.TestName, @event.ByteArray);
    }
}
