using System.Collections.Generic;

namespace Light.Contracts
{
    public class PagedResult<T> : ResultBase, IResult<Paged<T>>
    {
        public PagedResult() { }

        public PagedResult(Paged<T> data, string message = "")
        {
            if (data == null)
            {
                Status = ResultCode.Error;
                Message = string.IsNullOrEmpty(message) ? "Data is null." : message;
            }
            else
            {
                Status = ResultCode.Success;
                Message = message ?? "";
                Data = data;
            }
        }

        public PagedResult(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
            : this(new Paged<T>(data, pageNumber, pageSize, totalRecords))
        { }

        public static implicit operator Paged<T>(PagedResult<T> result) => result?.Data;

        public Paged<T> Data { get; set; }
    }
}
