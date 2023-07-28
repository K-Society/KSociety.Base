using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    ///<inheritdoc cref="IIntegrationQueueHandler{TIntegrationEventReply}"/>
    public class IntegrationRpcClientHandler<TIntegrationEventReply>
        : IntegrationGeneralHandler, IIntegrationRpcClientHandler<TIntegrationEventReply>
        where TIntegrationEventReply : IIntegrationEventReply
    {
        protected readonly ILogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>? Logger;

        #region [Constructors]

        public IntegrationRpcClientHandler(ILoggerFactory? loggerFactory = default)
            : base(loggerFactory)
        {
            Logger = LoggerFactory?.CreateLogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>();
        }

        public IntegrationRpcClientHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default)
            : base(loggerFactory, componentContext)
        {
            Logger = LoggerFactory?.CreateLogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>();
        }

        public IntegrationRpcClientHandler(ILogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>? logger = default, IComponentContext? componentContext = default)
            : base(componentContext)
        {
            Logger = logger;
        }

        #endregion

        public virtual void HandleReply(TIntegrationEventReply @integrationEventReply,
            CancellationToken cancel = default)
        {
            Logger?.LogWarning("IntegrationRpcHandler HandleRpcAsync: {0}, routing key: {1}", "NotImplemented!",
                @integrationEventReply.RoutingKey);
        }

        public virtual async ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply,
            CancellationToken cancel = default)
        {
            Logger?.LogWarning("IntegrationRpcHandler HandleRpcAsync: {0}, routing key: {1}", "NotImplemented!",
                @integrationEventReply.RoutingKey);
        }
    }
}