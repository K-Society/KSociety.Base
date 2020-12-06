using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        bool Exists();

        bool EnsureCreated();

        ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default);

        bool EnsureDeleted();

        ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default);

        void Migrate();

        ValueTask MigrateAsync(CancellationToken cancellationToken = default);

        void BeginTransaction();

        ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default);

        int Commit();

        ValueTask<int> CommitAsync(CancellationToken cancellationToken = default);

        void CommitTransaction();

        ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default);

        void Rollback();

        ValueTask RollbackAsync(CancellationToken cancellationToken = default);
    }
}
