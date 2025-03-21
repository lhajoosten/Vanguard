using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Domain.Entities.CourseAggregate
{
    public class Lesson : EntityBase<LessonId>
    {
        private readonly List<LessonResource> _resources = [];

        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public LessonType Type { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public int OrderIndex { get; private set; }
        public int DurationMinutes { get; private set; }
        public ModuleId ModuleId { get; private set; } = null!;

        // Navigation properties for EF Core
        public virtual CourseModule? Module { get; private set; }
        public virtual IReadOnlyCollection<LessonResource> Resources => _resources.AsReadOnly();

        private Lesson() { } // For EF Core

        private Lesson(
            LessonId id,
            string title,
            string description,
            LessonType type,
            string content,
            int orderIndex,
            int durationMinutes) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Lesson title cannot be empty");
            Guard.Against.Null(type, nameof(type), "Lesson type cannot be null");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");
            Guard.Against.Negative(durationMinutes, nameof(durationMinutes), "Duration cannot be negative");

            Title = title;
            Description = description;
            Type = type;
            Content = content;
            OrderIndex = orderIndex;
            DurationMinutes = durationMinutes;
        }

        public static Lesson Create(
            string title,
            string description,
            LessonType type,
            string content,
            int orderIndex,
            int durationMinutes = 0)
        {
            return new Lesson(
                LessonId.CreateUnique(),
                title,
                description,
                type,
                content,
                orderIndex,
                durationMinutes);
        }

        public void Update(
            string title,
            string description,
            LessonType type,
            string content,
            int durationMinutes)
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Lesson title cannot be empty");
            Guard.Against.Negative(durationMinutes, nameof(durationMinutes), "Duration cannot be negative");

            Title = title;
            Description = description;
            Type = type;
            Content = content;
            DurationMinutes = durationMinutes;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateOrderIndex(int orderIndex)
        {
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            OrderIndex = orderIndex;
            ModifiedAt = DateTime.UtcNow;
        }

        public void AddResource(string title, string url, string description = "")
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Resource title cannot be empty");
            Guard.Against.NullOrWhiteSpace(url, nameof(url), "Resource URL cannot be empty");

            var resource = LessonResource.Create(title, url, description);
            _resources.Add(resource);
            ModifiedAt = DateTime.UtcNow;
        }

        public void RemoveResource(Guid resourceId)
        {
            var resource = _resources.FirstOrDefault(r => r.Id == resourceId);
            Guard.Against.Null(resource, nameof(resource), $"Resource with ID {resourceId} not found");

            _resources.Remove(resource);
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
