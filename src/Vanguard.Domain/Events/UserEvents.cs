using Vanguard.Common.Abstractions;
using Vanguard.Common.Base;

namespace Vanguard.Domain.Events
{
    public record UserCreatedEvent(UserId UserId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserRoleAddedEvent(UserId UserId, Guid RoleId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserRoleRemovedEvent(UserId UserId, Guid RoleId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserProfileUpdatedEvent(UserId UserId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserEnrolledEvent(CourseId CourseId, UserId UserId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserProfileCreatedEvent(UserId UserId, UserProfileId ProfileId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserSettingsCreatedEvent(UserId UserId, UserSettingsId SettingsId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserActivatedEvent(UserId UserId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserDeactivatedEvent(UserId UserId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record UserSkillAssessedEvent(
        UserId UserId,
        SkillId SkillId,
        SkillAssessmentId AssessmentId) : IDomainEvent
    {
        public Guid Id { get; } = UserId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
