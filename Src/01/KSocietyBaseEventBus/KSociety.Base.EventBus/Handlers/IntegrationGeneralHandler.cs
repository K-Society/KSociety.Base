using Autofac;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    public class IntegrationGeneralHandler : IIntegrationGeneralHandler
    {
        protected readonly ILoggerFactory? LoggerFactory;
        protected readonly IComponentContext ComponentContext;

        protected IntegrationGeneralHandler(IComponentContext componentContext)
        {
            ComponentContext = componentContext;
        }

        protected IntegrationGeneralHandler(ILoggerFactory loggerFactory, IComponentContext componentContext)
        : this(componentContext)
        {
            LoggerFactory = loggerFactory;
        }
    }
}