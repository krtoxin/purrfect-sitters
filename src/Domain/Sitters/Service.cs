namespace Domain.Sitters;

public class Service
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Service() { }
    public Service(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
