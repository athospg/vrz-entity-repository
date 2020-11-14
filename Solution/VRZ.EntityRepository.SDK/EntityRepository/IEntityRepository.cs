using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VRZ.EntityRepository.SDK.EntityRepository
{
    public interface IEntityRepository<TKey, TEntity>
        where TKey : IComparable<TKey>
        where TEntity : class, new()
    {
        #region Read Methods

        ValueTask<bool> Any(Expression<Func<TEntity, bool>> predicate);
        ValueTask<long> CountAll();
        ValueTask<long> CountWhere(Expression<Func<TEntity, bool>> predicate);

        ValueTask<TEntity> Find([DisallowNull] TKey key);
        ValueTask<TEntity> FindIncluding([DisallowNull] TKey key, bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includeProperties);
        ValueTask<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true);

        Task<IEnumerable<TEntity>> FindAll(bool asNoTracking = true);
        Task<IEnumerable<TEntity>> FindAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true);
        Task<IEnumerable<TEntity>> FindAllIncluding(bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TEntity>> FindAllIncluding(Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includeProperties);

        #endregion

        #region Write Methods

        Task<TEntity> Add([DisallowNull] TEntity entity);
        Task<IEnumerable<TEntity>> Add(IEnumerable<TEntity> entities);
        Task<TEntity> Update([DisallowNull] TEntity entity);
        Task<IEnumerable<TEntity>> Update(IEnumerable<TEntity> entity);
        Task<TEntity> Remove(TEntity entity);
        Task<TEntity> Remove([DisallowNull] TKey key);

        #endregion

        #region Utilities

        string GetPrimaryKeyNameAndType(out Type primaryKeyType);

        TKey GetPrimaryKeyValue([DisallowNull] TEntity entity);

        Expression<Func<TEntity, bool>> GetKeyEqualsExpression([DisallowNull] TKey key);

        #endregion
    }
}
