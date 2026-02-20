namespace Core.Application.Common;

/// <summary>
/// Abstract base record for all DTOs.
/// </summary>
public abstract record BaseDto
{
    public Guid Id { get; init; }
    public bool IsActive { get; init; } = true;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
