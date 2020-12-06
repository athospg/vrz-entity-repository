using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VRZ.EntityRepository.SDK.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        ///     Orders the query
        ///     <param name="query" />
        ///     with all properties in
        ///     <param name="orderBy" />
        ///     separated with commas (,).
        ///     All properties may include an optional 'asc', 'ascending', 'desc' or 'descending' after
        ///     the property and separated by a single space or the default order
        ///     <param name="defaultDirection" />
        ///     is applied.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <param name="defaultDirection"></param>
        /// <returns>IOrderedQueryable that can be ordered again with 'ThenBy(Descending)'</returns>
        public static IOrderedQueryable<TEntity> Order<TEntity>(this IQueryable<TEntity> query, string orderBy,
            ListSortDirection defaultDirection = ListSortDirection.Ascending)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return (IOrderedQueryable<TEntity>)query;

            var hasOrdered = false;

            var properties = orderBy.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
            foreach (var propertyOrder in properties)
            {
                var spt = propertyOrder
                    .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToArray();

                if (spt.Length == 0)
                    continue;

                var prop = spt[0].Trim();

                var sortDirection = defaultDirection;
                if (spt.Length > 1)
                {
                    var sort = spt[1].Trim();
                    if (sort == "asc" || sort == "ascending")
                        sortDirection = ListSortDirection.Ascending;

                    if (sort == "desc" || sort == "descending")
                        sortDirection = ListSortDirection.Descending;
                }

                var property = typeof(TEntity).GetProperty(prop);
                if (property == null)
                    continue;

                var method = !hasOrdered
                    ? typeof(QueryableExtensions)
                        .GetMethod(sortDirection == ListSortDirection.Ascending
                            ? nameof(OrderByProperty)
                            : nameof(OrderByPropertyDescending))?
                        .MakeGenericMethod(typeof(TEntity), property.PropertyType)
                    : typeof(QueryableExtensions)
                        .GetMethod(sortDirection == ListSortDirection.Ascending
                            ? nameof(OrderThenBy)
                            : nameof(OrderThenByDescending))?
                        .MakeGenericMethod(typeof(TEntity), property.PropertyType);

                hasOrdered = true;

                query = (IOrderedQueryable<TEntity>)method?.Invoke(null, new object[] { query, property });
            }

            return (IOrderedQueryable<TEntity>)query;
        }


        public static IOrderedQueryable<TEntity> OrderByProperty<TEntity, TRet>(IQueryable<TEntity> q, PropertyInfo p)
        {
            var pe = Expression.Parameter(typeof(TEntity));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));
            return q.OrderBy(Expression.Lambda<Func<TEntity, TRet>>(se, pe));
        }

        public static IOrderedQueryable<TEntity> OrderByPropertyDescending<TEntity, TRet>(IQueryable<TEntity> q,
            PropertyInfo p)
        {
            var pe = Expression.Parameter(typeof(TEntity));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));
            return q.OrderByDescending(Expression.Lambda<Func<TEntity, TRet>>(se, pe));
        }

        public static IOrderedQueryable<TEntity> OrderThenBy<TEntity, TRet>(IOrderedQueryable<TEntity> q,
            PropertyInfo p)
        {
            var pe = Expression.Parameter(typeof(TEntity));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));

            return q.ThenBy(Expression.Lambda<Func<TEntity, TRet>>(se, pe));
        }

        public static IOrderedQueryable<TEntity> OrderThenByDescending<TEntity, TRet>(IOrderedQueryable<TEntity> q,
            PropertyInfo p)
        {
            var pe = Expression.Parameter(typeof(TEntity));
            Expression se = Expression.Convert(Expression.Property(pe, p), typeof(TRet));

            return q.ThenByDescending(Expression.Lambda<Func<TEntity, TRet>>(se, pe));
        }
    }
}
