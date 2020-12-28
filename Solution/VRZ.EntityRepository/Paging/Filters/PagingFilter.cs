using System.Text.Json.Serialization;

namespace VRZ.EntityRepository.Paging.Filters
{
    public class PagingFilter : IPagingFilter
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private int? _maxPageSize;


        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }

        public virtual int PageSize
        {
            get => _pageSize;
            set => _pageSize = MaxPageSize.HasValue && value > MaxPageSize
                ? MaxPageSize.Value
                : value > 0
                    ? value
                    : _pageSize;
        }


        [JsonIgnore]
        public virtual int? MaxPageSize
        {
            get => _maxPageSize;
            set
            {
                _maxPageSize = value.HasValue && value > 0 ? value : _maxPageSize;
                PageSize = _pageSize;
            }
        }


        public string OrderBy { get; set; }

        public bool Ascending { get; set; } = true;
    }
}
