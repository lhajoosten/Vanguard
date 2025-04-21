namespace Vanguard.Domain.ValueObjects.Ranges
{
    public class DateOnlyRange : Range<DateOnly>
    {
        public DateOnlyRange(DateOnly min, DateOnly max) : base(min, max)
        {
            if (min > max)
            {
                throw new ArgumentException("Min date cannot be greater than max date.");
            }
        }

        // Calculate the number of days in this range (inclusive)
        public int DayCount()
        {
            return Max.DayNumber - Min.DayNumber + 1;
        }

        // Check if a specific date is contained in this range
        public bool Contains(DateOnly date)
        {
            return IsInRange(date);
        }

        // Check if this date range contains another date range entirely
        public bool Contains(DateOnlyRange other)
        {
            return Min <= other.Min && Max >= other.Max;
        }
    }
}
