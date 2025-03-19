using Vanguard.Common.Abstractions;
using Vanguard.Common.Base;

namespace Vanguard.Domain.Events
{
    public record SkillCreatedEvent(SkillId SkillId) : IDomainEvent
    {
        public Guid Id { get; } = SkillId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillUpdatedEvent(SkillId SkillId) : IDomainEvent
    {
        public Guid Id { get; } = SkillId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessedForUserEvent(
        SkillId SkillId,
        UserId UserId,
        SkillAssessmentId AssessmentId) : IDomainEvent
    {
        public Guid Id { get; } = SkillId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
