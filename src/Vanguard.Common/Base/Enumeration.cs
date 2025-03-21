using System.Reflection;

namespace Vanguard.Common.Base
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }
        public int Id { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static T FromId<T>(int id) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(id, "id", item => item.Id == id);
            return matchingItem;
        }

        public static T FromName<T>(string name) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            return matchingItem! == null!
                ? throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}")
                : matchingItem;
        }

        public int CompareTo(object? other)
        {
            if (other is null)
                return 1; // any instance is considered greater than null
            return Id.CompareTo(((Enumeration)other).Id);
        }

        public static bool operator ==(Enumeration left, Enumeration right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Enumeration left, Enumeration right)
        {
            return !(left == right);
        }

        public static bool operator <(Enumeration left, Enumeration right)
        {
            if (left is null && right is null)
                return false;
            if (left is null)
                return true; // null is considered less than any non-null value
            if (right is null)
                return false; // non-null is not less than null
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Enumeration left, Enumeration right)
        {
            if (left is null && right is null)
                return true;
            if (left is null)
                return true; // null is less than or equal to any non-null
            if (right is null)
                return false; // non-null is not less than or equal to null
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Enumeration left, Enumeration right)
        {
            if (left is null && right is null)
                return false;
            if (left is null)
                return false; // null is never greater than a non-null value
            if (right is null)
                return true; // any non-null value is greater than null
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Enumeration left, Enumeration right)
        {
            if (left is null && right is null)
                return true;
            if (left is null)
                return false; // null is not greater than or equal to a non-null
            if (right is null)
                return true; // any non-null is considered greater than or equal to null
            return left.CompareTo(right) >= 0;
        }
    }
}
