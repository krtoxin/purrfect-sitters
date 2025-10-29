using Domain.Users;
using Domain.ValueObjects;

namespace Tests.Data.Users;

public static class UserData
{
    public static User CreateUser(
        string? id = null,
        string? name = null,
        string? email = null,
        UserRole roles = UserRole.Owner)
    {
        var userId = id != null ? Guid.Parse(id) : Guid.NewGuid();
        var uniqueEmail = email ?? $"user_{Guid.NewGuid()}@example.com";
        var userName = name ?? $"User {Guid.NewGuid()}";
        return User.Register(
            userId,
            Email.Create(uniqueEmail),
            userName,
            roles
        );
    }

    public static IEnumerable<User> CreateUsers(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateUser(
                id: Guid.NewGuid().ToString(),
                name: $"User {Guid.NewGuid()}",
                email: $"user_{Guid.NewGuid()}@example.com"
            ));
    }
}