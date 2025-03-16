using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.SkillAggregate
{
    public class SkillAssessmentQuestionOption : Entity<SkillAssessmentQuestionOptionId>
    {
        public string Text { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; }
        public int OrderIndex { get; private set; }

        private SkillAssessmentQuestionOption() { } // For EF Core

        private SkillAssessmentQuestionOption(
            SkillAssessmentQuestionOptionId id,
            string text,
            bool isCorrect,
            int orderIndex) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(text, nameof(text), "Option text cannot be empty");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            Text = text;
            IsCorrect = isCorrect;
            OrderIndex = orderIndex;
        }

        public static SkillAssessmentQuestionOption Create(
            string text,
            bool isCorrect,
            int orderIndex)
        {
            return new SkillAssessmentQuestionOption(
                SkillAssessmentQuestionOptionId.CreateUnique(),
                text,
                isCorrect,
                orderIndex);
        }

        public void Update(string text)
        {
            Guard.Against.NullOrWhiteSpace(text, nameof(text), "Option text cannot be empty");

            Text = text;
            ModifiedAt = DateTime.UtcNow;
        }

        public void SetCorrect(bool isCorrect)
        {
            IsCorrect = isCorrect;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateOrderIndex(int orderIndex)
        {
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            OrderIndex = orderIndex;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}