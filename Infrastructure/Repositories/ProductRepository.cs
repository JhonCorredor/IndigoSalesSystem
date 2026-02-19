using Core.Application.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id) => await context.Products.FindAsync(id);

    public async Task<IEnumerable<Product>> GetAllAsync() => await context.Products.ToListAsync();

    public async Task AddAsync(Product product) => await context.Products.AddAsync(product);

    public void Update(Product product) => context.Products.Update(product);

    public void Delete(Product product) => context.Products.Remove(product);

    public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() > 0;
}