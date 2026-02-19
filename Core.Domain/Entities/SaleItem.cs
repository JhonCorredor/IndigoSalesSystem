namespace Core.Domain.Entities;

public class SaleItem(Guid id, Guid productId, int quantity, decimal unitPrice)
{
    public Guid Id { get; init; } = id;
    public Guid ProductId { get; init; } = productId;
    public int Quantity { get; init; } = quantity;
    public decimal UnitPrice { get; init; } = unitPrice;

    // Propiedad calculada, no necesita set
    public decimal SubTotal => Quantity * UnitPrice;

    // Propiedad de navegación para EF Core
    public Product? Product { get; init; }
}