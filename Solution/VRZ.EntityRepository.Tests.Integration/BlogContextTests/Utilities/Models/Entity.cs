using System;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models
{
    public class Entity : IEquatable<Entity>
    {
        private DateTimeOffset _createdDate;
        private DateTimeOffset _modifiedDate;

        public long Id { get; set; }

        public DateTimeOffset CreatedDate
        {
            get => _createdDate;
            set => _createdDate = value.ToUniversalTime();
        }

        public DateTimeOffset ModifiedDate
        {
            get => _modifiedDate;
            set => _modifiedDate = value.ToUniversalTime();
        }

        public bool Equals(Entity other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return _createdDate.Equals(other._createdDate) &&
                   _modifiedDate.Equals(other._modifiedDate) &&
                   Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_createdDate, _modifiedDate, Id);
        }
    }
}
