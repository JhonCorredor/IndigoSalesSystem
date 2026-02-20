using Core.Application.Common;

namespace Core.Application.DTOs;

/// <summary>
/// DTO for Sale entity list operations.
/// </summary>
public record SaleDto : BaseDto
{
    public DateTime Date { get; init; }
    public decimal Total { get; init; }
    public int ItemsCount { get; init; }
    public IReadOnlyList<SaleItemDto> Items { get; init; } = [];
}

/// <summary>
/// DTO for SaleItem entity.
/// </summary>
public record SaleItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal SubTotal { get; init; }
}

/// <summary>
/// Request DTO for creating a sale.
/// </summary>
public record SaleRequestDto(List<SaleItemRequestDto> Items);

/// <summary>
/// Request DTO for sale items.
/// </summary>
public record SaleItemRequestDto(Guid ProductId, int Quantity);