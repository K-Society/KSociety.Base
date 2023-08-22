// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Test.IntegrationEvent.EventHandling;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Handler;
using Event;
using Microsoft.Extensions.Logging;

public class TestEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
{
    private readonly ILoggerFactory _loggerFactory;
    private ILogger _logger;

    public TestEventHandler(
        ILoggerFactory loggerFactory
    )
    {
        this._loggerFactory = loggerFactory;
    }

    public async ValueTask Handle(TestIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        ;
    }
}
