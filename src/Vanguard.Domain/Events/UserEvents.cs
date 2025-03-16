using Vanguard.Domain.Abstraction;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Events
{
    public record UserCreatedEvent(UserId UserId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserRoleAddedEvent(UserId UserId, Guid RoleId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserRoleRemovedEvent(UserId UserId, Guid RoleId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserProfileUpdatedEvent(UserId UserId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserEnrolledEvent(CourseId CourseId, UserId UserId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserProfileCreatedEvent(UserId UserId, UserProfileId ProfileId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserSettingsCreatedEvent(UserId UserId, UserSettingsId SettingsId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserActivatedEvent(UserId UserId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserDeactivatedEvent(UserId UserId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserSkillAssessedEvent(
        UserId UserId,
        SkillId SkillId,
        SkillAssessmentId AssessmentId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
