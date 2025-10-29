namespace Domain.ValueObjects;

public sealed record Money(decimal Amount, string Currency)
{
    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required.");
        return new Money(decimal.Round(amount, 2), currency.ToUpperInvariant());
    }
}