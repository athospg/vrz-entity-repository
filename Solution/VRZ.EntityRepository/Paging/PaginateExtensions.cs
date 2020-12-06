using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRZ.EntityRepository.Paging.Filters;
using VRZ.EntityRepository.SDK.Extensions;

namespace VRZ.EntityRepository.Paging
{
    public static class PaginateExtensions
    {
        public static async Task<PagedList<T>> Paginate<T>(this IQueryable<T> query, IPagingFilter filter)
        {
            var count = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(filter.OrderBy))
            {
                var direction = filter.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;
                query = query.Order(filter.OrderBy, direction);
            }

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedList<T>(items, count, filter.PageNumber, filter.PageSize);
        }
    }
}
