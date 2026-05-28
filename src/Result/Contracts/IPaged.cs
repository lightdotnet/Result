using System.Collections.Generic;

namespace Light.Contracts
{
    public interface IPaged : IPage
    {
        int TotalPages { get; }
        int TotalRecords { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }

    public interface IPaged<T> : IPaged
    {
        IEnumerable<T> Records { get; }
    }
}
