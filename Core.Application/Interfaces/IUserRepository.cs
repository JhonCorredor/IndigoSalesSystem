using Core.Domain.Entities;

namespace Core.Application.Interfaces;

/// <summary>
/// Repository interface for User entity operations.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> ExistsAsync(string username, string email);
    Task AddAsync(User user);
    void Update(User user);
    Task SaveChangesAsync();
}
