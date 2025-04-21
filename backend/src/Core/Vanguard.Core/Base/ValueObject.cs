namespace Vanguard.Core.Base
{
    /// <summary>
    /// Base class for value objects in Domain-Driven Design.
    /// Value objects are immutable objects that contain attributes but no identity.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Gets the components that should be used for equality checking.
        /// </summary>
        /// <returns>An enumerable of objects representing the value components.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }
    }
}