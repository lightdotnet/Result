using System;
using System.Collections.Generic;
using System.Linq;

namespace Light.Contracts
{
    public class Paged : IPaged
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public int TotalPages
        {
            get
            {
                return PageSize > 0
                    ? (int)Math.Ceiling(TotalRecords / (double)PageSize)
                    : 0;
            }
        }

        public bool HasPreviousPage { get { return PageNumber > 1; } }
        public bool HasNextPage { get { return PageNumber < TotalPages; } }
    }

    public class Paged<T> : Paged, IPaged<T>
    {
        public Paged() { }

        public Paged(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            Records = data ?? Enumerable.Empty<T>();
        }

        public IEnumerable<T> Records { get; set; } = Enumerable.Empty<T>();
    }
}
