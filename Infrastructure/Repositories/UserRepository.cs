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
/// Repository implementation for User entity using Abstract Factory pattern.
/// </summary>
public class UserRepository : ABaseRepository<User, UserDto>, IUserRepository
{
    public UserRepository(ApplicationDbContext context, IMapper<User, UserDto> mapper) 
        : base(context, mapper) { }

    protected override IQueryable<User> GetQueryWithIncludes()
    {
        return DbSet.Include(u => u.Role);
    }

    protected override IQueryable<User> ApplyFilters(IQueryable<User> query, QueryFilterRequest filters)
    {
        query = base.ApplyFilters(query, filters);

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var searchTerm = filters.SearchTerm.ToLower();
            query = query.Where(u =>
                u.Username.ToLower().Contains(searchTerm) ||
                u.Email.ToLower().Contains(searchTerm) ||
                u.FirstName.ToLower().Contains(searchTerm) ||
                u.LastName.ToLower().Contains(searchTerm));
        }

        return query;
    }

    protected override Expression<Func<User, object>> GetSortExpression(string sortBy)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "username" => u => u.Username,
            "email" => u => u.Email,
            "firstname" => u => u.FirstName,
            "lastname" => u => u.LastName,
            "lastloginat" => u => u.LastLoginAt!,
            _ => base.GetSortExpression(sortBy)
        };
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var normalizedUsername = username.ToLowerInvariant();
        return await GetQueryWithIncludes()
            .FirstOrDefaultAsync(u => u.Username == normalizedUsername);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await GetQueryWithIncludes()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
    }

    public async Task<bool> ExistsAsync(string username, string email)
    {
        var normalizedUsername = username.ToLowerInvariant();
        var normalizedEmail = email.ToLowerInvariant();

        return await DbSet
            .AnyAsync(u => u.Username == normalizedUsername || u.Email == normalizedEmail);
    }

    // Legacy interface methods
    async Task<IEnumerable<User>> IUserRepository.GetAllAsync()
    {
        return await GetQueryWithIncludes()
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    async Task IUserRepository.AddAsync(User user)
    {
        await base.AddAsync(user);
    }

    async Task IUserRepository.SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}
