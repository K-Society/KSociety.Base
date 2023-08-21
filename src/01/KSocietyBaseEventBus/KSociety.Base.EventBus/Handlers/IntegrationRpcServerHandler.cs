namespace KSociety.Base.EventBus.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Abstractions;
    using Abstractions.Handler;
    using Microsoft.Extensions.Logging;

    ///<inheritdoc cref="IIntegrationRpcServerHandler{TIntegrationEvent, TIntegrationEventReply}"/>
    public class IntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        : IntegrationGeneralHandler, IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>
        where TIntegrationEvent : IIntegrationEventRpc
        where TIntegrationEventReply : IIntegrationEventReply
    {
        protected readonly ILogger<IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>>? Logger;

        #region [Constructors]

        public IntegrationRpcServerHandler(ILoggerFactory? loggerFactory = default)
            : base(loggerFactory)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>>();
        }

        public IntegrationRpcServerHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default)
            : base(loggerFactory, componentContext)
        {
            this.Logger = this.LoggerFactory?.CreateLogger<IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>>();
        }

        public IntegrationRpcServerHandler(ILogger<IIntegrationRpcServerHandler<TIntegrationEvent, TIntegrationEventReply>>? logger = default, IComponentContext? componentContext = default)
            : base(componentContext)
        {
            this.Logger = logger;
        }

        #endregion

        public virtual TIntegrationEventReply HandleRpc(TIntegrationEvent @event, CancellationToken cancel = default)
        {
            this.Logger?.LogWarning("IntegrationRpcHandler HandleRpc: {0}, routing key: {1}", "NotImplemented!",
                @event.RoutingKey);
            throw new NotImplementedException();
        }

        public virtual ValueTask<TIntegrationEventReply> HandleRpcAsync(TIntegrationEvent @event,
            CancellationToken cancel = default)
        {
            this.Logger?.LogWarning("IntegrationRpcHandler HandleRpcAsync: {0}, routing key: {1}", "NotImplemented!",
                @event.RoutingKey);
            throw new NotImplementedException();
        }
    }
}
