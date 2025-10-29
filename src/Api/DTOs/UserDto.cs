namespace Api.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Roles { get; set; } = "Owner";
}

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
