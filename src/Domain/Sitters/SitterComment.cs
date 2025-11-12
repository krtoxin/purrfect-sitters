using Domain.Common;

namespace Domain.Sitters;

public class SitterComment : Entity
{
    public Guid SitterProfileId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private SitterComment() { }

    private SitterComment(Guid id, Guid sitterProfileId, string content)
    {
        Id = id;
        SitterProfileId = sitterProfileId;
        UpdateContent(content);
        CreatedAt = DateTime.UtcNow;
    }

    public static SitterComment Create(Guid sitterProfileId, string content)
        => new(Guid.NewGuid(), sitterProfileId, content);

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty.");
        Content = content.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}