using Core.Application.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SaleRepository(ApplicationDbContext context) : ISaleRepository
{
    public async Task<Sale?> GetByIdWithItemsAsync(Guid id) =>
        await context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate) =>
        await context.Sales
            .Include(s => s.Items)
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .OrderByDescending(s => s.Date)
            .ToListAsync();

    public async Task AddAsync(Sale sale) => await context.Sales.AddAsync(sale);

    public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() > 0;
}