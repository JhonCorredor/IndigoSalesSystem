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
/// Repository implementation for Role entity using Abstract Factory pattern.
/// </summary>
public class RoleRepository : ABaseRepository<Role, RoleDto>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context, IMapper<Role, RoleDto> mapper) 
        : base(context, mapper) { }

    protected override IQueryable<Role> GetQueryWithIncludes()
    {
        return DbSet.Include(r => r.Users);
    }

    protected override IQueryable<Role> ApplyFilters(IQueryable<Role> query, QueryFilterRequest filters)
    {
        query = base.ApplyFilters(query, filters);

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var searchTerm = filters.SearchTerm.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(searchTerm) ||
                (r.Description != null && r.Description.ToLower().Contains(searchTerm)));
        }

        return query;
    }

    protected override Expression<Func<Role, object>> GetSortExpression(string sortBy)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "name" => r => r.Name,
            _ => base.GetSortExpression(sortBy)
        };
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<IEnumerable<Role>> GetActiveRolesAsync()
    {
        return await DbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    // Legacy interface methods
    async Task<IEnumerable<Role>> IRoleRepository.GetAllAsync()
    {
        return await DbSet.OrderBy(r => r.Name).ToListAsync();
    }

    async Task IRoleRepository.AddAsync(Role role)
    {
        await base.AddAsync(role);
    }

    async Task IRoleRepository.SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}
