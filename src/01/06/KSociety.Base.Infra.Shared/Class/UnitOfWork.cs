namespace KSociety.Base.Infra.Shared.Class
{
    using KSociety.Base.Infra.Abstraction.Interface;
    using System.Threading;
    using System.Threading.Tasks;
    using Interface;
    using KSociety.Base.InfraSub.Shared.Class;

    ///<inheritdoc cref="IDatabaseUnitOfWork"/>
    public class UnitOfWork<TContext> : DisposableObject, IDatabaseUnitOfWork
        where TContext : DatabaseContext
    {
        private IDatabaseFactory<TContext>? _dbFactory;
        private TContext? _context;

        public UnitOfWork(IDatabaseFactory<TContext> dbFactory)
        {
            this._dbFactory = dbFactory;
        }

        public TContext? Context => this._context ??= this._dbFactory?.Get();

        ///<inheritdoc/>
        public string? GetConnectionString()
        {
            return this.Context?.GetConnectionString();
        }

        ///<inheritdoc/>
        public ValueTask<string?> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            var result = this.Context?.GetConnectionString();
            return new ValueTask<string?>(result);
        }

        ///<inheritdoc/>
        public bool? Exists()
        {
            return this.Context?.Exists();
        }

        public async ValueTask<bool?> ExistsAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null) {return await this.Context.ExistsAsync(cancellationToken).ConfigureAwait(false);}

            return null;
        }

        ///<inheritdoc/>
        public bool? EnsureCreated()
        {
            return this.Context?.EnsureCreated();
        }

        ///<inheritdoc/>
        public async ValueTask<bool?> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null) {return await this.Context.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);}
            return null;
        }

        ///<inheritdoc/>
        public bool? EnsureDeleted()
        {
            return this.Context?.EnsureDeleted();
        }

        ///<inheritdoc/>
        public async ValueTask<bool?> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null) {return await this.Context.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);}
            return null;
        }

        ///<inheritdoc/>
        public void Migrate(string? targetMigration = null)
        {
            this.Context?.Migrate(targetMigration);
        }

        ///<inheritdoc/>
        public async ValueTask MigrateAsync(string? targetMigration = null,
            CancellationToken cancellationToken = default)
        {
            if (this.Context != null) {await this.Context.MigrateAsync(targetMigration, cancellationToken).ConfigureAwait(false);}
        }

        public string? CreateScript()
        {
            return this.Context?.CreateScript();
        }

        ///<inheritdoc/>
        public void BeginTransaction()
        {
            this.Context?.BeginTransaction();
        }

        ///<inheritdoc/>
        public async ValueTask BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null) {await this.Context.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);}
        }

        ///<inheritdoc/>
        public int? Commit()
        {
            return this.Context?.Commit();
        }

        ///<inheritdoc/>
        public async ValueTask<int?> CommitAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null) {return await this.Context.CommitAsync(cancellationToken).ConfigureAwait(false);}
            return null;
        }

        ///<inheritdoc/>
        public void CommitTransaction()
        {
            this.Context?.CommitTransaction();
        }

        ///<inheritdoc/>
        public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (this.Context != null) {await this.Context.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);}
        }

        ///<inheritdoc/>
        public void Rollback()
        {
            this.Context?.Rollback();
        }

        ///<inheritdoc/>
        public async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
        {
            if(this.Context != null) {await this.Context.RollbackAsync(cancellationToken).ConfigureAwait(false);}
        }

        ///<inheritdoc/>
        protected override void DisposeManagedResources()
        {
            if (this._dbFactory != null)
            {
                this._dbFactory.Dispose();
                this._dbFactory = null;
            }

            if (this._context == null) {return;}
            this._context.Dispose();
            this._context = null;
        }
    }
}
