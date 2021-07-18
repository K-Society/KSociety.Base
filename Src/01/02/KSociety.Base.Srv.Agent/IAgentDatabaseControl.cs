using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentDatabaseControl
    {
        string GetConnectionString();
        string GetConnectionString(CancellationToken cancellationToken);
        ValueTask<string> GetConnectionStringAsync();
        ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken);

        bool EnsureCreated();
        bool EnsureCreated(CancellationToken cancellationToken);
        ValueTask<bool> EnsureCreatedAsync();
        ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken);

        bool EnsureDeleted();
        bool EnsureDeleted(CancellationToken cancellationToken);
        ValueTask<bool> EnsureDeletedAsync();
        ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken);

        void Migration();
        void Migration(CancellationToken cancellationToken);
        ValueTask MigrationAsync();
        ValueTask MigrationAsync(CancellationToken cancellationToken);
    }
}
