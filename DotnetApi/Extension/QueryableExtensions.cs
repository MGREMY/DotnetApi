using DotnetApi.Dto.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Extension;

public static class QueryableExtensions
{
    public static PagedResponse<T> ToPagedResponse<T>(this IQueryable<T> query, PaginationRequest pagination,
        Func<int> totalCountAction)
    {
        var totalPages = (int)Math.Ceiling((double)totalCountAction() / pagination.PageSize);

        return new PagedResponse<T>
        {
            PageSize = pagination.PageSize,
            PageNumber = pagination.PageNumber,
            HasNextPage = pagination.PageNumber < totalPages,
            HasPreviousPage = pagination.PageNumber > 1,
            TotalPages = totalPages,
            Data = query.ToArray(),
        };
    }

    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> query,
        PaginationRequest pagination, Func<CancellationToken, Task<int>> totalCountFunc,
        CancellationToken cancellationToken)
    {
        var totalPages = (int)Math.Ceiling((double)await totalCountFunc(cancellationToken) / pagination.PageSize);

        return new PagedResponse<T>
        {
            PageSize = pagination.PageSize,
            PageNumber = pagination.PageNumber,
            HasNextPage = pagination.PageNumber < totalPages,
            HasPreviousPage = pagination.PageNumber > 1,
            TotalPages = totalPages,
            Data = await query.ToArrayAsync(cancellationToken),
        };
    }
}