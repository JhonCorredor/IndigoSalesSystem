using Core.Domain.Common;

namespace Core.Domain.Entities;

/// <summary>
/// Represents a role in the system for authorization purposes.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private readonly List<User> _users = [];
    public IReadOnlyCollection<User> Users => _users;

    private Role() { }

    public Role(Guid id, string name, string? description = null) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Description = description;
    }

    public void Update(string name, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Description = description;
        MarkAsUpdated();
    }
}
