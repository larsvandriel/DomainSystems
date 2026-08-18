using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Pagination.Pagination
{
    public sealed record PageResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, long TotalCount)
    {
        public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}
