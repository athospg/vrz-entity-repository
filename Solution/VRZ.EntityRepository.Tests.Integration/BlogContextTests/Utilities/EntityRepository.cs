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
    }
}
