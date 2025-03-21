using Vanguard.Common.Abstractions;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Domain.Events
{
    public record EnrollmentCompletedEvent(EnrollmentId EnrollmentId, UserId UserId, CourseId CourseId) : IDomainEvent
    {
        public Guid Id { get; } = EnrollmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentDroppedEvent(EnrollmentId EnrollmentId, UserId UserId, CourseId CourseId) : IDomainEvent
    {
        public Guid Id { get; } = EnrollmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentStatusChangedEvent(EnrollmentId EnrollmentId, EnrollmentStatus Status) : IDomainEvent
    {
        public Guid Id { get; } = EnrollmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentGradeSetEvent(
        EnrollmentId EnrollmentId,
        UserId UserId,
        CourseId CourseId,
        int Grade) : IDomainEvent
    {
        public Guid Id { get; } = EnrollmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record EnrollmentFinalExamScoreSetEvent(
        EnrollmentId EnrollmentId,
        UserId UserId,
        CourseId CourseId,
        int FinalExamScore) : IDomainEvent
    {
        public Guid Id { get; } = EnrollmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record LessonCompletedEvent(EnrollmentId EnrollmentId, LessonId LessonId) : IDomainEvent
    {
        public Guid Id { get; } = EnrollmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
