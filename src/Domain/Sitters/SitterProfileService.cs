namespace Domain.Sitters;

public class SitterProfileService
{
    public Guid SitterProfileId { get; private set; }
    public Guid ServiceId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SitterProfileService() { }
    public SitterProfileService(Guid sitterProfileId, Guid serviceId)
    {
        SitterProfileId = sitterProfileId;
        ServiceId = serviceId;
        CreatedAt = DateTime.UtcNow;
    }
}
