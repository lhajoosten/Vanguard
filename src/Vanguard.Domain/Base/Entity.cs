namespace Vanguard.Domain.Base
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? ModifiedAt { get; protected set; }

        // For EF Core
        protected Entity() { }

        protected Entity(TId id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Entity<TId> other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Id.Equals(default(TId)) || other.Id.Equals(default(TId)))
                return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !(left == right);
        }
    }
}
