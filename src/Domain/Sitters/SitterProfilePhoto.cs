namespace Domain.Sitters;

public class SitterProfilePhoto
{
    public Guid Id { get; private set; }
    public Guid SitterProfileId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private SitterProfilePhoto() { }
    public SitterProfilePhoto(Guid id, Guid sitterProfileId, string url)
    {
        Id = id;
        SitterProfileId = sitterProfileId;
        Url = url;
        CreatedAt = DateTime.UtcNow;
    }
}
