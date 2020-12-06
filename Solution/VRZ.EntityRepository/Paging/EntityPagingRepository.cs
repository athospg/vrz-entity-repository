using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRZ.EntityRepository.Paging.Filters;

namespace VRZ.EntityRepository.Paging
{
    public class EntityPagingRepository<TKey, TEntity, TContext> :
        EntityRepository<TKey, TEntity, TContext>,
        IEntityPagingRepository<TKey, TEntity>
        where TKey : IComparable<TKey>
        where TEntity : class, new()
        where TContext : DbContext
    {
        public EntityPagingRepository(TContext context) : base(context)
        {
        }


        public virtual async Task<PagedList<TEntity>> FindAll(IPagingFilter filter, bool asNoTracking = true)
        {
            var query = GetFindAllQueryable(asNoTracking);

            return await query.Paginate(filter);
        }

        public virtual async Task<PagedList<TEntity>> FindAll(IPagingFilter filter,
            Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true)
        {
            var query = GetFindAllQueryable(predicate, asNoTracking);

            return await query.Paginate(filter);
        }

        public virtual async Task<PagedList<TEntity>> FindAllIncluding(IPagingFilter filter, bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetFindAllIncludingQueryable(asNoTracking, includeProperties);

            return await query.Paginate(filter);
        }

        public virtual async Task<PagedList<TEntity>> FindAllIncluding(IPagingFilter filter,
            Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetFindAllIncludingQueryable(predicate, asNoTracking, includeProperties);

            return await query.Paginate(filter);
        }
    }
}
