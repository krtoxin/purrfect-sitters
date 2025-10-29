namespace Domain.Users;

[Flags]
public enum UserRole
{
    None = 0,
    Owner = 1,
    Sitter = 2,
    Admin = 4
}