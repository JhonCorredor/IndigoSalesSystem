using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Shared.Utilities.Mappers.Implementations;

/// <summary>
/// Mapper for User entity to UserDto.
/// </summary>
public class UserMapper : IMapper<User, UserDto>
{
    public UserDto Map(User source)
    {
        return new UserDto
        {
            Id = source.Id,
            Username = source.Username,
            Email = source.Email,
            FirstName = source.FirstName,
            LastName = source.LastName,
            FullName = source.FullName,
            RoleId = source.RoleId,
            RoleName = source.Role?.Name ?? "Unknown",
            LastLoginAt = source.LastLoginAt,
            IsActive = source.IsActive,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
