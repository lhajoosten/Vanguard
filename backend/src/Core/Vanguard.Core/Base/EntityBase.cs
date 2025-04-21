namespace Vanguard.Core.Base
{
    public abstract class EntityBase : IEntity
    {
        public int Id { get; protected set; }

        protected EntityBase() { }

        protected EntityBase(int id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            if (obj is not EntityBase other)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(EntityBase left, EntityBase right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(EntityBase left, EntityBase right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * 41;
        }
    }
}
