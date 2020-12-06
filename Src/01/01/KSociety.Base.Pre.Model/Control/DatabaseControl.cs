using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Srv.Agent;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Pre.Model.Control
{
    public class DatabaseControl : IDatabaseControl
    {
        private readonly ILogger<DatabaseControl> _logger;
        private readonly Srv.Agent.Control.DatabaseControl _databaseControl;

        public DatabaseControl(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DatabaseControl>();
            _databaseControl = new Srv.Agent.Control.DatabaseControl(agentConfiguration, loggerFactory);
        }

        public string GetConnectionString(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GetConnectionString");
            return _databaseControl.GetConnectionString(cancellationToken);
        }

        public async ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GetConnectionStringAsync");
            return await _databaseControl.GetConnectionStringAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool EnsureCreated(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("EnsureCreate");
            return _databaseControl.EnsureCreate(cancellationToken);
        }

        public async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("EnsureCreateAsync");
            return await _databaseControl.EnsureCreateAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool EnsureDeleted(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("EnsureDeleted");
            return _databaseControl.EnsureDeleted(cancellationToken);
        }

        public async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("EnsureDeletedAsync");
            return await _databaseControl.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Migration(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Migration");
            _databaseControl.Migration(cancellationToken);
        }

        public async ValueTask MigrationAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("MigrationAsync");
            await _databaseControl.MigrationAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}