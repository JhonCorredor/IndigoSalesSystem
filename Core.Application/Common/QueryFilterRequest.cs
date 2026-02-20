namespace Core.Application.Common;

/// <summary>
/// Request object for filtering, sorting and pagination.
/// </summary>
public record QueryFilterRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public bool IncludeInactive { get; init; }
}
