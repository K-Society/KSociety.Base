using System;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.InfraSub.Shared.Class;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Class;

public class DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>
    : DisposableObject, IDatabaseFactoryBase<TContext>
    where TLoggerFactory : ILoggerFactory
    where TConfiguration : IDatabaseConfiguration
    where TContext : DbContext
{
    private TContext _dataContext;

    protected readonly ILogger Logger;
    protected readonly TLoggerFactory LoggerFactory;
    private readonly TConfiguration _configuration;
    private readonly IMediator _mediator;


    protected DatabaseFactoryBase(TLoggerFactory loggerFactory, TConfiguration configuration)
    {
        LoggerFactory = loggerFactory;
        _configuration = configuration;

        Logger = loggerFactory.CreateLogger<DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>>();
    }

    protected DatabaseFactoryBase(TLoggerFactory loggerFactory, TConfiguration configuration, IMediator mediator)
    {
        LoggerFactory = loggerFactory;
        _configuration = configuration;
        _mediator = mediator;

        Logger = loggerFactory.CreateLogger<DatabaseFactoryBase<TLoggerFactory, TConfiguration, TContext>>();
    }

    public TContext Get()
    {
        if (_dataContext is not null) return _dataContext;
        _dataContext = CreateContext();

        return _dataContext;
    }

    private TContext CreateContext()
    {
        TContext output = null;
        try
        {
            if (_mediator is null)
            {
                output = (TContext)Activator.CreateInstance(typeof(TContext), LoggerFactory, _configuration);
            }
            else
            {
                output = (TContext)Activator.CreateInstance(typeof(TContext), LoggerFactory, _configuration,
                    _mediator);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CreateContext: ");
        }

        return output;
    }

    protected override void DisposeManagedResources()
    {

        if (_dataContext == null) return;
        _dataContext.Dispose();
        _dataContext = null;
    }
}