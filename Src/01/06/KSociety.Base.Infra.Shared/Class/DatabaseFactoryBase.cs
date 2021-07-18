using System;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.InfraSub.Shared.Class;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Class
{
    public class DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>
        : DisposableObject, IDatabaseFactoryBase<TContext>
        where TLoggerFactory : ILoggerFactory
        where TConfiguration : IDatabaseConfiguration
        where TContext : DbContext
    {
        private TContext _dataContext;

        private readonly ILogger _logger;
        private readonly TLoggerFactory _loggerFactory;
        private readonly TConfiguration _configuration;
        private readonly IMediator _mediator;


        protected DatabaseFactoryBase(TLoggerFactory loggerFactory, TConfiguration configuration)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;

            _logger = loggerFactory.CreateLogger<DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>>();
        }

        protected DatabaseFactoryBase(TLoggerFactory loggerFactory, TConfiguration configuration, IMediator mediator)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _mediator = mediator;

            _logger = loggerFactory.CreateLogger<DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>>();
        }

        public TContext Get()
        {
            if (_dataContext is not null) return _dataContext;
            _dataContext = CreateContext();
            //if (!Exists())
            //{
            //    EnsureCreated();
            //}

            return _dataContext;
        }

        private TContext CreateContext()
        {
            TContext output = null;
            try
            {
                if (_mediator is null)
                {
                    output = (TContext)Activator.CreateInstance(typeof(TContext), _loggerFactory, _configuration);
                }
                else
                {
                    output = (TContext)Activator.CreateInstance(typeof(TContext), _loggerFactory, _configuration,
                        _mediator);
                }

                //output.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateContext: ");
                //Console.WriteLine(@"Base CreateContext: " + ex.Message + @" - " + ex.StackTrace);
                //throw ex.InnerExceptio
            }

            return output;
        }

        protected override void DisposeManagedResources()
        {

            if (_dataContext == null) { return; }
            _dataContext.Dispose();
            _dataContext = null;
        }
    }
}
