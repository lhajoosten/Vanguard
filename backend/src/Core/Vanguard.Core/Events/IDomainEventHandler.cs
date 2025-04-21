namespace Vanguard.Core.Events
{
    /// <summary>
    /// Interface for domain event handlers
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event</typeparam>
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
    }
}