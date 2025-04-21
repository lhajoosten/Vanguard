namespace Vanguard.Core.Base
{
    /// <summary>
    /// Marks an entity as an aggregate root in Domain-Driven Design.
    /// An aggregate root is the entry point to an aggregate, which is a cluster of domain objects that can be treated as a single unit.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
    }
}
