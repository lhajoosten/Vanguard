using Vanguard.Domain.Abstraction;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Events
{
    public record SkillAssessedEvent(
         SkillAssessmentId AssessmentId,
         UserId UserId,
         SkillId SkillId,
         ProficiencyLevel Level) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentUpdatedEvent(
        SkillAssessmentId AssessmentId,
        UserId UserId,
        SkillId SkillId,
        ProficiencyLevel Level) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentVerifiedEvent(
        SkillAssessmentId AssessmentId,
        UserId UserId,
        SkillId SkillId,
        UserId VerifiedById) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentVerificationRemovedEvent(
        SkillAssessmentId AssessmentId,
        UserId UserId,
        SkillId SkillId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentResultCreatedEvent(
        SkillAssessmentResultId ResultId,
        UserId UserId,
        SkillId SkillId,
        SkillAssessmentId AssessmentId,
        ProficiencyLevel AssignedLevel) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentResultVerifiedEvent(
        SkillAssessmentResultId ResultId,
        UserId UserId,
        SkillId SkillId,
        SkillAssessmentId AssessmentId,
        UserId VerifiedById) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentQuestionAddedEvent(
        SkillAssessmentQuestionId QuestionId,
        SkillId SkillId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentQuestionUpdatedEvent(
        SkillAssessmentQuestionId QuestionId,
        SkillId SkillId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
