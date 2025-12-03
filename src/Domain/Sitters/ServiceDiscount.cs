using Domain.Common;

namespace Domain.Sitters;

public class ServiceDiscount : Entity
{
    private ServiceDiscount() { }

    private ServiceDiscount(Guid id, SitterServiceType category, decimal percentage, DateTime expiresAt)
    {
        Id = id;
        Category = category;
        Percentage = percentage;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public SitterServiceType Category { get; private set; }
    public decimal Percentage { get; private set; } // 0..100
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Result<ServiceDiscount> Create(SitterServiceType category, decimal percentage, DateTime expiresAt)
    {
        if (percentage <= 0 || percentage > 100)
            return Result<ServiceDiscount>.Failure("Percentage must be in (0,100].");

        if (expiresAt <= DateTime.UtcNow)
            return Result<ServiceDiscount>.Failure("Expiration must be in the future.");

        var entity = new ServiceDiscount(Guid.NewGuid(), category, percentage, expiresAt);
        return Result<ServiceDiscount>.Success(entity);
    }

    public Result Update(decimal percentage, DateTime expiresAt)
    {
        if (percentage <= 0 || percentage > 100)
            return Result.Failure("Percentage must be in (0,100].");

        if (expiresAt <= DateTime.UtcNow)
            return Result.Failure("Expiration must be in the future.");

        Percentage = percentage;
        ExpiresAt = expiresAt;
        return Result.Success();
    }

    public bool IsActive(DateTime utcNow) => utcNow < ExpiresAt;
}
