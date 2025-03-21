using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.Domain.Entities.SkillAggregate
{
    public class Skill : AggregateRootBase<SkillId>
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public SkillCategoryId CategoryId { get; private set; } = null!;

        // Navigation property for EF Core
        public virtual SkillCategory? Category { get; private set; }
        public virtual ICollection<SkillAssessment> Assessments { get; private set; } = [];

        private Skill() { } // For EF Core

        private Skill(SkillId id, string name, string description, SkillCategory category) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Skill name cannot be empty");
            Guard.Against.NullOrWhiteSpace(description, nameof(description), "Skill description cannot be empty");
            Guard.Against.Null(category, nameof(category), "Skill category cannot be null");

            Name = name;
            Description = description;
            CategoryId = category.Id;
            Category = category;
        }

        public static Skill Create(string name, string description, SkillCategory category)
        {
            var skill = new Skill(SkillId.CreateUnique(), name, description, category);
            skill.AddDomainEvent(new SkillCreatedEvent(skill.Id));
            return skill;
        }

        public void Update(string name, string description, SkillCategory category)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Skill name cannot be empty");
            Guard.Against.NullOrWhiteSpace(description, nameof(description), "Skill description cannot be empty");
            Guard.Against.Null(category, nameof(category), "Skill category cannot be null");

            Name = name;
            Description = description;
            CategoryId = category.Id;
            Category = category;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new SkillUpdatedEvent(Id));
        }

        public SkillAssessment AssessFor(User user, ProficiencyLevel level, string evidence = "")
        {
            Guard.Against.Null(user, nameof(user));

            var assessment = SkillAssessment.Create(user.Id, Id, level, evidence);
            // The actual collection management would happen in the repository

            AddDomainEvent(new SkillAssessedForUserEvent(Id, user.Id, assessment.Id));
            return assessment;
        }
    }
}
