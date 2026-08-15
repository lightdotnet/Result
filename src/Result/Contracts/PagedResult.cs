using System.Collections.Generic;

namespace Light.Contracts
{
    public class PagedResult<T> : ResultBase, IResult<Paged<T>>
    {
        public PagedResult() { }

        public PagedResult(Paged<T> data, string message = "")
        {
            ResolveDataStatus(data != null, message, "Data is null.", out var status, out var resolvedMessage);
            Status = status;
            Message = resolvedMessage;
            if (data != null)
                Data = data;
        }

        public PagedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
            : this(new Paged<T>(data, pageNumber, pageSize, totalRecords))
        { }

        public static implicit operator Paged<T>(PagedResult<T> result) => result?.Data;

        public Paged<T> Data { get; set; }
    }
}
