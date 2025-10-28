using Domain.Pets;

public class CreatePetRequest
{
    public string Name { get; set; } = string.Empty;
    public PetType Type { get; set; }
    public string? Breed { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePetRequest
{
    public string Name { get; set; } = string.Empty;
    public PetType Type { get; set; }
    public string? Breed { get; set; }
    public string? Notes { get; set; }
}