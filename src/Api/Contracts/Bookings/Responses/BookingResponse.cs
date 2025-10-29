using System;
using System.Collections.Generic;
using Domain.Bookings;

namespace Api.Contracts.Bookings.Responses;

public sealed record BookingResponse(
    Guid Id,
    Guid PetId,
    Guid OwnerId,
    Guid SitterProfileId,
    DateTime StartUtc,
    DateTime EndUtc,
    string Status,
    BookingPriceResponse Price,
    bool IsReviewed,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? CompletedAtUtc,
    DateTime? CancelledAtUtc,
    BookingCancellationReason? CancellationReason,
    byte[] RowVersion,
    IEnumerable<BookingStatusHistoryResponse> StatusHistory,
    IEnumerable<BookingCareInstructionResponse> CareInstructions);
