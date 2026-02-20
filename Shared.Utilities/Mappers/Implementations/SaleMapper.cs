using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Shared.Utilities.Mappers.Implementations;

/// <summary>
/// Mapper for Sale entity to SaleDto.
/// </summary>
public class SaleMapper : IMapper<Sale, SaleDto>
{
    public SaleDto Map(Sale source)
    {
        return new SaleDto
        {
            Id = source.Id,
            Date = source.Date,
            Total = source.Total,
            ItemsCount = source.Items.Count,
            Items = source.Items.Select(MapSaleItem).ToList(),
            IsActive = source.IsActive,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    private static SaleItemDto MapSaleItem(SaleItem item)
    {
        return new SaleItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? "Unknown",
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            SubTotal = item.SubTotal
        };
    }
}
