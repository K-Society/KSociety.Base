﻿using KSociety.Base.Infra.Abstraction.Interface;
using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.Infra.Shared.Interface;
using KSociety.Base.InfraSub.Shared.Class;

namespace KSociety.Base.Infra.Shared.Class
{
    ///<inheritdoc cref="IDatabaseUnitOfWork"/>
    public class UnitOfWork<TContext> : DisposableObject, IDatabaseUnitOfWork
        where TContext : DatabaseContext
    {
        private IDatabaseFactory<TContext>? _dbFactory;
        private TContext? _context;

        public UnitOfWork(IDatabaseFactory<TContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public TContext? Context => _context ??= _dbFactory?.Get();

        ///<inheritdoc/>
        public string? GetConnectionString()
        {
            return Context?.GetConnectionString();
        }

        ///<inheritdoc/>
        public ValueTask<string?> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            var result = Context?.GetConnectionString();
            return new ValueTask<string?>(result);
        }

        ///<inheritdoc/>
        public bool? Exists()
        {
            return Context?.Exists();
        }

        ///<inheritdoc/>
        public bool? EnsureCreated()
        {
            return Context?.EnsureCreated();
        }

        ///<inheritdoc/>
        public async ValueTask<bool?> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            if (Context != null) return await Context.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
            return null;
        }

        ///<inheritdoc/>
        public bool? EnsureDeleted()
        {
            return Context?.EnsureDeleted();
        }

        ///<inheritdoc/>
        public async ValueTask<bool?> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            if (Context != null) return await Context.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            return null;
        }

        ///<inheritdoc/>
        public void Migrate(string? targetMigration = null)
        {
            Context?.Migrate(targetMigration);
        }

        ///<inheritdoc/>
        public async ValueTask MigrateAsync(string? targetMigration = null,
            CancellationToken cancellationToken = default)
        {
            if (Context != null) await Context.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);
        }

        public string? CreateScript()
        {
            return Context?.CreateScript();
        }

        ///<inheritdoc/>
        public void BeginTransaction()
        {
            Context?.BeginTransaction();
        }

        ///<inheritdoc/>
        public async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (Context != null) await Context.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public int? Commit()
        {
            return Context?.Commit();
        }

        ///<inheritdoc/>
        public async ValueTask<int?> CommitAsync(CancellationToken cancellationToken = default)
        {
            if (Context != null) return await Context.CommitAsync(cancellationToken).ConfigureAwait(false);
            return null;
        }

        ///<inheritdoc/>
        public void CommitTransaction()
        {
            Context?.CommitTransaction();
        }

        ///<inheritdoc/>
        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (Context != null) await Context.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        public void Rollback()
        {
            Context?.Rollback();
        }

        ///<inheritdoc/>
        public async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
        {
            if(Context != null) await Context.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }

        ///<inheritdoc/>
        protected override void DisposeManagedResources()
        {
            if (_dbFactory != null)
            {
                _dbFactory.Dispose();
                _dbFactory = null;
            }

            if (_context == null) return;
            _context.Dispose();
            _context = null;
        }
    }
}