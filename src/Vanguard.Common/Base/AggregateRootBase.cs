using Vanguard.Common.Abstractions;

namespace Vanguard.Common.Base
{
    /// <summary>
    /// Base class for aggregate roots
    /// </summary>
    /// <typeparam name="TId">The type of the aggregate's identifier</typeparam>
    public abstract class AggregateRootBase<TId> : EntityBase<TId>, IAggregateRoot<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRootBase() { }

        protected AggregateRootBase(TId id) : base(id) { }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
