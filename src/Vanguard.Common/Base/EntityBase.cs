using Vanguard.Common.Abstractions;

namespace Vanguard.Common.Base
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    /// <typeparam name="TId">The type of the entity's identifier</typeparam>
    public abstract class EntityBase<TId> : IEntity<TId>
    {
        public TId Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? ModifiedAt { get; protected set; }

        // For EF Core
        protected EntityBase() { }

        protected EntityBase(TId id)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not EntityBase<TId> other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Id!.Equals(default(TId)) || other.Id!.Equals(default(TId)))
                return false;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id!.GetHashCode();
        }

        public static bool operator ==(EntityBase<TId> left, EntityBase<TId> right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(EntityBase<TId> left, EntityBase<TId> right)
        {
            return !(left == right);
        }
    }
}
