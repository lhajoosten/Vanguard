using Vanguard.Common.Abstractions;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Domain.Events
{
    public record SkillAssessedEvent(
         SkillAssessmentId AssessmentId,
         UserId UserId,
         SkillId SkillId,
         ProficiencyLevel Level) : IDomainEvent
    {
        public Guid Id { get; } = AssessmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentUpdatedEvent(
        SkillAssessmentId AssessmentId,
        UserId UserId,
        SkillId SkillId,
        ProficiencyLevel Level) : IDomainEvent
    {
        public Guid Id { get; } = AssessmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentVerifiedEvent(
        SkillAssessmentId AssessmentId,
        UserId UserId,
        SkillId SkillId,
        UserId VerifiedById) : IDomainEvent
    {
        public Guid Id { get; } = AssessmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentVerificationRemovedEvent(
        SkillAssessmentId AssessmentId,
        UserId UserId,
        SkillId SkillId) : IDomainEvent
    {
        public Guid Id { get; } = AssessmentId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentResultCreatedEvent(
        SkillAssessmentResultId ResultId,
        UserId UserId,
        SkillId SkillId,
        SkillAssessmentId AssessmentId,
        ProficiencyLevel AssignedLevel) : IDomainEvent
    {
        public Guid Id { get; } = ResultId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentResultVerifiedEvent(
        SkillAssessmentResultId ResultId,
        UserId UserId,
        SkillId SkillId,
        SkillAssessmentId AssessmentId,
        UserId VerifiedById) : IDomainEvent
    {
        public Guid Id { get; } = ResultId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentQuestionAddedEvent(
        SkillAssessmentQuestionId QuestionId,
        SkillId SkillId) : IDomainEvent
    {
        public Guid Id { get; } = QuestionId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record SkillAssessmentQuestionUpdatedEvent(
        SkillAssessmentQuestionId QuestionId,
        SkillId SkillId) : IDomainEvent
    {
        public Guid Id { get; } = QuestionId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
