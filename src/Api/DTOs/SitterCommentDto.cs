namespace Api.DTOs;

public class SitterCommentDto
{
    public Guid Id { get; set; }
    public Guid SitterProfileId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSitterCommentDto
{
    public string Content { get; set; } = string.Empty;
}

public class UpdateSitterCommentDto
{
    public string Content { get; set; } = string.Empty;
}