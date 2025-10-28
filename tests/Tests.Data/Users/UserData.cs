using Domain.Users;
using Domain.ValueObjects;

namespace Tests.Data.Users;

public static class UserData
{
    public static User CreateUser(
        string id = "test-user-id",
        string name = "John Doe",
        string email = "john.doe@example.com",
        UserRole roles = UserRole.Owner)
    {
        return User.Register(
            Guid.Parse(id),
            Email.Create(email),
            name,
            roles
        );
    }

    public static IEnumerable<User> CreateUsers(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateUser(
                id: $"test-user-{i}",
                name: $"John Doe {i}",
                email: $"john.doe{i}@example.com"
            ));
    }
}