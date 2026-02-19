namespace Core.Domain.Entities;

public class Sale(Guid id, DateTime date)
{
    public Guid Id { get; init; } = id;
    public DateTime Date { get; init; } = date;
    public decimal Total { get; private set; }

    // Uso de Collection Expressions (C# 12) para inicializar la lista: []
    private readonly List<SaleItem> _items = [];

    // Exponemos la lista como solo lectura para proteger el estado
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        var item = new SaleItem(Guid.NewGuid(), productId, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        Total = _items.Sum(i => i.SubTotal);
    }
}