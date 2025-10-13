using Api.DTOs;
using Domain.Pets;
using Domain.Sitters;
using Domain.Users;
using Domain.ValueObjects;

namespace Api.Mappings;

public static class DtoMappings
{
    // User mappings
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

    // Pet mappings
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
            UpdatedAt = pet.CreatedAt // Pet doesn't have UpdatedAt, using CreatedAt
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

    // Sitter mappings
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
            UpdatedAt = sitter.CreatedAt // SitterProfile doesn't have UpdatedAt
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

    // Booking mappings
    public static BookingDto ToDto(this Domain.Bookings.Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            PetId = booking.PetId,
            OwnerId = booking.OwnerId,
            SitterId = booking.SitterProfileId,
            StartDate = booking.StartUtc,
            EndDate = booking.EndUtc,
            Status = booking.Status.ToString(),
            TotalAmount = booking.Price.TotalAmount,
            ServiceFee = booking.Price.ServiceFeeAmount,
            Currency = booking.Price.Currency,
            ServiceType = booking.ServiceType.ToString(),
            CareInstructions = string.Join("; ", booking.CareInstructionSnapshots.Select(ci => ci.Text)),
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }

    public static Domain.Bookings.Booking ToDomain(this CreateBookingDto dto)
    {
        var price = Domain.Bookings.BookingPrice.Create(dto.TotalAmount, dto.ServiceFee, dto.Currency);
        var serviceType = Enum.Parse<SitterServiceType>(dto.ServiceType, ignoreCase: true);
        var careInstructions = dto.CareInstructions.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(instruction => instruction.Trim())
            .ToArray();

        return Domain.Bookings.Booking.Create(
            Guid.NewGuid(),
            dto.PetId,
            dto.OwnerId,
            dto.SitterId,
            dto.StartDate,
            dto.EndDate,
            price,
            serviceType,
            careInstructions
        );
    }
}
