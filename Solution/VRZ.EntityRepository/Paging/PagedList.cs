using System;
using System.Collections.Generic;
using VRZ.EntityRepository.Paging.Result;

namespace VRZ.EntityRepository.Paging
{
    public class PagedList<T> : List<T>, IPagedResult
    {
        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }


        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;


        public PagedList()
        {
        }

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
