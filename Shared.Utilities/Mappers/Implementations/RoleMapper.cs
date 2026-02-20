using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Shared.Utilities.Mappers.Implementations;

/// <summary>
/// Mapper for Role entity to RoleDto.
/// </summary>
public class RoleMapper : IMapper<Role, RoleDto>
{
    public RoleDto Map(Role source)
    {
        return new RoleDto
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            UsersCount = source.Users.Count,
            IsActive = source.IsActive,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}
