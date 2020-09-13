using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models
{
    public class Tag : Entity
    {
        public string Name { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
