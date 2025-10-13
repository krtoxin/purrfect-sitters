namespace Api.DTOs;

public class SitterDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal AverageRating { get; set; }
    public decimal? BaseRateAmount { get; set; }
    public string? BaseRateCurrency { get; set; }
    public string ServicesOffered { get; set; } = string.Empty;
    public int CompletedBookings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSitterDto
{
    public Guid UserId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public decimal? BaseRateAmount { get; set; }
    public string? BaseRateCurrency { get; set; }
    public string ServicesOffered { get; set; } = "DayVisit";
}

public class UpdateSitterDto
{
    public string Bio { get; set; } = string.Empty;
    public decimal? BaseRateAmount { get; set; }
    public string? BaseRateCurrency { get; set; }
    public string ServicesOffered { get; set; } = string.Empty;
}
