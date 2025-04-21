// src/Core/Vanguard.Core/Events/IDomainEventDispatcher.cs
namespace Vanguard.Core.Events
{
    /// <summary>
    /// Interface for dispatching domain events
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
    }
}