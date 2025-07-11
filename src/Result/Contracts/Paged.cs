using System;
using System.Collections.Generic;

namespace Light.Contracts
{
    public class Paged : IPaged
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;
    }

    public class Paged<T> : Paged, IPaged<T>
    {
        public Paged() { }

        public Paged(IEnumerable<T> data, int page, int pageSize, int count)
        {
            Page = page;
            PageSize = pageSize;
            TotalRecords = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Records = data;
        }

        public IEnumerable<T> Records { get; set; }
    }
}