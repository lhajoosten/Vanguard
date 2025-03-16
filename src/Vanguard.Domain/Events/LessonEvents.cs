using Vanguard.Domain.Abstraction;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Events
{
    public record LessonCompletedEvent(EnrollmentId EnrollmentId, LessonId LessonId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
