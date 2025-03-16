namespace Vanguard.Domain.Abstraction
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
