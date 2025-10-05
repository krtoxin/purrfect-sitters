using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Users;

public class User : AggregateRoot
{
    private User() { }

    private User(Guid id, Email email, string name, UserRole roles)
    {
        Id = id;
        Email = email;
        Name = name;
        Roles = roles;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public Email Email { get; private set; }
    public string Name { get; private set; } = default!;
    public UserRole Roles { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static User Register(Guid id, Email email, string name, UserRole initialRoles = UserRole.Owner)
        => new(id, email, name, initialRoles);

    public void AddRole(UserRole role)
    {
        if (!Roles.HasFlag(role))
        {
            Roles |= role;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveRole(UserRole role)
    {
        if (Roles.HasFlag(role))
        {
            Roles &= ~role;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Name required.");
        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}