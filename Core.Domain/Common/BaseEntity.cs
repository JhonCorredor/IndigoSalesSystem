namespace Core.Domain.Common;

/// <summary>
/// Abstract base class for all domain entities.
/// Provides common properties like Id, audit fields, and soft delete support.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsActive { get; protected set; } = true;

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    protected BaseEntity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the entity as updated.
    /// </summary>
    protected void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft deletes the entity by setting IsActive to false.
    /// </summary>
    public virtual void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    /// <summary>
    /// Reactivates a soft-deleted entity.
    /// </summary>
    public virtual void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }
}
