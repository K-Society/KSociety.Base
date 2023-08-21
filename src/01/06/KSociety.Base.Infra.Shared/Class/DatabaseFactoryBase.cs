namespace KSociety.Base.Infra.Shared.Class
{
    using KSociety.Base.Infra.Abstraction.Interface;
    using System;
    using Interface;
    using KSociety.Base.InfraSub.Shared.Class;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>
        : DisposableObject, IDatabaseFactoryBase<TContext>
        where TLoggerFactory : ILoggerFactory
        where TConfiguration : IDatabaseConfiguration
        where TContext : DbContext
    {
        private TContext? _dataContext;

        protected readonly ILogger Logger;
        protected readonly TLoggerFactory LoggerFactory;
        private readonly TConfiguration _configuration;
        private readonly IMediator? _mediator;


        protected DatabaseFactoryBase(TLoggerFactory loggerFactory, TConfiguration configuration)
        {
            this.LoggerFactory = loggerFactory;
            this._configuration = configuration;

            this.Logger = loggerFactory.CreateLogger<DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>>();
        }

        protected DatabaseFactoryBase(TLoggerFactory loggerFactory, TConfiguration configuration, IMediator mediator)
        {
            this.LoggerFactory = loggerFactory;
            this._configuration = configuration;
            this._mediator = mediator;

            this.Logger = loggerFactory.CreateLogger<DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>>();
        }

        public TContext? Get()
        {
            if (this._dataContext != null) {return this._dataContext;}
            this._dataContext = this.CreateContext();

            return this._dataContext;
        }

        private TContext? CreateContext()
        {
            TContext? output = null;
            try
            {
                if (this._mediator is null)
                {
                    output = (TContext?)Activator.CreateInstance(typeof(TContext), this.LoggerFactory, this._configuration);
                }
                else
                {
                    output = (TContext?)Activator.CreateInstance(typeof(TContext), this.LoggerFactory, this._configuration, this._mediator);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CreateContext: ");
            }

            return output;
        }

        protected override void DisposeManagedResources()
        {

            if (this._dataContext == null) {return;}
            this._dataContext.Dispose();
            this._dataContext = null;
        }
    }
}
