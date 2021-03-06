﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models
{
    public class Tag : Entity, IEquatable<Tag>
    {
        public string Name { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        public bool Equals(Tag other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return base.Equals(other) &&
                   string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((Tag)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());
            hashCode.Add(Name, StringComparer.InvariantCultureIgnoreCase);
            return hashCode.ToHashCode();
        }
    }
}
