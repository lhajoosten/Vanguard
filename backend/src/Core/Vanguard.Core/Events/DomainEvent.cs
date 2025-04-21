namespace Vanguard.Core.Events
{
    /// <summary>
    /// Base class for domain events
    /// </summary>
    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; }

        protected DomainEventBase()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}