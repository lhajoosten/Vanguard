namespace Vanguard.Common.Abstractions
{
    /// <summary>
    /// Interface for domain events
    /// </summary>
    public interface IDomainEvent
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}
