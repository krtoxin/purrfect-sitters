namespace Domain.Sitters;

[Flags]
public enum SitterServiceType
{
    None = 0,
    DayVisit = 1,
    OvernightStay = 2,
    Walking = 4,
    FeedingOnly = 8,
    MedicationAdministration = 16
}