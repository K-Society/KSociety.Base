using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        bool Exists();

        bool EnsureCreated();

        ValueTask<bool> EnsureCreatedAsync();
        ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken);

        bool EnsureDeleted();

        ValueTask<bool> EnsureDeletedAsync();
        ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken);

        void Migrate();

        ValueTask MigrateAsync();
        ValueTask MigrateAsync(CancellationToken cancellationToken);

        void BeginTransaction();

        ValueTask BeginTransactionAsync();
        ValueTask BeginTransactionAsync(CancellationToken cancellationToken);

        int Commit();

        ValueTask<int> CommitAsync();
        ValueTask<int> CommitAsync(CancellationToken cancellationToken);

        void CommitTransaction();

        ValueTask CommitTransactionAsync();
        ValueTask CommitTransactionAsync(CancellationToken cancellationToken);

        void Rollback();

        ValueTask RollbackAsync();
        ValueTask RollbackAsync(CancellationToken cancellationToken);
    }
}
