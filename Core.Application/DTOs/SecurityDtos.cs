using Core.Application.Common;

namespace Core.Application.DTOs;

/// <summary>
/// DTO for User entity list operations.
/// </summary>
public record UserDto : BaseDto
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
    public Guid RoleId { get; init; }
    public DateTime? LastLoginAt { get; init; }
}

/// <summary>
/// DTO for Role entity list operations.
/// </summary>
public record RoleDto : BaseDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int UsersCount { get; init; }
}
