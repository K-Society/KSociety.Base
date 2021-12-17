using Autofac;
using KSociety.Base.App.Shared.Dto.Res.Control;
using KSociety.Base.Srv.Contract.Control;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace KSociety.Base.Srv.Behavior.Control;

public class DatabaseControl : IDatabaseControl
{
    private readonly ILogger<DatabaseControl> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IComponentContext _componentContext;
    private readonly ICommandHandler _commandHandler;

    public DatabaseControl(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandler commandHandler
    )
    {
        _loggerFactory = loggerFactory;
        _componentContext = componentContext;
        _commandHandler = commandHandler;
        _logger = _loggerFactory.CreateLogger<DatabaseControl>();
    }

    public EnsureCreated EnsureCreated(CallContext context = default)
    {
        _logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        return _commandHandler.ExecuteWithResponse<EnsureCreated>(_loggerFactory, _componentContext);
    }

    public EnsureDeleted EnsureDeleted(CallContext context = default)
    {
        _logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        return _commandHandler.ExecuteWithResponse<EnsureDeleted>(_loggerFactory, _componentContext);
    }

    public void Migration(CallContext context = default)
    {
        _logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        _commandHandler.Execute(_loggerFactory, _componentContext, "MigrationReqHdlr");
    }

    public ConnectionString GetConnectionString(CallContext context = default)
    {
        _logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        return _commandHandler.ExecuteWithResponse<ConnectionString>(_loggerFactory, _componentContext);
    }
}