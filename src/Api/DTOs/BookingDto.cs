namespace Api.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid SitterId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal ServiceFee { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string CareInstructions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBookingDto
{
    public Guid PetId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid SitterId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ServiceFee { get; set; }
    public string Currency { get; set; } = "USD";
    public string ServiceType { get; set; } = string.Empty;
    public string CareInstructions { get; set; } = string.Empty;
}

public class UpdateBookingDto
{
    public string Status { get; set; } = string.Empty;
    public string CareInstructions { get; set; } = string.Empty;
}
