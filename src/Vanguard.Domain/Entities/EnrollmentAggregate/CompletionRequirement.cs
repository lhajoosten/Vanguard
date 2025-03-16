using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.EnrollmentAggregate
{
    public class CompletionRequirement : Entity<CompletionRequirementId>
    {
        public CourseId CourseId { get; private set; } = null!;
        public CompletionCriteria Criteria { get; private set; }
        public int RequiredValue { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public bool IsRequired { get; private set; } = true;
        public int OrderIndex { get; private set; }

        // Navigation properties for EF Core
        public virtual Course? Course { get; private set; }

        private CompletionRequirement() { } // For EF Core

        private CompletionRequirement(
            CompletionRequirementId id,
            CourseId courseId,
            CompletionCriteria criteria,
            int requiredValue,
            string description,
            bool isRequired,
            int orderIndex) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(courseId, nameof(courseId), "Course ID cannot be null");
            Guard.Against.NegativeOrZero(requiredValue, nameof(requiredValue), "Required value must be positive");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            CourseId = courseId;
            Criteria = criteria;
            RequiredValue = requiredValue;
            Description = description ?? string.Empty;
            IsRequired = isRequired;
            OrderIndex = orderIndex;
        }

        public static CompletionRequirement Create(
            CourseId courseId,
            CompletionCriteria criteria,
            int requiredValue,
            string description = "",
            bool isRequired = true,
            int orderIndex = 0)
        {
            return new CompletionRequirement(
                CompletionRequirementId.CreateUnique(),
                courseId,
                criteria,
                requiredValue,
                description,
                isRequired,
                orderIndex);
        }

        public void Update(
            CompletionCriteria criteria,
            int requiredValue,
            string description,
            bool isRequired)
        {
            Guard.Against.NegativeOrZero(requiredValue, nameof(requiredValue), "Required value must be positive");

            Criteria = criteria;
            RequiredValue = requiredValue;
            Description = description ?? string.Empty;
            IsRequired = isRequired;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateOrderIndex(int orderIndex)
        {
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            OrderIndex = orderIndex;
            ModifiedAt = DateTime.UtcNow;
        }

        public bool IsSatisfiedBy(Enrollment enrollment)
        {
            Guard.Against.Null(enrollment, nameof(enrollment));

            if (enrollment.CourseId != CourseId)
            {
                throw new InvalidOperationException("Cannot evaluate enrollment for a different course");
            }

            return Criteria.IsSatisfiedBy(enrollment, RequiredValue);
        }
    }
}
