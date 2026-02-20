namespace Core.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }

    // Propiedad calculada, no necesita set
    public decimal SubTotal => Quantity * UnitPrice;

    // Propiedad de navegación para EF Core (private set permite que EF Core la asigne)
    public Product? Product { get; private set; }

    // Constructor privado para EF Core
    private SaleItem() { }

    public SaleItem(Guid id, Guid productId, int quantity, decimal unitPrice)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}