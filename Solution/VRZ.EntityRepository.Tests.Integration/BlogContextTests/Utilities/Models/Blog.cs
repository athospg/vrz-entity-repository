using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models.Abstract;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models
{
    public class Blog : Entity, ITagEntity, IEquatable<Blog>
    {
        public string Name { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public bool Equals(Blog other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return base.Equals(other) &&
                   string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                   Equals(Posts, other.Posts) && Equals(Tags, other.Tags);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((Blog)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());
            hashCode.Add(Name, StringComparer.InvariantCultureIgnoreCase);
            hashCode.Add(Posts);
            hashCode.Add(Tags);
            return hashCode.ToHashCode();
        }
    }
}
