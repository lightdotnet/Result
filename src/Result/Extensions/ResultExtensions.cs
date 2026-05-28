using Light.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Light.Extensions
{
    public static class ResultExtensions
    {
        public static bool IsFailed(this IResult result)
        {
            return !result.IsSuccess;
        }

        public static PagedResult<T> ToPagedResult<T>(
            this IEnumerable<T> list, int pageNumber = 1, int pageSize = 10)
        {
            if (list == null)
            {
                return new PagedResult<T>
                {
                    Status = ResultCode.Error,
                    Message = "The list is null.",
                };
            }

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var materialized = list as IList<T>;
            if (materialized == null)
                materialized = list.ToList();

            var count = materialized.Count;
            var data = materialized
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>(data, pageNumber, pageSize, count);
        }

        public static PagedResult<T> ToPagedResult<T>(
            this IEnumerable<T> list, IPage page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            return list.ToPagedResult(page.PageNumber, page.PageSize);
        }

        public static Paged<T> ToPaged<T>(
            this IEnumerable<T> list, int pageNumber = 1, int pageSize = 10)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var materialized = list as IList<T>;
            if (materialized == null)
                materialized = list.ToList();

            var count = materialized.Count;
            var data = materialized
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new Paged<T>(data, pageNumber, pageSize, count);
        }

        public static Paged<T> ToPaged<T>(
            this IEnumerable<T> list, IPage page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            return list.ToPaged(page.PageNumber, page.PageSize);
        }
    }
}
