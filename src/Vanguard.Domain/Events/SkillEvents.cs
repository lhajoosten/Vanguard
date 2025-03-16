using Vanguard.Domain.Abstraction;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Events
{
    public record SkillCreatedEvent(SkillId SkillId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillUpdatedEvent(SkillId SkillId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessedForUserEvent(
        SkillId SkillId,
        UserId UserId,
        SkillAssessmentId AssessmentId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
