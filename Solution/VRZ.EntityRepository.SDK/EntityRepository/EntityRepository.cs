using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VRZ.EntityRepository.SDK.EntityRepository
{
    public class EntityRepository<TKey, TEntity, TContext> : IEntityRepository<TKey, TEntity>
        where TKey : IComparable<TKey>
        where TEntity : class, new()
        where TContext : DbContext
    {
        protected readonly TContext Context;

        public EntityRepository(TContext context)
        {
            Context = context;
        }

        #region Read Methods

        public async ValueTask<long> CountAll()
        {
            return await Context.Set<TEntity>()
                .AsNoTracking()
                .LongCountAsync();
        }

        public async ValueTask<long> CountWhere(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>()
                .AsNoTracking()
                .LongCountAsync(predicate);
        }

        public async ValueTask<bool> Any(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>()
                .AsNoTracking()
                .AnyAsync(predicate);
        }

        public async ValueTask<TEntity> Find(TKey key)
        {
            return key is Array keys
                ? await Context.Set<TEntity>().FindAsync(keys.Cast<object>().ToArray())
                : await Context.Set<TEntity>().FindAsync(key);
        }

        public async ValueTask<TEntity> FindIncluding(TKey key, bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var expressionTree = GetKeyEqualsExpression(key);

            var query = asNoTracking
                ? Context.Set<TEntity>().AsNoTracking()
                : Context.Set<TEntity>();

            query = includeProperties.Aggregate(query,
                (current, property) => current.Include(property));

            return await query.SingleOrDefaultAsync(expressionTree);
        }

        public async Task<IEnumerable<TEntity>> FindAll(bool asNoTracking = true)
        {
            var query = asNoTracking
                ? Context.Set<TEntity>().AsNoTracking()
                : Context.Set<TEntity>();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAll(Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true)
        {
            var query = asNoTracking
                ? Context.Set<TEntity>().AsNoTracking()
                : Context.Set<TEntity>();

            return await query
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAllIncluding(bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = asNoTracking
                ? Context.Set<TEntity>().AsNoTracking()
                : Context.Set<TEntity>();

            query = includeProperties.Aggregate(query,
                (current, property) => current.Include(property));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAllIncluding(Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = asNoTracking
                ? Context.Set<TEntity>().AsNoTracking()
                : Context.Set<TEntity>();

            query = query.Where(predicate);

            query = includeProperties.Aggregate(query,
                (current, property) => current.Include(property));

            return await query.ToListAsync();
        }

        public async ValueTask<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true)
        {
            var query = asNoTracking
                ? Context.Set<TEntity>().AsNoTracking()
                : Context.Set<TEntity>();

            return await query.FirstOrDefaultAsync(predicate);
        }

        #endregion

        #region Write Methods

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            MapChildrenEntities(entity);

            await Context.Set<TEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> Add(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                MapChildrenEntities(entity);
            }

            await Context.Set<TEntity>().AddRangeAsync(entities);
            await Context.SaveChangesAsync();

            return entities;
        }

        public virtual async Task<int> Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<int> Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }

            return await Context.SaveChangesAsync();
        }

        public virtual Task<int> Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            return Context.SaveChangesAsync();
        }

        public virtual async Task<int> Remove(TKey key)
        {
            var entity = await Context.Set<TEntity>().FindAsync(key);
            Context.Set<TEntity>().Remove(entity);
            return await Context.SaveChangesAsync();
        }

        #endregion

        #region Utilities

        public Expression<Func<TEntity, bool>> GetKeyEqualsExpression(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var entityType = Context.Model.FindEntityType(typeof(TEntity));

            var primaryKeyName = entityType.FindPrimaryKey().Properties.Select(p => p.Name).FirstOrDefault();
            var primaryKeyType = entityType.FindPrimaryKey().Properties.Select(p => p.ClrType).FirstOrDefault();

            if (primaryKeyName == null || primaryKeyType == null)
                throw new ArgumentException("Entity does not have any primary key defined", typeof(TEntity).Name);

            object primaryKeyValue;
            try
            {
                primaryKeyValue = Convert.ChangeType(key, primaryKeyType, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Cannot assign type {key.GetType()} to type {primaryKeyType}",
                    typeof(TEntity).Name);
            }

            var pe = Expression.Parameter(typeof(TEntity), "entity");
            var me = Expression.Property(pe, primaryKeyName);
            var constant = Expression.Constant(primaryKeyValue, primaryKeyType);
            var body = Expression.Equal(me, constant);
            var expressionTree = Expression.Lambda<Func<TEntity, bool>>(body, pe);

            return expressionTree;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<PropertyInfo> GetProperties => typeof(TEntity).GetProperties();

        private static IEnumerable<PropertyInfo> GetCollectionsInfo()
        {
            return GetProperties
                .Where(x => x.PropertyType != typeof(string) &&
                            x.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) &&
                            x.GetCustomAttribute(typeof(NotMappedAttribute), false) is not NotMappedAttribute);
        }

        private static void MapChildrenEntities(TEntity entity)
        {
            //TODO
        }

        #endregion
    }
}
