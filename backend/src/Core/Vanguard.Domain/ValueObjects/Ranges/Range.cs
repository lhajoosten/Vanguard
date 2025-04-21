using Ardalis.GuardClauses;

namespace Vanguard.Domain.ValueObjects.Ranges
{
    public class Range<T> where T : IComparable<T>
    {
        public Range(T min, T max)
        {
            Guard.Against.Null(min, nameof(min));
            Guard.Against.Null(max, nameof(max));

            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentException("Min cannot be greater than max.");
            }

            if (min.Equals(max))
            {
                throw new ArgumentException("Min and max cannot be equal.");
            }

            Min = min;
            Max = max;
        }

        public T Min { get; }
        public T Max { get; }

        public bool IsInRange(T value)
        {
            return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
        }

        public bool IsEmpty()
        {
            return Min.CompareTo(Max) > 0;
        }

        public bool IsOverlapping(Range<T> other)
        {
            return !(other.Min.CompareTo(Max) > 0 || Min.CompareTo(other.Max) > 0);
        }

        public Range<T> GetIntersection(Range<T> other)
        {
            if (!IsOverlapping(other))
            {
                throw new InvalidOperationException("Ranges do not overlap.");
            }
            return new Range<T>(Min.CompareTo(other.Min) > 0 ? Min : other.Min,
                                Max.CompareTo(other.Max) < 0 ? Max : other.Max);
        }
    }
}
