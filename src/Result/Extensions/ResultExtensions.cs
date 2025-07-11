using Light.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Light.Extensions
{
    public static class ResultExtensions
    {
        public static ResultCode MapResultCode(this IResult result)
        {
            Enum.TryParse(result.Code, out ResultCode resultCode);
            return resultCode;
        }

        public static bool IsFailed(this IResult result) => !result.Succeeded;

        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> list, int page = 1, int pageSize = 10)
        {
            if (list == null)
            {
                return new PagedResult<T>
                {
                    Code = ResultCode.error.ToString(),
                    Message = "The list is null.",
                };
            }

            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;
            var count = list.Count();
            var data = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<T>(data, page, pageSize, count);
        }

        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> list, IPage page) =>
            list.ToPagedResult(page.Page, page.PageSize);

        public static Paged<T> ToPaged<T>(this IEnumerable<T> list, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;
            var count = list.Count();
            var data = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new Paged<T>(data, page, pageSize, count);
        }

        public static Paged<T> ToPaged<T>(this IEnumerable<T> list, IPage page)
        {
            return list.ToPaged(page.Page, page.PageSize);
        }
    }
}