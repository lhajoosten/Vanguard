using Vanguard.Common.Base;

namespace Vanguard.Domain.ValueObjects
{
    /// <summary>
    /// Represents a date range with a start and end date
    /// </summary>
    public class DateRange : ValueObject
    {
        private DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the start date
        /// </summary>
        public DateTime Start { get; }

        /// <summary>
        /// Gets the end date
        /// </summary>
        public DateTime End { get; }

        /// <summary>
        /// Gets the duration of the date range
        /// </summary>
        public TimeSpan Duration => End - Start;

        /// <summary>
        /// Creates a new date range
        /// </summary>
        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <returns>A new date range</returns>
        public static DateRange Create(DateTime start, DateTime end)
        {
            if (start > end)
                throw new ArgumentException("Start date must be before end date", nameof(start));

            return new DateRange(start, end);
        }

        /// <summary>
        /// Checks if this date range overlaps with another date range
        /// </summary>
        /// <param name="other">The other date range</param>
        /// <returns>True if the date ranges overlap, false otherwise</returns>
        public bool Overlaps(DateRange other)
        {
            return Start < other.End && other.Start < End;
        }

        /// <summary>
        /// Checks if this date range contains a date
        /// </summary>
        /// <param name="date">The date to check</param>
        /// <returns>True if the date range contains the date, false otherwise</returns>
        public bool Contains(DateTime date)
        {
            return date >= Start && date <= End;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }

        public override string ToString()
        {
            return $"{Start:d} - {End:d}";
        }
    }
}
