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
/// Repository implementation for Product entity using Abstract Factory pattern.
/// </summary>
public class ProductRepository : ABaseRepository<Product, ProductDto>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context, IMapper<Product, ProductDto> mapper) 
        : base(context, mapper) { }

    protected override IQueryable<Product> ApplyFilters(IQueryable<Product> query, QueryFilterRequest filters)
    {
        query = base.ApplyFilters(query, filters);

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var searchTerm = filters.SearchTerm.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm));
        }

        return query;
    }

    protected override Expression<Func<Product, object>> GetSortExpression(string sortBy)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "name" => p => p.Name,
            "price" => p => p.Price,
            "stock" => p => p.Stock,
            _ => base.GetSortExpression(sortBy)
        };
    }

    // Legacy interface methods for backward compatibility
    async Task<IEnumerable<Product>> IProductRepository.GetAllAsync()
    {
        return await DbSet.Where(p => p.IsActive).ToListAsync();
    }

    async Task IProductRepository.AddAsync(Product product)
    {
        await base.AddAsync(product);
    }

    async Task<bool> IProductRepository.SaveChangesAsync()
    {
        return await base.SaveChangesAsync() > 0;
    }

    void IProductRepository.Delete(Product product)
    {
        product.Deactivate();
        Update(product);
    }
}