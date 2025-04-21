namespace Vanguard.Core.Events
{
    /// <summary>
    /// Marker interface for domain events
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
