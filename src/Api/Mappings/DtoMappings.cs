using Api.DTOs;
using Domain.Pets;
using Domain.Sitters;
using Domain.Users;
using Domain.ValueObjects;

namespace Api.Mappings;

public static class DtoMappings
{
    public static UserDto ToDto(this Domain.Users.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email.Value,
            Name = user.Name,
            Roles = user.Roles.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static Domain.Users.User ToDomain(this CreateUserDto dto)
    {
        return Domain.Users.User.Register(
            Guid.NewGuid(),
            Email.Create(dto.Email),
            dto.Name,
            Enum.Parse<UserRole>(dto.Roles, ignoreCase: true)
        );
    }

    public static PetDto ToDto(this Domain.Pets.Pet pet)
    {
        return new PetDto
        {
            Id = pet.Id,
            OwnerId = pet.OwnerId,
            Name = pet.Name,
            Type = pet.Type.ToString(),
            Breed = pet.Breed,
            Notes = pet.Notes,
            CreatedAt = pet.CreatedAt,
            UpdatedAt = pet.CreatedAt 
        };
    }

    public static Domain.Pets.Pet ToDomain(this CreatePetDto dto)
    {
        return Domain.Pets.Pet.Create(
            Guid.NewGuid(),
            dto.OwnerId,
            dto.Name,
            Enum.Parse<PetType>(dto.Type, ignoreCase: true),
            dto.Breed,
            dto.Notes
        );
    }

    public static SitterDto ToDto(this Domain.Sitters.SitterProfile sitter)
    {
        return new SitterDto
        {
            Id = sitter.Id,
            UserId = sitter.UserId,
            Bio = sitter.Bio,
            IsActive = sitter.IsActive,
            AverageRating = sitter.AverageRating,
            BaseRateAmount = sitter.BaseRate?.Amount,
            BaseRateCurrency = sitter.BaseRate?.Currency,
            ServicesOffered = sitter.ServicesOffered.ToString(),
            CompletedBookings = sitter.CompletedBookings,
            CreatedAt = sitter.CreatedAt,
            UpdatedAt = sitter.CreatedAt 
        };
    }

    public static Domain.Sitters.SitterProfile ToDomain(this CreateSitterDto dto)
    {
        var serviceType = Enum.Parse<SitterServiceType>(dto.ServicesOffered, ignoreCase: true);
        var baseRate = dto.BaseRateAmount.HasValue && !string.IsNullOrEmpty(dto.BaseRateCurrency)
            ? Money.Create(dto.BaseRateAmount.Value, dto.BaseRateCurrency)
            : null;

        return Domain.Sitters.SitterProfile.Create(
            Guid.NewGuid(),
            dto.UserId,
            dto.Bio,
            baseRate,
            serviceType
        );
    }

    public static BookingDto ToDto(this Domain.Bookings.Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            PetId = booking.PetId,
            OwnerId = booking.OwnerId,
            SitterProfileId = booking.SitterProfileId,
            StartUtc = booking.StartUtc,
            EndUtc = booking.EndUtc,
            Status = booking.Status.ToString(),
            Price = new BookingPriceDto {
                BaseAmount = booking.Price.BaseAmount,
                ServiceFeePercent = booking.Price.ServiceFeePercent,
                ServiceFeeAmount = booking.Price.ServiceFeeAmount,
                TotalAmount = booking.Price.TotalAmount,
                Currency = booking.Price.Currency
            },
            ServiceType = booking.ServiceType.ToString(),
            CareInstructions = booking.CareInstructionSnapshots.Select(ci => new Api.Contracts.Bookings.Responses.BookingCareInstructionResponse(ci.Id, ci.Text, ci.CreatedAt)),
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }

    public static Domain.Bookings.Booking ToDomain(this CreateBookingDto dto)
    {
        var price = Domain.Bookings.BookingPrice.Create(
            dto.BaseAmount,
            dto.ServiceFeePercent ?? 0,
            dto.Currency
        );
        var serviceType = Domain.Sitters.SitterServiceType.None; // Default to None
        var careInstructions = dto.CareInstructionTexts?.ToArray() ?? Array.Empty<string>();

        return Domain.Bookings.Booking.Create(
            Guid.NewGuid(),
            dto.PetId,
            Guid.Empty, // OwnerId not present in CreateBookingDto
            dto.SitterProfileId,
            dto.StartUtc,
            dto.EndUtc,
            price,
            serviceType,
            careInstructions
        );
    }
}
