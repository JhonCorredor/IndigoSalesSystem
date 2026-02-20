using Core.Application.Common;

namespace Core.Application.DTOs;

/// <summary>
/// DTO for Product entity list operations.
/// </summary>
public record ProductDto : BaseDto
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string? ImageUrl { get; init; }
}

/// <summary>
/// Request DTO for creating/updating a product.
/// </summary>
public record ProductRequestDto(
    string Name,
    decimal Price,
    int Stock,
    string? ImageUrl
);
