using Autofac;
using KSociety.Base.EventBus.Abstractions.Handler;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.EventBus.Handlers
{
    ///<inheritdoc cref="IIntegrationGeneralHandler"/>
    public abstract class IntegrationGeneralHandler : IIntegrationGeneralHandler
    {
        protected readonly ILoggerFactory? LoggerFactory;
        protected readonly IComponentContext? ComponentContext;

        #region [Constructors]

        protected IntegrationGeneralHandler(IComponentContext? componentContext = default)
        {
            ComponentContext = componentContext;
        }

        protected IntegrationGeneralHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default)
        : this(componentContext)
        {
            LoggerFactory = loggerFactory;
        }

        protected IntegrationGeneralHandler(ILoggerFactory? loggerFactory = default)
        {
            LoggerFactory = loggerFactory;
        }

        #endregion
    }
}