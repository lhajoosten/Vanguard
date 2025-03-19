namespace Vanguard.Common.Abstractions
{
    /// <summary>
    /// Interface for domain event service
    /// </summary>
    public interface IDomainEventService
    {
        /// <summary>
        /// Publishes the domain events of the aggregate root
        /// </summary>
        /// <param name="aggregateRoot"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishEventsAsync(IAggregateRoot aggregateRoot, CancellationToken cancellationToken = default);
    }
}
