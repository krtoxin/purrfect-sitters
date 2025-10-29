namespace Api.DTOs;

public class PetDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePetDto
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePetDto
{
    public string Name { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Notes { get; set; }
}
