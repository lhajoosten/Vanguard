namespace Vanguard.Common.Abstractions
{
    /// <summary>
    /// Base interface for all entities
    /// </summary>
    public interface IEntity
    {
        DateTime CreatedAt { get; }
        DateTime? ModifiedAt { get; }
    }

    /// <summary>
    /// Generic entity interface with an ID property
    /// </summary>
    /// <typeparam name="TId">The type of the entity's identifier</typeparam>
    public interface IEntity<TId> : IEntity
    {
        TId Id { get; }
    }
}
