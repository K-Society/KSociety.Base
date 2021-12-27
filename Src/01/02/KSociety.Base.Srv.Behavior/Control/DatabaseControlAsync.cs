using System.Threading.Tasks;
using Autofac;
using KSociety.Base.App.Shared.Dto.Res.Control;
using KSociety.Base.Srv.Contract.Control;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace KSociety.Base.Srv.Behavior.Control;

public class DatabaseControlAsync : IDatabaseControlAsync
{
    private readonly ILogger<DatabaseControlAsync> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IComponentContext _componentContext;
    private readonly ICommandHandlerAsync _commandHandlerAsync;

    public DatabaseControlAsync(
        ILoggerFactory loggerFactory,
        IComponentContext componentContext,
        ICommandHandlerAsync commandHandlerAsync
    )
    {
        _loggerFactory = loggerFactory;
        _componentContext = componentContext;
        _commandHandlerAsync = commandHandlerAsync;
        _logger = _loggerFactory.CreateLogger<DatabaseControlAsync>();
    }

    public async ValueTask<EnsureCreated> EnsureCreatedAsync(CallContext context = default)
    {
        _logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        return await _commandHandlerAsync.ExecuteWithResponseAsync<EnsureCreated>(_loggerFactory, _componentContext, context.CancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<EnsureDeleted> EnsureDeletedAsync(CallContext context = default)
    {
        _logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        return await _commandHandlerAsync.ExecuteWithResponseAsync<EnsureDeleted>(_loggerFactory, _componentContext, context.CancellationToken).ConfigureAwait(false);
    }

    public async ValueTask MigrationAsync(CallContext context = default)
    {
        _logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        await _commandHandlerAsync.ExecuteAsync(_loggerFactory, _componentContext, "MigrationReqHdlr", context.CancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<ConnectionString> GetConnectionStringAsync(CallContext context = default)
    {
        _logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        return await _commandHandlerAsync.ExecuteWithResponseAsync<ConnectionString>(_loggerFactory, _componentContext).ConfigureAwait(false);
    }
}