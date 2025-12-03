using System;

namespace Api.DTOs;

public class DiscountDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty; 
    public decimal Percentage { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDiscountDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class UpdateDiscountDto
{
    public decimal Percentage { get; set; }
    public DateTime ExpiresAt { get; set; }
}
