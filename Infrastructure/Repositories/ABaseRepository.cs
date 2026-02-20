using System.Linq.Expressions;
using Core.Application.Common;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Utilities.Mappers;

namespace Infrastructure.Repositories;

/// <summary>
/// Abstract base repository class implementing generic CRUD operations.
/// Uses Abstract Factory pattern to provide common data access functionality.
/// </summary>
/// <typeparam name="TEntity">The entity type, must inherit from BaseEntity.</typeparam>
/// <typeparam name="TDto">The DTO type for list operations.</typeparam>
public abstract class ABaseRepository<TEntity, TDto> : IBaseRepository<TEntity, TDto>
    where TEntity : BaseEntity
    where TDto : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly IMapper<TEntity, TDto>? Mapper;

    protected ABaseRepository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    protected ABaseRepository(ApplicationDbContext context, IMapper<TEntity, TDto> mapper)
        : this(context)
    {
        Mapper = mapper;
    }

    /// <summary>
    /// Override to specify includes for queries.
    /// </summary>
    protected virtual IQueryable<TEntity> GetQueryWithIncludes()
    {
        return DbSet.AsQueryable();
    }

    /// <summary>
    /// Override to implement custom filtering logic.
    /// </summary>
    protected virtual IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, QueryFilterRequest filters)
    {
        if (!filters.IncludeInactive)
        {
            query = query.Where(e => e.IsActive);
        }

        return query;
    }

    /// <summary>
    /// Override to implement sorting logic.
    /// </summary>
    protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, QueryFilterRequest filters)
    {
        if (string.IsNullOrWhiteSpace(filters.SortBy))
        {
            return query.OrderByDescending(e => e.CreatedAt);
        }

        return filters.SortDescending
            ? query.OrderByDescending(GetSortExpression(filters.SortBy))
            : query.OrderBy(GetSortExpression(filters.SortBy));
    }

    /// <summary>
    /// Override to provide custom sort expressions.
    /// </summary>
    protected virtual Expression<Func<TEntity, object>> GetSortExpression(string sortBy)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "createdat" => e => e.CreatedAt,
            "updatedat" => e => e.UpdatedAt!,
            "id" => e => e.Id,
            _ => e => e.CreatedAt
        };
    }

    /// <summary>
    /// Maps entity to DTO using injected mapper or override.
    /// </summary>
    protected virtual TDto MapToDto(TEntity entity)
    {
        if (Mapper is null)
        {
            throw new InvalidOperationException(
                $"No mapper configured for {typeof(TEntity).Name}. " +
                "Either inject IMapper<TEntity, TDto> or override MapToDto method.");
        }
        return Mapper.Map(entity);
    }

    /// <summary>
    /// Maps DTO collection from entities.
    /// </summary>
    protected virtual IEnumerable<TDto> MapToDtos(IEnumerable<TEntity> entities)
    {
        return entities.Select(MapToDto);
    }

    public virtual async Task<PagedResult<TDto>> GetAllAsync(QueryFilterRequest filters)
    {
        var query = GetQueryWithIncludes();
        query = ApplyFilters(query, filters);

        var totalCount = await query.CountAsync();

        query = ApplySorting(query, filters);

        var items = await query
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        var dtos = MapToDtos(items).ToList();

        return PagedResult<TDto>.Create(dtos, totalCount, filters.PageNumber, filters.PageSize);
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await GetQueryWithIncludes()
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        return MapToDtos(entities);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await GetQueryWithIncludes()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await DbSet.AddRangeAsync(entities);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null)
        {
            return false;
        }

        entity.Deactivate();
        Update(entity);
        return true;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(e => e.Id == id);
    }
}
