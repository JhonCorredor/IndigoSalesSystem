using Core.Domain.Entities;

namespace Core.Application.Interfaces;

/// <summary>
/// Repository interface for Role entity operations.
/// </summary>
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetActiveRolesAsync();
    Task AddAsync(Role role);
    void Update(Role role);
    Task SaveChangesAsync();
}
