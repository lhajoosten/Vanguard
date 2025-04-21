using Vanguard.Core.Events;

namespace Vanguard.Core.Base
{
    /// <summary>
    /// Base class for aggregate roots that support domain events
    /// </summary>
    public abstract class AggregateRoot : EntityBase, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot() : base() { }

        protected AggregateRoot(int id) : base(id) { }

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}