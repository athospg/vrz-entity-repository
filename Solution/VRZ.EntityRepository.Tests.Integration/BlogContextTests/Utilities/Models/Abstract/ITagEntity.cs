using System.Collections.Generic;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models.Abstract
{
    public interface ITagEntity
    {
        public ICollection<Tag> Tags { get; set; }
    }
}
