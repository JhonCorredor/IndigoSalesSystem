using Core.Domain.Common;

namespace Core.Domain.Entities;

/// <summary>
/// Represents a user in the system with authentication credentials.
/// </summary>
public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime? LastLoginAt { get; private set; }

    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }

    private User() { }

    public User(
        Guid id,
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        Guid roleId) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        Username = username.ToLowerInvariant();
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        RoleId = roleId;
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public void UpdateProfile(string firstName, string lastName, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        FirstName = firstName;
        LastName = lastName;
        Email = email.ToLowerInvariant();
        MarkAsUpdated();
    }

    public void ChangePassword(string newPasswordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);

        PasswordHash = newPasswordHash;
        MarkAsUpdated();
    }

    public void AssignRole(Guid roleId)
    {
        RoleId = roleId;
        MarkAsUpdated();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}
