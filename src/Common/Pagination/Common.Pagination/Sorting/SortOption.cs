using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Pagination.Sorting
{
    public sealed record SortOption<TField>(TField Field, SortDirection Direction = SortDirection.Ascending) where TField : struct, Enum;
}
