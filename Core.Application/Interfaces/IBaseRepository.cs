using Core.Application.Common;

namespace Core.Application.Interfaces;

/// <summary>
/// Base repository interface defining generic CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TDto">The DTO type for list operations.</typeparam>
public interface IBaseRepository<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    /// <summary>
    /// Retrieves all entities with optional filtering and pagination.
    /// </summary>
    Task<PagedResult<TDto>> GetAllAsync(QueryFilterRequest filters);

    /// <summary>
    /// Retrieves all entities without pagination.
    /// </summary>
    Task<IEnumerable<TDto>> GetAllAsync();

    /// <summary>
    /// Retrieves a single entity by its unique identifier.
    /// </summary>
    Task<TEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Adds multiple entities to the data store.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an existing entity in the data store.
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Deletes an entity by its unique identifier (soft delete if supported).
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Saves all pending changes to the data store.
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Checks if an entity with the specified ID exists.
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}
