﻿namespace VRZ.EntityRepository.Paging.Result
{
    public class PagedResult : IPagedResult
    {
        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }


        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;
    }
}
