using Core.Domain.Common;

namespace Core.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string? ImageUrl { get; private set; }

    private Product() { }

    public Product(Guid id, string name, decimal price, int stock, string? imageUrl) : base(id)
    {
        Name = name;
        Price = price;
        Stock = stock;
        ImageUrl = imageUrl;
    }

    public void UpdateDetails(string name, decimal price, int stock, string? imageUrl)
    {
        Name = name;
        Price = price;
        Stock = stock;
        ImageUrl = imageUrl;
        MarkAsUpdated();
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        if (Stock < quantity) throw new InvalidOperationException($"Stock insuficiente para el producto {Name}.");

        Stock -= quantity;
        MarkAsUpdated();
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        Stock += quantity;
        MarkAsUpdated();
    }
}