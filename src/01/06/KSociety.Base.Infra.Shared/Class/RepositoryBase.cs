﻿using KSociety.Base.Infra.Shared.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Class
{
    ///<inheritdoc cref="IRepositoryBase{TEntity}"/>
    public abstract class RepositoryBase<TContext, TEntity> : IRepositoryBase<TEntity>
        where TContext : DatabaseContext
        where TEntity : class
    {
        private TContext? _dataContext;
        protected readonly ILoggerFactory LoggerFactory;

        protected TContext? DataContext => _dataContext ??= DatabaseFactory.Get();

        protected readonly DbSet<TEntity>? DataBaseSet;

        protected readonly ILogger<RepositoryBase<TContext, TEntity>> Logger;

        protected IDatabaseFactory<TContext> DatabaseFactory { get; private set; }

        protected bool? Exists => DataContext?.Exists();

        protected RepositoryBase(ILoggerFactory loggerFactory, IDatabaseFactory<TContext> databaseFactory)
        {
            LoggerFactory = loggerFactory;
            DatabaseFactory = databaseFactory;
            DataBaseSet = DataContext?.Set<TEntity>();

            Logger = LoggerFactory.CreateLogger<RepositoryBase<TContext, TEntity>>();
        }

        public virtual EntityEntry<TEntity>? Add(TEntity entity)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = DataBaseSet?.Add(entity);
            //Logger.LogTrace("RepositoryBase Add: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual async ValueTask<EntityEntry<TEntity>?> AddAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = await DataBaseSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            //Logger.LogTrace("RepositoryBase AddAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
                
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {
                DataBaseSet?.AddRange(entities);
                //Logger.LogTrace("RepositoryBase AddRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual async ValueTask AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {
                await DataBaseSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                //Logger.LogTrace("RepositoryBase AddRangeAsync: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual EntityEntry<TEntity>? Update(TEntity entity)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = DataBaseSet?.Update(entity);
            //Logger.LogTrace("RepositoryBase Update: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {

                DataBaseSet?.UpdateRange(entities);
                //Logger.LogTrace("RepositoryBase UpdateRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual EntityEntry<TEntity>? Delete(TEntity entity)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = DataBaseSet?.Remove(entity);
            //Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void DeleteRange(IEnumerable<TEntity>? entities)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {
                if (entities is not null)
                {
                    DataBaseSet?.RemoveRange(entities);
                }
                
                //Logger.LogTrace("RepositoryBase DeleteRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual IEnumerable<EntityEntry<TEntity>?>? Delete(Expression<Func<TEntity, bool>> where)
        {
            //Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + where.GetType().FullName + ")");
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            IEnumerable<TEntity>? objects = DataBaseSet?.Where(@where).AsEnumerable();
            IEnumerable<EntityEntry<TEntity>?>? output = new List<EntityEntry<TEntity>>();

            if (objects is not null)
            {
                foreach (var obj in objects)
                {
                    output = output?.Append(DataBaseSet?.Remove(obj));
                }
            }

            return output;
        }

        public virtual TEntity? First(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null ? DataBaseSet?.FirstOrDefault() : DataBaseSet?.FirstOrDefault(filter);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual async ValueTask<TEntity?> FirstAsync(Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null
                    ? await DataBaseSet.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
                    : await DataBaseSet.FirstOrDefaultAsync(filter, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual TEntity? Last<TKeySelector>(Expression<Func<TEntity, TKeySelector>> keySelector,
            Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null
                    ? DataBaseSet.OrderBy(keySelector).LastOrDefault()
                    : DataBaseSet.OrderBy(keySelector).LastOrDefault(filter);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual async ValueTask<TEntity?> LastAsync<TKeySelector>(
            Expression<Func<TEntity, TKeySelector>> keySelector, Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null
                    ? await DataBaseSet.OrderBy(keySelector).LastOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false)
                    : await DataBaseSet.OrderBy(keySelector).LastOrDefaultAsync(filter, cancellationToken)
                        .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual TEntity? Find(params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase Find: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return DataBaseSet?.Find(keyObject);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}({2})", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, keyObject.GetType().FullName);
                return null;
            }
        }

        public virtual async ValueTask<TEntity?> FindAsync(CancellationToken cancellationToken = default,
            params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase FindAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return await DataBaseSet.FindAsync(keyObject, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "FindAsync {0}.{1}({2})", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, keyObject.GetType().FullName);
                return null;
            }
        }

        public virtual int? Count(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return -1;
            }

            try
            {
                return filter is null ? DataBaseSet?.Count() : DataBaseSet?.Count(filter);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Count {0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return -1;
            }
        }

        public virtual async ValueTask<int?> CountAsync(Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return -1;
            }

            try
            {
                return filter is null
                    ? await DataBaseSet.CountAsync(cancellationToken).ConfigureAwait(false)
                    : await DataBaseSet.CountAsync(filter, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CountAsync {0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return -1;
            }
        }

        public virtual IQueryable<TEntity>? Query(Expression<Func<TEntity, bool>>? filter = null)
        {
            //Logger.LogTrace("RepositoryBase Query: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName + ")");
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            try
            {
                return filter is null ? DataBaseSet : DataBaseSet?.Where(filter);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Query {0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual IQueryable<TEntity>? QueryObjectGraph(Expression<Func<TEntity, bool>> filter, string children)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            return DataBaseSet?.Include(children).Where(filter);
        }

        public virtual IQueryable<TEntity>? FindAll()
        {
            //Logger.LogTrace("RepositoryBase FindAll: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("{0}.{1} Database not exists!", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }

            try
            {
                return DataBaseSet;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public virtual IQueryable<TEntity>? GetPaged<TKeySelector>(
            int pageNumber, int pageSize,
            Expression<Func<TEntity, TKeySelector>>? keySelector = null,
            Expression<Func<TEntity, bool>>? filter = null)
        {
            if (!Exists.HasValue || !Exists.Value)
            {
                Logger.LogWarning("{0}.{1} Database not exists!", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }

            try
            {
                var skip = (pageNumber - 1) * pageSize;

                if (filter is null)
                {
                    return keySelector is null
                        ? DataBaseSet?.Skip(skip).Take(pageSize)
                        : DataBaseSet?.OrderBy(keySelector).Skip(skip).Take(pageSize);
                }

                return keySelector is null
                    ? DataBaseSet?.Where(filter).Skip(skip).Take(pageSize)
                    : DataBaseSet?.Where(filter).OrderBy(keySelector).Skip(skip).Take(pageSize);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return null;
            }
        }

        public void Dispose()
        {
            DataContext?.Dispose();
        }
    }
}