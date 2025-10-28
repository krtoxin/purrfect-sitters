using Domain.Common;
using Domain.Users;
using Domain.ValueObjects;

namespace Domain.Sitters;

public class Sitter : Entity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public Email EmailValue { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public string Bio { get; private set; } = default!;
    public Money HourlyRate { get; private set; } = default!;
    public bool IsAvailable { get; private set; }

    private Sitter() : base() { }

    public Sitter(
        Guid id,
        string firstName,
        string lastName,
        string email,
        Email emailValue,
        string phone,
        Address address,
        string bio,
        Money hourlyRate,
        bool isAvailable = true)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        EmailValue = emailValue;
        Phone = phone;
        Address = address;
        Bio = bio;
        HourlyRate = hourlyRate;
        IsAvailable = isAvailable;
    }

    public void Update(
        string firstName,
        string lastName,
        string phone,
        string bio,
        Money hourlyRate,
        Address address,
        bool isAvailable)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Bio = bio;
        HourlyRate = hourlyRate;
        Address = address;
        IsAvailable = isAvailable;
    }
}