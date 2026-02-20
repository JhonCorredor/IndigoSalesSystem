namespace Core.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }

    public decimal SubTotal => Quantity * UnitPrice;

    public Product? Product { get; private set; }

    private SaleItem() { }

    public SaleItem(Guid id, Guid productId, int quantity, decimal unitPrice)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}