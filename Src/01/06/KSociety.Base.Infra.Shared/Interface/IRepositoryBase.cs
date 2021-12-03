using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Interface
{
    /// <include file='..\Doc\RepositoryBase.xml' path='docs/members[@name="RepositoryBase"]/RepositoryBase/*'/>
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class
    {
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Add(TEntity entity);

        ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        void AddRange(IEnumerable<TEntity> entities);

        ValueTask AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>> Delete(Expression<Func<TEntity, bool>> where);

        TEntity Find(params object[] keyObjects);

        ValueTask<TEntity> FindAsync(CancellationToken cancellationToken = default, params object[] keyObjects);

        /// <include file='..\Doc\RepositoryBase.xml' path='docs/members[@name="RepositoryBase"]/Count/*'/>
        int Count(Expression<Func<TEntity, bool>> filter);

        /// <include file='..\Doc\RepositoryBase.xml' path='docs/members[@name="RepositoryBase"]/CountAsync/*'/>
        ValueTask<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter);

        IQueryable<TEntity> QueryObjectGraph(Expression<Func<TEntity, bool>> filter, string children);

        IQueryable<TEntity> FindAll();
    }
}
