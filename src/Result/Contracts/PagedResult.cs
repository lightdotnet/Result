using System.Collections.Generic;

namespace Light.Contracts
{
    public class PagedResult<T> : ResultBase, IResult<Paged<T>>
    {
        public PagedResult() { }

        public PagedResult(Paged<T> data)
        {
            Code = ResultCode.success.ToString();
            Succeeded = true;
            Data = data;
        }

        public PagedResult(IEnumerable<T> data, int page, int pageSize, int count) : this(new Paged<T>(data, page, pageSize, count))
        { }

        public Paged<T> Data { get; set; }
    }
}
