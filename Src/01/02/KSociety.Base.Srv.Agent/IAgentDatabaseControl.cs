using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentDatabaseControl
    {
        string GetConnectionString(CancellationToken cancellationToken = default);

        ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default);

        bool EnsureCreated(CancellationToken cancellationToken = default);

        ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default);

        bool EnsureDeleted(CancellationToken cancellationToken = default);

        ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default);

        void Migration(CancellationToken cancellationToken = default);

        ValueTask MigrationAsync(CancellationToken cancellationToken = default);
    }
}