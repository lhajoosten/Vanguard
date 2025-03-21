using MediatR;
using Microsoft.Extensions.Logging;
using Vanguard.Common.Abstractions;

namespace Vanguard.Infrastructure.Persistence.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DomainEventService> _logger;

        public DomainEventService(
            IMediator mediator,
            ILogger<DomainEventService> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsAsync(IAggregateRoot aggregateRoot, CancellationToken cancellationToken = default)
        {
            if (aggregateRoot == null || !aggregateRoot.DomainEvents.Any())
                return;

            _logger.LogInformation("Publishing {EventCount} domain events for aggregate {AggregateType}",
                aggregateRoot.DomainEvents.Count,
                aggregateRoot.GetType().Name);

            foreach (var domainEvent in aggregateRoot.DomainEvents)
            {
                _logger.LogDebug("Publishing domain event {EventType}", domainEvent.GetType().Name);

                try
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing domain event {EventType}", domainEvent.GetType().Name);
                    throw; // Rethrow to ensure transaction is rolled back
                }
            }
        }
    }
}
