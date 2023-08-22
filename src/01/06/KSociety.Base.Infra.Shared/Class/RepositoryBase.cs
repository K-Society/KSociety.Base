// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Class
{
    using Interface;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    ///<inheritdoc cref="IRepositoryBase{TEntity}"/>
    public abstract class RepositoryBase<TContext, TEntity> : IRepositoryBase<TEntity>
        where TContext : DatabaseContext
        where TEntity : class
    {
        private TContext? _dataContext;
        protected readonly ILoggerFactory LoggerFactory;

        protected TContext? DataContext => this._dataContext ??= this.DatabaseFactory.Get();

        protected readonly DbSet<TEntity>? DataBaseSet;

        protected readonly ILogger<RepositoryBase<TContext, TEntity>> Logger;

        protected IDatabaseFactory<TContext> DatabaseFactory { get; private set; }

        protected bool? Exists => this.DataContext?.Exists();

        protected RepositoryBase(ILoggerFactory loggerFactory, IDatabaseFactory<TContext> databaseFactory)
        {
            this.LoggerFactory = loggerFactory;
            this.DatabaseFactory = databaseFactory;
            this.DataBaseSet = this.DataContext?.Set<TEntity>();

            this.Logger = this.LoggerFactory.CreateLogger<RepositoryBase<TContext, TEntity>>();
        }

        public virtual EntityEntry<TEntity>? Add(TEntity entity)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = this.DataBaseSet?.Add(entity);
            //Logger.LogTrace("RepositoryBase Add: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
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
            //Logger.LogTrace("RepositoryBase AddAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
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
                //Logger.LogTrace("RepositoryBase AddRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
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
                //Logger.LogTrace("RepositoryBase AddRangeAsync: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
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
            //Logger.LogTrace("RepositoryBase Update: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
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
                //Logger.LogTrace("RepositoryBase UpdateRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
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
            //Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void DeleteRange(IEnumerable<TEntity>? entities)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
            }
            else
            {
                if (entities is not null)
                {
                    this.DataBaseSet?.RemoveRange(entities);
                }
                
                //Logger.LogTrace("RepositoryBase DeleteRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual IEnumerable<EntityEntry<TEntity>?>? Delete(Expression<Func<TEntity, bool>> where)
        {
            //Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + where.GetType().FullName + ")");
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            IEnumerable<TEntity>? objects = this.DataBaseSet?.Where(@where).AsEnumerable();
            IEnumerable<EntityEntry<TEntity>?>? output = new List<EntityEntry<TEntity>>();

            if (objects is not null)
            {
                foreach (var obj in objects)
                {
                    output = output?.Append(this.DataBaseSet?.Remove(obj));
                }
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
                    ? this.DataBaseSet.OrderBy(keySelector).LastOrDefault()
                    : this.DataBaseSet.OrderBy(keySelector).LastOrDefault(filter);
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

        public virtual TEntity? Find(params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase Find: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return this.DataBaseSet?.Find(keyObject);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}({2})", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, keyObject.GetType().FullName);
                return null;
            }
        }

        public virtual async ValueTask<TEntity?> FindAsync(CancellationToken cancellationToken = default,
            params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase FindAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return await this.DataBaseSet.FindAsync(keyObject, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "FindAsync {0}.{1}({2})", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, keyObject.GetType().FullName);
                return null;
            }
        }

        public virtual int? Count(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return -1;
            }

            try
            {
                return filter is null ? this.DataBaseSet?.Count() : this.DataBaseSet?.Count(filter);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Count {0}.{1}", this.GetType().FullName,
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
                return -1;
            }

            try
            {
                return filter is null
                    ? await this.DataBaseSet.CountAsync(cancellationToken).ConfigureAwait(false)
                    : await this.DataBaseSet.CountAsync(filter, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "CountAsync {0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return -1;
            }
        }

        public virtual IQueryable<TEntity>? Query(Expression<Func<TEntity, bool>>? filter = null)
        {
            //Logger.LogTrace("RepositoryBase Query: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName + ")");
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null ? this.DataBaseSet : this.DataBaseSet?.Where(filter);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Query {0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual IQueryable<TEntity>? QueryObjectGraph(Expression<Func<TEntity, bool>> filter, string children)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("Database not exists!");
                return null;
            }

            return this.DataBaseSet?.Include(children).Where(filter);
        }

        public virtual IQueryable<TEntity>? FindAll()
        {
            //Logger.LogTrace("RepositoryBase FindAll: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("{0}.{1} Database not exists!", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }

            try
            {
                return this.DataBaseSet;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual IQueryable<TEntity>? GetPaged<TKeySelector>(
            int pageNumber, int pageSize,
            Expression<Func<TEntity, TKeySelector>>? keySelector = null,
            Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!this.Exists.HasValue || !this.Exists.Value)
            {
                this.Logger.LogWarning("{0}.{1} Database not exists!", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
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

        public void Dispose()
        {
            this.DataContext?.Dispose();
        }
    }
}
