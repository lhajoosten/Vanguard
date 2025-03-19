namespace Vanguard.Common.Abstractions
{
    /// <summary>
    /// Interface for aggregate roots
    /// </summary>
    public interface IAggregateRoot
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }

    /// <summary>
    /// Generic interface for aggregate roots with an ID property
    /// </summary>
    /// <typeparam name="TId">The type of the aggregate's identifier</typeparam>
    public interface IAggregateRoot<TId> : IAggregateRoot, IEntity<TId>
    {
        void AddDomainEvent(IDomainEvent domainEvent);
    }
}
