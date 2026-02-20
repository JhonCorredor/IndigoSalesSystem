using Core.Domain.Entities;

namespace Core.Application.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdWithItemsAsync(Guid id);
    Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(Sale sale);
    Task<bool> SaveChangesAsync();
}