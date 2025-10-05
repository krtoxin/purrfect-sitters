namespace Domain.Bookings;

public class BookingPrice
{
    private BookingPrice() { }

    private BookingPrice(decimal baseAmount, decimal serviceFeePercent, string currency)
    {
        if (baseAmount <= 0) throw new ArgumentException("Base amount must be > 0.", nameof(baseAmount));
        if (serviceFeePercent < 0 || serviceFeePercent > 100) throw new ArgumentException("Service fee percent invalid.", nameof(serviceFeePercent));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required.", nameof(currency));

        BaseAmount = decimal.Round(baseAmount, 2);
        ServiceFeePercent = decimal.Round(serviceFeePercent, 2);
        ServiceFeeAmount = decimal.Round(BaseAmount * (ServiceFeePercent / 100m), 2);
        TotalAmount = BaseAmount + ServiceFeeAmount;
        Currency = currency.ToUpperInvariant();
    }

    public decimal BaseAmount { get; private set; }
    public decimal ServiceFeePercent { get; private set; }
    public decimal ServiceFeeAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = default!;

    public static BookingPrice Create(decimal baseAmount, decimal serviceFeePercent, string currency)
        => new(baseAmount, serviceFeePercent, currency);

    public BookingPrice Recalculate(decimal? newBaseAmount = null, decimal? newServiceFeePercent = null)
    {
        return Create(newBaseAmount ?? BaseAmount, newServiceFeePercent ?? ServiceFeePercent, Currency);
    }
}