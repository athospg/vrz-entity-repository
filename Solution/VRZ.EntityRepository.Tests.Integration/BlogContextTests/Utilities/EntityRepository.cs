using System.Linq;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities
{
    public class EntityRepository<T> : EntityRepository<long, T, BlogContext>, IEntityRepository<long, T>
        where T : Entity, new()
    {
        public EntityRepository(BlogContext context) : base(context)
        {
        }

        internal BlogContext GetContext()
        {
            return Context;
        }


        public override IQueryable<T> OrderQuery(IQueryable<T> query)
        {
            return query switch
            {
                IQueryable<Post> q => (IQueryable<T>)q.OrderByDescending(x => x.Date),
                IQueryable<Blog> q => (IQueryable<T>)q.OrderBy(x => x.Name),
                IQueryable<Tag> q => (IQueryable<T>)q.OrderBy(x => x.Name),
                _ => query.OrderByDescending(x => x.ModifiedDate),
            };
        }
    }
}
