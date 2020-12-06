using System;
using System.Collections.Generic;

namespace VRZ.EntityRepository.Paging
{
    public class PagedList<T> : List<T>
    {
        public int PageSize { get; }

        public int TotalCount { get; }

        public int CurrentPage { get; }

        public int TotalPages { get; }


        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;


        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            PageSize = pageSize;
            TotalCount = count;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }
    }
}
