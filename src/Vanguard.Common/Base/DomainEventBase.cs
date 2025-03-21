using Vanguard.Common.Abstractions;

namespace Vanguard.Common.Base
{
    /// <summary>
    /// Base class for domain events
    /// </summary>
    public abstract class DomainEventBase : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }

        protected DomainEventBase(Guid id)
        {
            Id = id;
            OccurredOn = DateTime.UtcNow;
        }

        protected DomainEventBase() : this(Guid.NewGuid())
        {
        }
    }
}
