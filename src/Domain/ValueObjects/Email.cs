using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed record Email
{
    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || !Pattern.IsMatch(input))
            throw new ArgumentException("Invalid email.", nameof(input));
        return new Email(input.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;
}