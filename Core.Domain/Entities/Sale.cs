using Core.Domain.Common;

namespace Core.Domain.Entities;

public class Sale : BaseEntity
{
    public DateTime Date { get; init; }
    public decimal Total { get; private set; }

    private readonly List<SaleItem> _items = [];
    public IReadOnlyCollection<SaleItem> Items => _items;

    private Sale() { }

    public Sale(Guid id, DateTime date) : base(id)
    {
        Date = date;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        var item = new SaleItem(Guid.NewGuid(), productId, quantity, unitPrice);
        _items.Add(item);
        RecalculateTotal();
        MarkAsUpdated();
    }

    private void RecalculateTotal()
    {
        Total = _items.Sum(i => i.SubTotal);
    }
}