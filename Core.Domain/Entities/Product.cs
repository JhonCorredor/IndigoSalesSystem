namespace Core.Domain.Entities;

// Uso de Primary Constructor de C# 12
public class Product(Guid id, string name, decimal price, int stock, string? imageUrl)
{
    public Guid Id { get; init; } = id;
    public string Name { get; private set; } = name;
    public decimal Price { get; private set; } = price;
    public int Stock { get; private set; } = stock;
    public string? ImageUrl { get; private set; } = imageUrl;

    // Método para el Update del CRUD
    public void UpdateDetails(string name, decimal price, string? imageUrl)
    {
        Name = name;
        Price = price;
        ImageUrl = imageUrl;
    }

    // Lógica de negocio encapsulada
    public void RemoveStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        if (Stock < quantity) throw new InvalidOperationException($"Stock insuficiente para el producto {Name}.");

        Stock -= quantity;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");
        Stock += quantity;
    }
}