namespace Domain.ValueObjects;

public sealed record Address(string Line1, string? Line2, string City, string StateOrProvince, string PostalCode, string Country)
{
    public static Address Create(string line1, string? line2, string city, string state, string postal, string country)
    {
        if (string.IsNullOrWhiteSpace(line1)) throw new ArgumentException("Line1 required.");
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City required.");
        if (string.IsNullOrWhiteSpace(postal)) throw new ArgumentException("Postal code required.");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country required.");
        return new Address(line1.Trim(), string.IsNullOrWhiteSpace(line2) ? null : line2.Trim(), city.Trim(), state.Trim(), postal.Trim(), country.Trim().ToUpperInvariant());
    }

    public override string ToString() => $"{Line1}{(Line2 is null ? "" : " " + Line2)}, {City}, {StateOrProvince} {PostalCode}, {Country}";
}