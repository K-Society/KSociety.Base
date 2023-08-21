namespace KSociety.Base.EventBus.Handlers
{
    using Autofac;
    using Abstractions.Handler;
    using Microsoft.Extensions.Logging;

    ///<inheritdoc cref="IIntegrationGeneralHandler"/>
    public abstract class IntegrationGeneralHandler : IIntegrationGeneralHandler
    {
        protected readonly ILoggerFactory? LoggerFactory;
        protected readonly IComponentContext? ComponentContext;

        #region [Constructors]

        protected IntegrationGeneralHandler(IComponentContext? componentContext = default)
        {
            this.ComponentContext = componentContext;
        }

        protected IntegrationGeneralHandler(ILoggerFactory? loggerFactory = default, IComponentContext? componentContext = default)
        : this(componentContext)
        {
            this.LoggerFactory = loggerFactory;
        }

        protected IntegrationGeneralHandler(ILoggerFactory? loggerFactory = default)
        {
            this.LoggerFactory = loggerFactory;
        }

        #endregion
    }
}
