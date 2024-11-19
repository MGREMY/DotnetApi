using DotnetApi.Dto.Pagination;
using DotnetApi.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Extension;

public static class DbSetExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this DbSet<T> set, PaginationRequest pagination)
        where T : class, IBaseEntity
    {
        return set
            .OrderBy(x => x.Id)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }
}