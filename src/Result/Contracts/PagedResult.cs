using System.Collections.Generic;

namespace Light.Contracts
{
    public class PagedResult<T> : ResultBase, IResult<Paged<T>>
    {
        public PagedResult() { }

        public PagedResult(Paged<T> data)
        {
            if (data == null)
            {
                Status = ResultCode.Error;
                Message = "Data is null.";
            }
            else
            {
                Status = ResultCode.Success;
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
