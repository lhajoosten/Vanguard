using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Domain.Entities.SkillAggregate
{
    public class SkillAssessmentAnswer : EntityBase<SkillAssessmentAnswerId>
    {
        private readonly List<SkillAssessmentQuestionOptionId> _selectedOptions = [];

        public UserId UserId { get; private set; } = null!;
        public SkillAssessmentQuestionId QuestionId { get; private set; } = null!;
        public SkillAssessmentId AssessmentId { get; private set; } = null!;
        public string TextAnswer { get; private set; } = string.Empty;
        public int ScoreEarned { get; private set; }
        public DateTime AnsweredAt { get; private set; }
        public IReadOnlyCollection<SkillAssessmentQuestionOptionId> SelectedOptions => _selectedOptions.AsReadOnly();

        // Navigation properties for EF Core
        public virtual User? User { get; private set; }
        public virtual SkillAssessmentQuestion? Question { get; private set; }
        public virtual SkillAssessment? Assessment { get; private set; }

        private SkillAssessmentAnswer() { } // For EF Core

        private SkillAssessmentAnswer(
            SkillAssessmentAnswerId id,
            UserId userId,
            SkillAssessmentQuestionId questionId,
            SkillAssessmentId assessmentId) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(userId, nameof(userId), "User ID cannot be null");
            Guard.Against.Null(questionId, nameof(questionId), "Question ID cannot be null");
            Guard.Against.Null(assessmentId, nameof(assessmentId), "Assessment ID cannot be null");

            UserId = userId;
            QuestionId = questionId;
            AssessmentId = assessmentId;
            AnsweredAt = DateTime.UtcNow;
        }

        public static SkillAssessmentAnswer Create(
            UserId userId,
            SkillAssessmentQuestionId questionId,
            SkillAssessmentId assessmentId)
        {
            return new SkillAssessmentAnswer(
                SkillAssessmentAnswerId.CreateUnique(),
                userId,
                questionId,
                assessmentId);
        }

        public void SetTextAnswer(string textAnswer, SkillAssessmentQuestion question, int score)
        {
            Guard.Against.Null(question, nameof(question));
            Guard.Against.OutOfRange(score, nameof(score), 0, question.PointValue,
                $"Score must be between 0 and {question.PointValue}");

            if (question.Type != QuestionType.OpenText)
            {
                throw new InvalidOperationException("Text answers are only valid for open text questions");
            }

            TextAnswer = textAnswer ?? string.Empty;
            ScoreEarned = score;
            ModifiedAt = DateTime.UtcNow;
        }

        public void SelectOptions(
            List<SkillAssessmentQuestionOptionId> optionIds,
            SkillAssessmentQuestion question)
        {
            Guard.Against.Null(question, nameof(question));
            Guard.Against.Null(optionIds, nameof(optionIds));

            if (question.Type == QuestionType.OpenText)
            {
                throw new InvalidOperationException("Cannot select options for an open text question");
            }

            if (question.Type == QuestionType.SingleChoice && optionIds.Count > 1)
            {
                throw new InvalidOperationException("Can only select one option for single choice questions");
            }

            // Validate that all options belong to the question
            foreach (var optionId in optionIds)
            {
                if (!question.Options.Any(o => o.Id == optionId))
                {
                    throw new ArgumentException($"Option with ID {optionId} does not belong to question {question.Id}");
                }
            }

            _selectedOptions.Clear();
            _selectedOptions.AddRange(optionIds);

            // Calculate score based on selected options
            CalculateScore(question);

            ModifiedAt = DateTime.UtcNow;
        }

        private void CalculateScore(SkillAssessmentQuestion question)
        {
            if (question.Type == QuestionType.OpenText)
            {
                // Score for open text questions is set manually
                return;
            }

            var correctOptions = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
            var selectedOptions = _selectedOptions.ToList();

            if (question.Type == QuestionType.SingleChoice)
            {
                // For single choice, it's all or nothing
                ScoreEarned = selectedOptions.Count == 1 && correctOptions.Contains(selectedOptions[0])
                    ? question.PointValue
                    : 0;
            }
            else // MultipleChoice
            {
                // For multiple choice, calculate partial credit
                if (correctOptions.Count == 0)
                {
                    ScoreEarned = 0;
                    return;
                }

                int correctSelections = selectedOptions.Count(id => correctOptions.Contains(id));
                int incorrectSelections = selectedOptions.Count(id => !correctOptions.Contains(id));

                // Calculate score with penalty for incorrect selections
                double correctRatio = (double)correctSelections / correctOptions.Count;
                double incorrectPenalty = incorrectSelections > 0 ? (double)incorrectSelections / correctOptions.Count : 0;

                // Apply formula: (correct ratio - incorrect penalty) * point value, minimum 0
                double scoreRatio = Math.Max(0, correctRatio - incorrectPenalty);
                ScoreEarned = (int)Math.Round(scoreRatio * question.PointValue);
            }
        }
    }
}