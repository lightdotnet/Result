using Light.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Light.Extensions
{
    public static class PagedExtensions
    {
        public static Paged<T> ToPaged<T>(
            this IEnumerable<T> list, int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            if (list == null)
                return new Paged<T>(Enumerable.Empty<T>(), pageNumber, pageSize, 0);

            var data = Slice(list, pageNumber, pageSize, out var count);
            return new Paged<T>(data, pageNumber, pageSize, count);
        }

        public static Paged<T> ToPaged<T>(
            this IEnumerable<T> list, IPage page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            return list.ToPaged(page.PageNumber, page.PageSize);
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

            var data = Slice(list, pageNumber, pageSize, out var count);
            return new PagedResult<T>(data, pageNumber, pageSize, count);
        }

        public static PagedResult<T> ToPagedResult<T>(
            this IEnumerable<T> list, IPage page)
        {
            if (page == null)
                throw new ArgumentNullException(nameof(page));
            return list.ToPagedResult(page.PageNumber, page.PageSize);
        }

        private static List<T> Slice<T>(IEnumerable<T> list, int pageNumber, int pageSize, out int count)
        {
            if (!(list is IList<T> materialized))
                materialized = list.ToList();

            count = materialized.Count;
            var skip = (long)(pageNumber - 1) * pageSize;
            return materialized.Skip(skip > count ? count : (int)skip).Take(pageSize).ToList();
        }
    }
}
