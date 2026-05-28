using System;
using System.Collections.Generic;

namespace Light.Contracts
{
    public class PagedResult<T> : ResultBase, IResult<Paged<T>>
    {
        public PagedResult() { }

        public PagedResult(Paged<T> data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Status = ResultCode.Success;
        }

        public PagedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
            : this(new Paged<T>(data, pageNumber, pageSize, totalRecords))
        { }

        public static implicit operator Paged<T>(PagedResult<T> result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result),
                    "Cannot convert null PagedResult<T> to Paged<T>.");
            if (result.Data == null)
                throw new InvalidOperationException(
                    "Cannot convert PagedResult<T> to Paged<T>: Data is null. "
                    + "Code: " + result.Code);
            return result.Data;
        }

        public Paged<T> Data { get; set; }
    }
}
