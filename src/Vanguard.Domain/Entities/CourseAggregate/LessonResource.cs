using Ardalis.GuardClauses;
using Vanguard.Common.Base;

namespace Vanguard.Domain.Entities.CourseAggregate
{
    public class LessonResource : EntityBase<Guid>
    {
        public string Title { get; private set; } = string.Empty;
        public string Url { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        private LessonResource() { } // For EF Core

        private LessonResource(Guid id, string title, string url, string description) : base(id)
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Resource title cannot be empty");
            Guard.Against.NullOrWhiteSpace(url, nameof(url), "Resource URL cannot be empty");

            Title = title;
            Url = url;
            Description = description;
        }

        public static LessonResource Create(string title, string url, string description = "")
        {
            return new LessonResource(Guid.NewGuid(), title, url, description);
        }

        public void Update(string title, string url, string description)
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Resource title cannot be empty");
            Guard.Against.NullOrWhiteSpace(url, nameof(url), "Resource URL cannot be empty");

            Title = title;
            Url = url;
            Description = description;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
