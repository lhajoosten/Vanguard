namespace Vanguard.Domain.ValueObjects.Ranges
{
    public class TimeOnlyRange : Range<TimeOnly>
    {
        public TimeOnlyRange(TimeOnly min, TimeOnly max) : base(min, max)
        {
            if (min > max)
            {
                throw new ArgumentException("Min time cannot be greater than max time.");
            }
        }

        // Checks if this time range contains the specified time
        public bool Contains(TimeOnly time)
        {
            return IsInRange(time);
        }

        // Determines if this time range spans midnight
        public bool SpansMidnight()
        {
            return Min > Max;
        }

        // Calculate the duration of this time range
        public TimeSpan Duration()
        {
            if (SpansMidnight())
            {
                // Calculate duration that wraps around midnight
                return TimeSpan.FromHours(24) - Min.ToTimeSpan() + Max.ToTimeSpan();
            }
            else
            {
                return Max.ToTimeSpan() - Min.ToTimeSpan();
            }
        }
    }
}
