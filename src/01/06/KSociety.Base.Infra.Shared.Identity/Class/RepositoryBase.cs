namespace KSociety.Base.Infra.Shared.Identity.Class
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using KSociety.Base.InfraSub.Shared.Class.Csv;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Extensions.Logging;

    public abstract class RepositoryBase<TContext, TEntity, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin,
        TRoleClaim, TUserToken> : KSociety.Base.Infra.Shared.Interface.IRepositoryBase<TEntity>
        where TContext : Shared.Identity.Class.DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TEntity : class
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        private TContext? _dataContext;
        private IUserStore<TUser>? _userStore;
        private IRoleStore<TRole>? _roleStore;
        protected readonly ILoggerFactory LoggerFactory;

        protected TContext? DataContext => this._dataContext ??= this.DatabaseFactory?.Get();
        protected IUserStore<TUser>? UserStore => this._userStore ??= this.DatabaseFactory?.GetUserStore();
        protected IRoleStore<TRole>? RoleStore => this._roleStore ??= this.DatabaseFactory?.GetRoleStore();

        protected readonly DbSet<TEntity>? DataBaseSet;

        protected readonly ILogger<RepositoryBase<TContext, TEntity, TUser, TRole, TKey, TUserClaim, TUserRole,
            TUserLogin, TRoleClaim, TUserToken>> Logger;

        protected Interface.IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken>? DatabaseFactory { get; private set; }

        protected bool? Exists => this.DataContext?.Exists();

        protected RepositoryBase(ILoggerFactory loggerFactory, Interface.IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
                databaseFactory)
        {
            this.LoggerFactory = loggerFactory;
            this.DatabaseFactory = databaseFactory;
            this.DataBaseSet = this.DataContext?.Set<TEntity>();

            this.Logger = this.LoggerFactory
                .CreateLogger<RepositoryBase<TContext, TEntity, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin,
                    TRoleClaim, TUserToken>>();
        }

        public virtual EntityEntry<TEntity>? Add(TEntity entity)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = this.DataBaseSet?.Add(entity);
            this.Logger.LogTrace("RepositoryBase Add: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName +
                                 ")" + " State: " + result?.State);
            return result;
        }

        public virtual async ValueTask<EntityEntry<TEntity>?> AddAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = await this.DataBaseSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            this.Logger.LogTrace("RepositoryBase AddAsync: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName +
                                 ")" + " State: " + result.State);
            return result;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
            }
            else
            {

                this.DataBaseSet?.AddRange(entities);
                this.Logger.LogTrace("RepositoryBase AddRange: " + this.GetType().FullName + "." +
                                     System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual async ValueTask AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
            }
            else
            {
                await this.DataBaseSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                this.Logger.LogTrace("RepositoryBase AddRangeAsync: " + this.GetType().FullName + "." +
                                     System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual EntityEntry<TEntity>? Update(TEntity entity)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = this.DataBaseSet?.Update(entity);
            this.Logger.LogTrace("RepositoryBase Update: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName +
                                 ")" + " State: " + result?.State);
            return result;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
            }
            else
            {

                this.DataBaseSet?.UpdateRange(entities);
                this.Logger.LogTrace("RepositoryBase UpdateRange: " + this.GetType().FullName + "." +
                                     System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual EntityEntry<TEntity>? Delete(TEntity entity)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = this.DataBaseSet?.Remove(entity);
            this.Logger.LogTrace("RepositoryBase Delete: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName +
                                 ")" + " State: " + result?.State);
            return result;
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
            }
            else
            {

                this.DataBaseSet?.RemoveRange(entities);
                this.Logger.LogTrace("RepositoryBase DeleteRange: " + this.GetType().FullName + "." +
                                     System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual IEnumerable<EntityEntry<TEntity>> Delete(Expression<Func<TEntity, bool>> where)
        {
            //Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." +
            //                System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + where.GetType().FullName +
            //                ")");
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            IEnumerable<TEntity> objects = this.DataBaseSet.Where(@where).AsEnumerable();
            IEnumerable<EntityEntry<TEntity>> output = new List<EntityEntry<TEntity>>();
            foreach (var obj in objects)
            {
                output.Append(this.DataBaseSet.Remove(obj));
            }

            return output;

        }

        public virtual TEntity? First(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null ? this.DataBaseSet?.FirstOrDefault() : this.DataBaseSet?.FirstOrDefault(filter);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual async ValueTask<TEntity?> FirstAsync(Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null
                    ? await this.DataBaseSet.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
                    : await this.DataBaseSet.FirstOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual TEntity? Last<TKeySelector>(Expression<Func<TEntity, TKeySelector>> keySelector,
            Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null
                    ? this.DataBaseSet?.OrderBy(keySelector).LastOrDefault()
                    : this.DataBaseSet?.OrderBy(keySelector).LastOrDefault(filter);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual async ValueTask<TEntity?> LastAsync<TKeySelector>(
            Expression<Func<TEntity, TKeySelector>> keySelector, Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null
                    ? await this.DataBaseSet.OrderBy(keySelector).LastOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false)
                    : await this.DataBaseSet.OrderBy(keySelector).LastOrDefaultAsync(filter, cancellationToken)
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public TEntity? Find(params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase Find: " + GetType().FullName + "." +
            //                System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName +
            //                ")");

            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            return this.DataBaseSet?.Find(keyObject);
        }

        public async ValueTask<TEntity?> FindAsync(CancellationToken cancellationToken = default,
            params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase FindAsync: " + GetType().FullName + "." +
            //System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName +
            //")");
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }
            return await this.DataBaseSet.FindAsync(keyObject).ConfigureAwait(false);
        }

        public virtual int? Count(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null ? this.DataBaseSet?.Count() : this.DataBaseSet?.Count(filter);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return -1;
            }
        }

        public virtual async ValueTask<int?> CountAsync(Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {

            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            } 
            
            try
            {
                if (filter is null)
                {
                    return await this.DataBaseSet.CountAsync(cancellationToken).ConfigureAwait(false);
                }

                return await this.DataBaseSet.CountAsync(filter, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return -1;
            }
        }

        public IQueryable<TEntity>? Query(Expression<Func<TEntity, bool>>? filter = null)
        {
            //Logger.LogTrace("RepositoryBase Query: " + GetType().FullName + "." +
            //                System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName +
            //                ")");

            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            if (filter is not null)
            {
                return this.DataBaseSet?.Where(filter);
            }

            return this.DataBaseSet;
        }

        public IQueryable<TEntity>? QueryObjectGraph(Expression<Func<TEntity, bool>> filter, string children)
        {
            //Logger.LogTrace("RepositoryBase QueryObjectGraph: " + GetType().FullName + "." +
            //                System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName +
            //                "," + children + ")");

            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            return this.DataBaseSet?.Include(children).Where(filter);
        }

        public IQueryable<TEntity>? FindAll()
        {
            //Logger.LogTrace("RepositoryBase FindAll: " + GetType().FullName + "." +
            //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            return this.DataBaseSet;
        }

        public virtual IQueryable<TEntity>? GetPaged<TKeySelector>(
            int pageNumber, int pageSize,
            Expression<Func<TEntity, TKeySelector>>? keySelector = null,
            Expression<Func<TEntity, bool>>? filter = null)
        {
            //Logger.LogTrace("RepositoryBase FindAll: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }


            try
            {
                var skip = (pageNumber - 1) * pageSize;
                if (filter is null)
                {
                    return keySelector is null
                        ? this.DataBaseSet?.Skip(skip).Take(pageSize)
                        : this.DataBaseSet?.OrderBy(keySelector).Skip(skip).Take(pageSize);
                }

                return keySelector is null
                    ? this.DataBaseSet?.Where(filter).Skip(skip).Take(pageSize)
                    : this.DataBaseSet?.Where(filter).OrderBy(keySelector).Skip(skip).Take(pageSize);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public void ImportCsv(string fileName)
        {
            this.Logger.LogTrace("RepositoryBase ImportCsv: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var result = ReadCsv<TEntity>.Import(this.LoggerFactory, fileName);
            if (!result.Any())
            {
                return;
            }

            this.DeleteRange(this.FindAll());

            this.AddRange(result);
        }

        public async ValueTask ImportCsvAsync(string fileName, CancellationToken cancellationToken = default)
        {
            this.Logger.LogTrace("RepositoryBase ImportCsvAsync: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var result = ReadCsv<TEntity>.ImportAsync(this.LoggerFactory, fileName, cancellationToken);

            this.DeleteRange(this.FindAll());

            await foreach (var entity in result.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                await this.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            }

        }

        public void ExportCsv(string fileName)
        {
            this.Logger.LogTrace("RepositoryBase ExportCsv: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            WriteCsv<TEntity>.Export(this.LoggerFactory, fileName, this.FindAll());
        }

        public async ValueTask ExportCsvAsync(string fileName, CancellationToken cancellationToken = default)
        {
            this.Logger.LogTrace("RepositoryBase ExportCsvAsync: " + this.GetType().FullName + "." +
                                 System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            await WriteCsv<TEntity>.ExportAsync(this.LoggerFactory, fileName, this.FindAll()).ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.DataContext?.Dispose();
        }
    }
}
