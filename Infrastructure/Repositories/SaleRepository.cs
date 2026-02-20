using System.Linq.Expressions;
using Core.Application.Common;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Utilities.Mappers;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Sale entity using Abstract Factory pattern.
/// </summary>
public class SaleRepository : ABaseRepository<Sale, SaleDto>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context, IMapper<Sale, SaleDto> mapper) 
        : base(context, mapper) { }

    protected override IQueryable<Sale> GetQueryWithIncludes()
    {
        return DbSet
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .AsSplitQuery();
    }

    protected override Expression<Func<Sale, object>> GetSortExpression(string sortBy)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "date" => s => s.Date,
            "total" => s => s.Total,
            _ => base.GetSortExpression(sortBy)
        };
    }

    public async Task<Sale?> GetByIdWithItemsAsync(Guid id)
    {
        return await GetQueryWithIncludes()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await GetQueryWithIncludes()
            .Where(s => s.Date >= startDate && s.Date <= endDate && s.IsActive)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    // Legacy interface methods
    async Task ISaleRepository.AddAsync(Sale sale)
    {
        await base.AddAsync(sale);
    }

    async Task<bool> ISaleRepository.SaveChangesAsync()
    {
        return await base.SaveChangesAsync() > 0;
    }
}