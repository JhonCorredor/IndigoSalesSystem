using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Shared.Utilities.Mappers.Implementations;

/// <summary>
/// Mapper for Product entity to ProductDto.
/// </summary>
public class ProductMapper : IMapper<Product, ProductDto>
{
    public ProductDto Map(Product source)
    {
        return new ProductDto
        {
            Id = source.Id,
            Name = source.Name,
            Price = source.Price,
            Stock = source.Stock,
            ImageUrl = source.ImageUrl,
            IsActive = source.IsActive,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
