using VRZ.EntityRepository.Paging;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities
{
    public class EntityPagingRepository<T> : EntityPagingRepository<long, T, BlogContext>, IEntityPagingRepository<long, T>
        where T : Entity, new()
    {
        public EntityPagingRepository(BlogContext context) : base(context)
        {
        }

        internal BlogContext GetContext()
        {
            return Context;
        }
    }
}
