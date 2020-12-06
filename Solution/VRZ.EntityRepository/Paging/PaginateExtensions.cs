using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRZ.EntityRepository.Paging.Filters;

namespace VRZ.EntityRepository.Paging
{
    public static class PaginateExtensions
    {
        public static async Task<PagedList<T>> Paginate<T>(this IQueryable<T> source, IPagingFilter filter)
        {
            var count = await source.CountAsync();

            var items = await source
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedList<T>(items, count, filter.PageNumber, filter.PageSize);
        }
    }
}
