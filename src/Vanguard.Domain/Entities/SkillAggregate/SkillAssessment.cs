using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.SkillAggregate
{
    public class SkillAssessment : AggregateRoot<SkillAssessmentId>
    {
        private readonly List<SkillAssessmentQuestion> _questions = [];
        private readonly List<SkillAssessmentResult> _results = [];

        public UserId UserId { get; private set; } = null!;
        public SkillId SkillId { get; private set; } = null!;
        public ProficiencyLevel Level { get; private set; }
        public DateTime AssessedAt { get; private set; }
        public string Evidence { get; private set; } = string.Empty;
        public bool IsVerified { get; private set; } = false;
        public UserId? VerifiedById { get; private set; } = null;
        public DateTime? VerifiedAt { get; private set; } = null;
        public IReadOnlyCollection<SkillAssessmentQuestion> Questions => _questions.AsReadOnly();
        public IReadOnlyCollection<SkillAssessmentResult> Results => _results.AsReadOnly();

        // Navigation properties for EF Core
        public virtual User? User { get; private set; }
        public virtual Skill? Skill { get; private set; }
        public virtual User? VerifiedBy { get; private set; }

        private SkillAssessment() { } // For EF Core

        private SkillAssessment(
            SkillAssessmentId id,
            UserId userId,
            SkillId skillId,
            ProficiencyLevel level,
            string evidence) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(userId, nameof(userId), "User ID cannot be null");
            Guard.Against.Null(skillId, nameof(skillId), "Skill ID cannot be null");

            UserId = userId;
            SkillId = skillId;
            Level = level;
            Evidence = evidence ?? string.Empty;
            AssessedAt = DateTime.UtcNow;
        }

        public static SkillAssessment Create(
            UserId userId,
            SkillId skillId,
            ProficiencyLevel level,
            string evidence = "")
        {
            var assessment = new SkillAssessment(
                SkillAssessmentId.CreateUnique(),
                userId,
                skillId,
                level,
                evidence);

            assessment.AddDomainEvent(new SkillAssessedEvent(assessment.Id, userId, skillId, level));
            return assessment;
        }

        public void UpdateAssessment(ProficiencyLevel level, string evidence)
        {
            Level = level;
            Evidence = evidence ?? string.Empty;
            AssessedAt = DateTime.UtcNow;

            // If the assessment was previously verified, it needs to be re-verified
            if (IsVerified)
            {
                IsVerified = false;
                VerifiedById = null;
                VerifiedAt = null;
            }

            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new SkillAssessmentUpdatedEvent(Id, UserId, SkillId, Level));
        }

        public void Verify(UserId verifiedById)
        {
            Guard.Against.Null(verifiedById, nameof(verifiedById), "Verifier ID cannot be null");

            // Can't verify your own skill assessment
            if (verifiedById == UserId)
            {
                throw new InvalidOperationException("Users cannot verify their own skill assessments");
            }

            IsVerified = true;
            VerifiedById = verifiedById;
            VerifiedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new SkillAssessmentVerifiedEvent(Id, UserId, SkillId, verifiedById));
        }

        public void RemoveVerification()
        {
            if (!IsVerified)
                return;

            IsVerified = false;
            VerifiedAt = null;
            ModifiedAt = DateTime.UtcNow;

            // Keep the VerifiedById for audit purposes

            AddDomainEvent(new SkillAssessmentVerificationRemovedEvent(Id, UserId, SkillId));
        }
        public SkillAssessmentQuestion AddQuestion(
            string text,
            string explanation,
            QuestionType type,
            DifficultyLevel difficulty,
            int pointValue,
            int orderIndex)
        {
            Guard.Against.NullOrWhiteSpace(text, nameof(text), "Question text cannot be empty");
            Guard.Against.NegativeOrZero(pointValue, nameof(pointValue), "Point value must be positive");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            // If order index is already occupied, shift other questions
            if (_questions.Any(q => q.OrderIndex == orderIndex))
            {
                foreach (var q in _questions.Where(q => q.OrderIndex >= orderIndex).OrderByDescending(q => q.OrderIndex))
                {
                    q.UpdateOrderIndex(q.OrderIndex + 1);
                }
            }

            var question = SkillAssessmentQuestion.Create(
                SkillId,
                text,
                explanation,
                type,
                difficulty,
                pointValue,
                orderIndex);

            _questions.Add(question);
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new SkillAssessmentQuestionAddedEvent(question.Id, SkillId));
            return question;
        }

        public void RemoveQuestion(SkillAssessmentQuestionId questionId)
        {
            Guard.Against.Null(questionId, nameof(questionId));

            var question = _questions.FirstOrDefault(q => q.Id == questionId);
            Guard.Against.Null(question, nameof(question), $"Question with ID {questionId} not found");

            int removedIndex = question.OrderIndex;
            _questions.Remove(question);

            // Reindex remaining questions
            foreach (var q in _questions.Where(q => q.OrderIndex > removedIndex).OrderBy(q => q.OrderIndex))
            {
                q.UpdateOrderIndex(q.OrderIndex - 1);
            }

            ModifiedAt = DateTime.UtcNow;
        }

        public void ReorderQuestions(List<SkillAssessmentQuestionId> questionIds)
        {
            Guard.Against.NullOrEmpty(questionIds, nameof(questionIds), "Question IDs cannot be empty");

            // Ensure all questions exist in the assessment
            foreach (var questionId in questionIds)
            {
                if (!_questions.Any(q => q.Id == questionId))
                {
                    throw new ArgumentException($"Question with ID {questionId} does not exist in this assessment");
                }
            }

            // Reorder questions
            for (int i = 0; i < questionIds.Count; i++)
            {
                var question = _questions.First(q => q.Id == questionIds[i]);
                question.UpdateOrderIndex(i);
            }

            ModifiedAt = DateTime.UtcNow;
        }

        // Methods for managing results
        public SkillAssessmentResult RecordResult(UserId userId, IEnumerable<SkillAssessmentAnswer> answers)
        {
            Guard.Against.Null(userId, nameof(userId));
            Guard.Against.NullOrEmpty(answers, nameof(answers), "Assessment must have at least one answer");

            // Create the result
            var result = SkillAssessmentResult.Create(
                userId,
                SkillId,
                Id,
                answers,
                _questions);

            // Store the result
            _results.Add(result);
            ModifiedAt = DateTime.UtcNow;

            // Update the assessment's proficiency level based on the most recent verified result
            // or the highest scoring result if no verified results exist
            UpdateProficiencyLevel();

            return result;
        }

        private void UpdateProficiencyLevel()
        {
            // First try to find the most recent verified result
            var mostRecentVerifiedResult = _results
                .Where(r => r.IsVerified)
                .OrderByDescending(r => r.VerifiedAt)
                .FirstOrDefault();

            if (mostRecentVerifiedResult != null)
            {
                Level = mostRecentVerifiedResult.AssignedLevel;
                return;
            }

            // If no verified results, use the highest scoring result
            var highestScoringResult = _results
                .OrderByDescending(r => r.ScorePercentage)
                .FirstOrDefault();

            if (highestScoringResult != null)
            {
                Level = highestScoringResult.AssignedLevel;
            }
        }

        // Method to check if assessment has questions
        public bool HasQuestions()
        {
            return _questions.Count != 0;
        }

        // Method to get the maximum possible score
        public int GetMaximumPossibleScore()
        {
            return _questions.Sum(q => q.PointValue);
        }
    }
}
