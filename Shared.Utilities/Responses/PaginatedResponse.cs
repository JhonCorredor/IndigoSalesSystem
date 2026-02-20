namespace Shared.Utilities.Responses;

/// <summary>
/// Paginated response wrapper for list endpoints.
/// </summary>
/// <typeparam name="T">Type of items in the collection.</typeparam>
public class PaginatedResponse<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static PaginatedResponse<T> Create(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        return new PaginatedResponse<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
