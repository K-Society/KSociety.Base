using KSociety.Base.Infra.Shared.Interface;
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
    public abstract class RepositoryBase<TContext, TEntity> : IRepositoryBase<TEntity>
        where TContext : DatabaseContext
        where TEntity : class
    {
        private TContext _dataContext;
        protected readonly ILoggerFactory LoggerFactory;

        protected TContext DataContext => _dataContext ??= DatabaseFactory.Get();

        protected readonly DbSet<TEntity> DataBaseSet;

        protected readonly ILogger<RepositoryBase<TContext, TEntity>> Logger;

        protected IDatabaseFactory<TContext> DatabaseFactory { get; private set; }

        protected bool Exists => DataContext.Exists();

        protected RepositoryBase(ILoggerFactory loggerFactory, IDatabaseFactory<TContext> databaseFactory)
        {
            LoggerFactory = loggerFactory;
            DatabaseFactory = databaseFactory;
            DataBaseSet = DataContext.Set<TEntity>();
            
            Logger = LoggerFactory.CreateLogger<RepositoryBase<TContext, TEntity>>();
        }

        public virtual EntityEntry<TEntity> Add(TEntity entity)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            var result = DataBaseSet.Add(entity);
            //Logger.LogTrace("RepositoryBase Add: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual async ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (!Exists)
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
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {

                DataBaseSet.AddRange(entities);
                //Logger.LogTrace("RepositoryBase AddRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual async ValueTask AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (!Exists)
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

        public virtual EntityEntry<TEntity> Update(TEntity entity)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }
            var result = DataBaseSet.Update(entity);
            //Logger.LogTrace("RepositoryBase Update: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {

                DataBaseSet.UpdateRange(entities); 
                //Logger.LogTrace("RepositoryBase UpdateRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual EntityEntry<TEntity> Delete(TEntity entity)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }
            var result = DataBaseSet.Remove(entity);
            //Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + entity.GetType().FullName + ")" + " State: " + result.State);
            return result;
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
            }
            else
            {

                DataBaseSet.RemoveRange(entities);
                //Logger.LogTrace("RepositoryBase DeleteRange: " + GetType().FullName + "." +
                //                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public virtual IEnumerable<EntityEntry<TEntity>> Delete(Expression<Func<TEntity, bool>> where)
        {
            //Logger.LogTrace("RepositoryBase Delete: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + where.GetType().FullName + ")");
            if (!Exists)
            {
                Logger.LogWarning("Database not exists!");
                return null;
            }

            IEnumerable<TEntity> objects = DataBaseSet.Where(@where).AsEnumerable();
            IEnumerable<EntityEntry<TEntity>> output = new List<EntityEntry<TEntity>>();
            foreach (var obj in objects)
            {
                output.Append(DataBaseSet.Remove(obj));
            }

            return output;

        }

        public virtual TEntity Find(params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase Find: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (Exists)
            {
                try
                {
                    return DataBaseSet.Find(keyObject);
                }
                catch (Exception ex)
                {
                    Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ") " + ex.Message + " - " + ex.StackTrace);
                    return null;
                }
            }
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public virtual async ValueTask<TEntity> FindAsync(CancellationToken cancellationToken = default, params object[] keyObject)
        {
            //Logger.LogTrace("RepositoryBase FindAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ")");
            if (Exists)
            {
                try
                {
                    return await DataBaseSet.FindAsync(keyObject, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + keyObject.GetType().FullName + ") " + ex.Message + " - " + ex.StackTrace);
                    return null;
                }
            }
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter)
        {
            //Logger.LogTrace("RepositoryBase Query: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName + ")");
            if (Exists)
            {
                try
                {
                    return DataBaseSet.Where(filter);
                }
                catch (Exception ex)
                {
                    Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " " + ex.Message + " - " + ex.StackTrace);
                    return null;
                }
            }
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public virtual IQueryable<TEntity> QueryObjectGraph(Expression<Func<TEntity, bool>> filter, string children)
        {
            //Logger.LogTrace("RepositoryBase QueryObjectGraph: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + "(" + filter.GetType().FullName + "," + children + ")");
            if (Exists) return DataBaseSet.Include(children).Where(filter);
            Logger.LogWarning("Database not exists!");
            return null;

        }

        public virtual IQueryable<TEntity> FindAll()
        {
            //Logger.LogTrace("RepositoryBase FindAll: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            if (Exists)
            {
                try
                {
                    return DataBaseSet;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                    return null;
                }
            }
            Logger.LogWarning("{0}.{1} Database not exists!", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            return null;
        }

        //public void Test(Expression<Func<TEntity, TProperty>> navigationPropertyPath)
        //{
        //    DataBaseSet.Include(navigationPropertyPath);
        //}

        public void Dispose()
        {
            DataContext.Dispose();
        }
    }
}
