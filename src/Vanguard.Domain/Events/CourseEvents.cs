using Vanguard.Domain.Abstraction;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Events
{
    public record CourseCreatedEvent(CourseId CourseId, UserId CreatorId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseUpdatedEvent(CourseId CourseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseModuleAddedEvent(CourseId CourseId, Guid ModuleId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseModuleRemovedEvent(CourseId CourseId, Guid ModuleId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CoursePublishedEvent(CourseId CourseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseUnpublishedEvent(CourseId CourseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseModulesReorderedEvent(CourseId CourseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseTargetSkillAddedEvent(CourseId CourseId, SkillId SkillId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseTargetSkillRemovedEvent(CourseId CourseId, SkillId SkillId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseReviewAddedEvent(CourseId CourseId, UserId UserId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseCompletionRequirementAddedEvent(
        CourseId CourseId,
        CompletionRequirementId RequirementId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseCompletionRequirementRemovedEvent(
        CourseId CourseId,
        CompletionRequirementId RequirementId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseCompletionRequirementsReorderedEvent(
        CourseId CourseId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseTagAddedEvent(
        CourseId CourseId,
        CourseTagId TagId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseTagRemovedEvent(
        CourseId CourseId,
        CourseTagId TagId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CourseTagsClearedEvent(
        CourseId CourseId,
        IEnumerable<CourseTagId> TagIds) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
