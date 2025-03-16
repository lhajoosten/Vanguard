using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.SkillAggregate
{
    public class SkillCategory : Entity<SkillCategoryId>
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        // Navigation property for EF Core
        public virtual ICollection<Skill> Skills { get; private set; } = new List<Skill>();

        private SkillCategory() { } // For EF Core

        private SkillCategory(SkillCategoryId id, string name, string description) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Skill category name cannot be empty");
            Guard.Against.NullOrWhiteSpace(description, nameof(description), "Skill category description cannot be empty");

            Name = name;
            Description = description;
        }

        public static SkillCategory Create(string name, string description)
        {
            return new SkillCategory(SkillCategoryId.CreateUnique(), name, description);
        }

        public void Update(string name, string description)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Skill category name cannot be empty");
            Guard.Against.NullOrWhiteSpace(description, nameof(description), "Skill category description cannot be empty");

            Name = name;
            Description = description;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
