using Vanguard.Domain.Abstraction;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Events
{
    public record EnrollmentCompletedEvent(EnrollmentId EnrollmentId, UserId UserId, CourseId CourseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentDroppedEvent(EnrollmentId EnrollmentId, UserId UserId, CourseId CourseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentStatusChangedEvent(EnrollmentId EnrollmentId, EnrollmentStatus Status) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentGradeSetEvent(
        EnrollmentId EnrollmentId,
        UserId UserId,
        CourseId CourseId,
        int Grade) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentFinalExamScoreSetEvent(
        EnrollmentId EnrollmentId,
        UserId UserId,
        CourseId CourseId,
        int FinalExamScore) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
