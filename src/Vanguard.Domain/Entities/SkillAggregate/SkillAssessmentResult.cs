using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.SkillAggregate
{
    public class SkillAssessmentResult : AggregateRoot<SkillAssessmentResultId>
    {
        private readonly List<SkillAssessmentAnswer> _answers = [];

        public UserId UserId { get; private set; } = null!;
        public SkillId SkillId { get; private set; } = null!;
        public SkillAssessmentId AssessmentId { get; private set; } = null!;
        public int TotalScore { get; private set; }
        public int MaximumPossibleScore { get; private set; }
        public double ScorePercentage => MaximumPossibleScore > 0
            ? (double)TotalScore / MaximumPossibleScore * 100
            : 0;
        public ProficiencyLevel AssignedLevel { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public bool IsVerified { get; private set; } = false;
        public UserId? VerifiedById { get; private set; } = null;
        public DateTime? VerifiedAt { get; private set; } = null;
        public string FeedbackNotes { get; private set; } = string.Empty;

        public IReadOnlyCollection<SkillAssessmentAnswer> Answers => _answers.AsReadOnly();

        // Navigation properties for EF Core
        public virtual User? User { get; private set; }
        public virtual Skill? Skill { get; private set; }
        public virtual SkillAssessment? Assessment { get; private set; }
        public virtual User? VerifiedBy { get; private set; }

        private SkillAssessmentResult() { } // For EF Core

        private SkillAssessmentResult(
            SkillAssessmentResultId id,
            UserId userId,
            SkillId skillId,
            SkillAssessmentId assessmentId,
            int totalScore,
            int maximumPossibleScore,
            ProficiencyLevel assignedLevel) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(userId, nameof(userId), "User ID cannot be null");
            Guard.Against.Null(skillId, nameof(skillId), "Skill ID cannot be null");
            Guard.Against.Null(assessmentId, nameof(assessmentId), "Assessment ID cannot be null");
            Guard.Against.Negative(totalScore, nameof(totalScore), "Total score cannot be negative");
            Guard.Against.NegativeOrZero(maximumPossibleScore, nameof(maximumPossibleScore), "Maximum score must be positive");
            Guard.Against.OutOfRange(totalScore, nameof(totalScore), 0, maximumPossibleScore,
                $"Total score must be between 0 and {maximumPossibleScore}");

            UserId = userId;
            SkillId = skillId;
            AssessmentId = assessmentId;
            TotalScore = totalScore;
            MaximumPossibleScore = maximumPossibleScore;
            AssignedLevel = assignedLevel;
            CompletedAt = DateTime.UtcNow;
        }

        public static SkillAssessmentResult Create(
            UserId userId,
            SkillId skillId,
            SkillAssessmentId assessmentId,
            IEnumerable<SkillAssessmentAnswer> answers,
            IEnumerable<SkillAssessmentQuestion> questions)
        {
            Guard.Against.Null(answers, nameof(answers));
            Guard.Against.Null(questions, nameof(questions));

            var answersList = answers.ToList();
            var questionsList = questions.ToList();

            // Calculate scores
            int totalScore = answersList.Sum(a => a.ScoreEarned);
            int maximumPossibleScore = questionsList.Sum(q => q.PointValue);

            // Determine proficiency level based on score percentage
            double scorePercentage = maximumPossibleScore > 0
                ? (double)totalScore / maximumPossibleScore * 100
                : 0;

            ProficiencyLevel assignedLevel = DetermineLevel(scorePercentage);

            var result = new SkillAssessmentResult(
                SkillAssessmentResultId.CreateUnique(),
                userId,
                skillId,
                assessmentId,
                totalScore,
                maximumPossibleScore,
                assignedLevel);

            // Add answers to result
            foreach (var answer in answersList)
            {
                result._answers.Add(answer);
            }

            result.AddDomainEvent(new SkillAssessmentResultCreatedEvent(
                result.Id,
                userId,
                skillId,
                assessmentId,
                assignedLevel));

            return result;
        }

        public void Verify(UserId verifiedById, string feedbackNotes = "")
        {
            Guard.Against.Null(verifiedById, nameof(verifiedById), "Verifier ID cannot be null");

            // Can't verify your own result
            if (verifiedById == UserId)
            {
                throw new InvalidOperationException("Users cannot verify their own skill assessment results");
            }

            IsVerified = true;
            VerifiedById = verifiedById;
            VerifiedAt = DateTime.UtcNow;
            FeedbackNotes = feedbackNotes ?? string.Empty;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new SkillAssessmentResultVerifiedEvent(
                Id,
                UserId,
                SkillId,
                AssessmentId,
                verifiedById));
        }

        public void AddFeedback(string feedbackNotes)
        {
            FeedbackNotes = feedbackNotes ?? string.Empty;
            ModifiedAt = DateTime.UtcNow;
        }

        private static ProficiencyLevel DetermineLevel(double scorePercentage)
        {
            return scorePercentage switch
            {
                >= 90 => ProficiencyLevel.Expert,
                >= 75 => ProficiencyLevel.Advanced,
                >= 50 => ProficiencyLevel.Intermediate,
                _ => ProficiencyLevel.Beginner
            };
        }
    }
}