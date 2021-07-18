using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.EventBus.Abstractions;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    public class IntegrationRpcClientHandler<TIntegrationEventReply>
        : IntegrationGeneralHandler, IIntegrationRpcClientHandler<TIntegrationEventReply>
        where TIntegrationEventReply : IIntegrationEventReply
    {
        protected readonly ILogger<IIntegrationRpcClientHandler<TIntegrationEventReply>> Logger;

        public IntegrationRpcClientHandler(ILoggerFactory loggerFactory, IComponentContext componentContext)
            : base(loggerFactory, componentContext)
        {
            Logger = LoggerFactory.CreateLogger<IIntegrationRpcClientHandler<TIntegrationEventReply>>();
        }

        public virtual void HandleReply(TIntegrationEventReply @integrationEventReply)
        {
            Logger.LogWarning("IntegrationRpcHandler HandleRpcAsync: NotImplemented! " + @integrationEventReply.RoutingKey);
        }

        public virtual void HandleReply(TIntegrationEventReply @integrationEventReply, CancellationToken cancel)
        {
            Logger.LogWarning("IntegrationRpcHandler HandleRpcAsync: NotImplemented! " + @integrationEventReply.RoutingKey);
        }

        public virtual async ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply)
        {
            Logger.LogWarning("IntegrationRpcHandler HandleRpcAsync: NotImplemented! " + @integrationEventReply.RoutingKey);
        }

        public virtual async ValueTask HandleReplyAsync(TIntegrationEventReply @integrationEventReply, CancellationToken cancel)
        {
            Logger.LogWarning("IntegrationRpcHandler HandleRpcAsync: NotImplemented! " + @integrationEventReply.RoutingKey);
        }
    }
}
