namespace DotnetApi.Dto.Pagination;

public record PagedResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Data { get; set; } = [];
}