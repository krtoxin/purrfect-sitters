namespace Domain.Common;

public readonly struct Optional<T>
{
    public bool HasValue { get; }
    public T? Value { get; }

    private Optional(T? value, bool hasValue)
    {
        Value = value;
        HasValue = hasValue;
    }

    public static Optional<T> None() => new(default, false);
    public static Optional<T> Some(T value) => new(value, true);

    public T GetValueOrDefault(T defaultValue) => HasValue ? Value! : defaultValue;
}