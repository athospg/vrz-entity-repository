using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VRZ.EntityRepository.Paging.Filters;

namespace VRZ.EntityRepository.Paging
{
    public interface IEntityPagingRepository<TKey, TEntity> : IEntityRepository<TKey, TEntity>
        where TKey : IComparable<TKey>
        where TEntity : class, new()
    {
        public Task<PagedList<TEntity>> FindAll(IPagingFilter filter, bool asNoTracking = true);

        public Task<PagedList<TEntity>> FindAll(IPagingFilter filter, Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true);

        public Task<PagedList<TEntity>> FindAllIncluding(IPagingFilter filter, bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includeProperties);

        public Task<PagedList<TEntity>> FindAllIncluding(IPagingFilter filter, Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
