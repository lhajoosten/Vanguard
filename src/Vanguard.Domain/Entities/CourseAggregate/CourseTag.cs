using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.CourseAggregate
{
    public class CourseTag : Entity<CourseTagId>
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;

        // Navigation property for EF Core
        public virtual ICollection<Course> Courses { get; private set; } = new List<Course>();

        private CourseTag() { } // For EF Core

        private CourseTag(CourseTagId id, string name, string description) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Tag name cannot be empty");

            Name = name;
            Description = description ?? string.Empty;
            Slug = GenerateSlug(name);
        }

        public static CourseTag Create(string name, string description = "")
        {
            return new CourseTag(CourseTagId.CreateUnique(), name, description);
        }

        public void Update(string name, string description)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Tag name cannot be empty");

            Name = name;
            Description = description ?? string.Empty;
            Slug = GenerateSlug(name);
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

        private static string GenerateSlug(string name)
        {
            // Simple slug generator - lowercase, replace spaces with hyphens
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            var slug = name.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("&", "and")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace("/", "-")
                .Replace("\\", "-");

            // Remove consecutive hyphens and trim hyphens from beginning and end
            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }

            return slug.Trim('-');
        }
    }
}