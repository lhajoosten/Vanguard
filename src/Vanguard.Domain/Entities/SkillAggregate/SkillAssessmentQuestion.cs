using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Domain.Entities.SkillAggregate
{
    public class SkillAssessmentQuestion : EntityBase<SkillAssessmentQuestionId>
    {
        private readonly List<SkillAssessmentQuestionOption> _options = [];

        public string Text { get; private set; } = string.Empty;
        public string Explanation { get; private set; } = string.Empty;
        public QuestionType Type { get; private set; }
        public DifficultyLevel Difficulty { get; private set; }
        public int PointValue { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int OrderIndex { get; private set; }
        public SkillId SkillId { get; private set; } = null!;
        public SkillAssessmentId AssessmentId { get; private set; } = null!;
        public IReadOnlyCollection<SkillAssessmentQuestionOption> Options => _options.AsReadOnly();

        // Navigation properties for EF Core
        public virtual Skill? Skill { get; private set; }
        public virtual SkillAssessment? Assessment { get; private set; }

        private SkillAssessmentQuestion() { } // For EF Core

        private SkillAssessmentQuestion(
            SkillAssessmentQuestionId id,
            SkillId skillId,
            string text,
            string explanation,
            QuestionType type,
            DifficultyLevel difficulty,
            int pointValue,
            int orderIndex) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(skillId, nameof(skillId), "Skill ID cannot be null");
            Guard.Against.NullOrWhiteSpace(text, nameof(text), "Question text cannot be empty");
            Guard.Against.NegativeOrZero(pointValue, nameof(pointValue), "Point value must be positive");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            SkillId = skillId;
            Text = text;
            Explanation = explanation ?? string.Empty;
            Type = type;
            Difficulty = difficulty;
            PointValue = pointValue;
            OrderIndex = orderIndex;
        }

        public static SkillAssessmentQuestion Create(
            SkillId skillId,
            string text,
            string explanation,
            QuestionType type,
            DifficultyLevel difficulty,
            int pointValue,
            int orderIndex)
        {
            return new SkillAssessmentQuestion(
                SkillAssessmentQuestionId.CreateUnique(),
                skillId,
                text,
                explanation,
                type,
                difficulty,
                pointValue,
                orderIndex);
        }

        public void Update(
            string text,
            string explanation,
            QuestionType type,
            DifficultyLevel difficulty,
            int pointValue)
        {
            Guard.Against.NullOrWhiteSpace(text, nameof(text), "Question text cannot be empty");
            Guard.Against.NegativeOrZero(pointValue, nameof(pointValue), "Point value must be positive");

            Text = text;
            Explanation = explanation ?? string.Empty;
            Type = type;
            Difficulty = difficulty;
            PointValue = pointValue;
            ModifiedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                ModifiedAt = DateTime.UtcNow;
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                ModifiedAt = DateTime.UtcNow;
            }
        }

        public void UpdateOrderIndex(int orderIndex)
        {
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            OrderIndex = orderIndex;
            ModifiedAt = DateTime.UtcNow;
        }

        public SkillAssessmentQuestionOption AddOption(
            string text,
            bool isCorrect,
            int orderIndex)
        {
            Guard.Against.NullOrWhiteSpace(text, nameof(text), "Option text cannot be empty");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            // For single choice questions, ensure only one correct answer
            if (Type == QuestionType.SingleChoice && isCorrect)
            {
                foreach (var opt in _options.Where(o => o.IsCorrect))
                {
                    opt.SetCorrect(false);
                }
            }

            var option = SkillAssessmentQuestionOption.Create(text, isCorrect, orderIndex);
            _options.Add(option);
            ModifiedAt = DateTime.UtcNow;

            return option;
        }

        public void RemoveOption(SkillAssessmentQuestionOptionId optionId)
        {
            Guard.Against.Null(optionId, nameof(optionId));

            var option = _options.FirstOrDefault(o => o.Id == optionId);
            if (option == null)
                return;

            _options.Remove(option);
            ModifiedAt = DateTime.UtcNow;
        }

        public void ReorderOptions(List<SkillAssessmentQuestionOptionId> optionIds)
        {
            Guard.Against.NullOrEmpty(optionIds, nameof(optionIds), "Option IDs cannot be empty");

            // Ensure all options exist in the question
            foreach (var optionId in optionIds)
            {
                if (!_options.Any(o => o.Id == optionId))
                {
                    throw new ArgumentException($"Option with ID {optionId} does not exist in this question");
                }
            }

            // Reorder options
            for (int i = 0; i < optionIds.Count; i++)
            {
                var option = _options.First(o => o.Id == optionIds[i]);
                option.UpdateOrderIndex(i);
            }

            ModifiedAt = DateTime.UtcNow;
        }
    }
}
