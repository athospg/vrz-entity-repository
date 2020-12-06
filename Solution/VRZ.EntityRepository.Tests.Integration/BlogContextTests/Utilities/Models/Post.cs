using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models.Abstract;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models
{
    public class Post : Entity, ITagEntity, IEquatable<Post>
    {
        public DateTimeOffset Date { get; set; }

        public string Name { get; set; }

        public long BlogId { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Blog Blog { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public bool Equals(Post other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return base.Equals(other) &&
                   Date.Equals(other.Date) &&
                   string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                   BlogId == other.BlogId &&
                   Equals(Tags, other.Tags);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((Post)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());
            hashCode.Add(Date);
            hashCode.Add(Name, StringComparer.InvariantCultureIgnoreCase);
            hashCode.Add(BlogId);
            hashCode.Add(Tags);
            return hashCode.ToHashCode();
        }
    }
}
