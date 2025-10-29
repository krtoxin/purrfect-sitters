using System;

using System.Collections.Generic;
using Api.Contracts.Bookings.Responses;

namespace Api.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid SitterProfileId { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public string Status { get; set; } = string.Empty;

    public BookingPriceDto Price { get; set; } = new();

    public string ServiceType { get; set; } = string.Empty;

    public IEnumerable<BookingCareInstructionResponse> CareInstructions { get; set; } = Array.Empty<BookingCareInstructionResponse>();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}

public class BookingPriceDto
{
    public decimal BaseAmount { get; set; }

    public decimal? ServiceFeePercent { get; set; }

    public decimal ServiceFeeAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "USD";
}


public class CreateBookingDto
{
    public Guid PetId { get; set; }
    public Guid SitterProfileId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal? ServiceFeePercent { get; set; }
    public string Currency { get; set; } = "USD";
    public IEnumerable<string>? CareInstructionTexts { get; set; }
}


public class UpdateBookingDto
{
    public string? Status { get; set; }

    public string? CareInstructions { get; set; }

}