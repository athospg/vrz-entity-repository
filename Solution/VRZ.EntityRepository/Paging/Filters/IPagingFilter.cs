using System.Text.Json.Serialization;

namespace VRZ.EntityRepository.Paging.Filters
{
    public interface IPagingFilter
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        [JsonIgnore]
        public int? MaxPageSize { get; set; }


        public string OrderBy { get; set; }

        public bool Ascending { get; set; }
    }
}
