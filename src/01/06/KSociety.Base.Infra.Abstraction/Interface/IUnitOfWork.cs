// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Abstraction.Interface
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The UnitOfWork class.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Exists
        /// </summary>
        /// <returns></returns>
        bool? Exists();

        /// <summary>
        /// ExistsAsync
        /// </summary>
        /// <returns></returns>
        ValueTask<bool?> ExistsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// EnsureCreated
        /// </summary>
        /// <returns></returns>
        bool? EnsureCreated();

        /// <summary>
        /// EnsureCreatedAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool?> EnsureCreatedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// EnsureDeleted
        /// </summary>
        /// <returns></returns>
        bool? EnsureDeleted();

        /// <summary>
        /// EnsureDeletedAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<bool?> EnsureDeletedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// EnsureDeletedAsync
        /// </summary>
        /// <param name="targetMigration"></param>
        void Migrate(string targetMigration = null);

        /// <summary>
        /// MigrateAsync
        /// </summary
        /// <param name="targetMigration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask MigrateAsync(string targetMigration = null, CancellationToken cancellationToken = default);

        string CreateScript();

        /// <summary>
        /// BeginTransaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// BeginTransactionAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commit
        /// </summary>
        /// <returns></returns>
        int? Commit();

        /// <summary>
        /// CommitAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<int?> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// CommitTransaction
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// CommitTransactionAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rollback
        /// </summary>
        void Rollback();

        /// <summary>
        /// RollbackAsync
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask RollbackAsync(CancellationToken cancellationToken = default);
    }
}
