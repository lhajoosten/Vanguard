using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Domain.Entities.CourseAggregate
{
    public class CourseModule : EntityBase<ModuleId>
    {
        private readonly List<Lesson> _lessons = [];

        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int OrderIndex { get; private set; }
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
        public int EstimatedDurationMinutes => _lessons.Sum(l => l.DurationMinutes);

        // Navigation properties for EF Core
        public CourseId CourseId { get; private set; } = null!;
        public virtual Course? Course { get; private set; }

        private CourseModule() { } // For EF Core

        private CourseModule(ModuleId id, string title, string description, int orderIndex) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Module title cannot be empty");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            Title = title;
            Description = description;
            OrderIndex = orderIndex;
        }

        public static CourseModule Create(string title, string description, int orderIndex)
        {
            return new CourseModule(ModuleId.CreateUnique(), title, description, orderIndex);
        }

        public void Update(string title, string description)
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Module title cannot be empty");

            Title = title;
            Description = description;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateOrderIndex(int orderIndex)
        {
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            OrderIndex = orderIndex;
            ModifiedAt = DateTime.UtcNow;
        }

        public Lesson AddLesson(
            string title,
            string description,
            LessonType type,
            string content,
            int orderIndex,
            int durationMinutes = 0)
        {
            Guard.Against.Null(type, nameof(type), "Lesson type cannot be null");
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Lesson title cannot be empty");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");
            Guard.Against.Negative(durationMinutes, nameof(durationMinutes), "Duration cannot be negative");

            // If order index is already occupied, shift other lessons
            if (_lessons.Any(l => l.OrderIndex == orderIndex))
            {
                foreach (var les in _lessons.Where(l => l.OrderIndex >= orderIndex).OrderByDescending(l => l.OrderIndex))
                {
                    les.UpdateOrderIndex(les.OrderIndex + 1);
                }
            }

            var lesson = Lesson.Create(title, description, type, content, orderIndex, durationMinutes);
            _lessons.Add(lesson);
            ModifiedAt = DateTime.UtcNow;

            return lesson;
        }

        public void RemoveLesson(LessonId lessonId)
        {
            Guard.Against.Null(lessonId, nameof(lessonId));

            var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
            Guard.Against.Null(lesson, nameof(lesson), $"Lesson with ID {lessonId} not found in module");

            int removedIndex = lesson.OrderIndex;
            _lessons.Remove(lesson);

            // Reindex remaining lessons
            foreach (var l in _lessons.Where(l => l.OrderIndex > removedIndex).OrderBy(l => l.OrderIndex))
            {
                l.UpdateOrderIndex(l.OrderIndex - 1);
            }

            ModifiedAt = DateTime.UtcNow;
        }

        public void ReorderLessons(List<LessonId> lessonIds)
        {
            Guard.Against.NullOrEmpty(lessonIds, nameof(lessonIds), "Lesson IDs cannot be empty");

            // Ensure all lessons exist in the module
            foreach (var lessonId in lessonIds)
            {
                if (!_lessons.Any(l => l.Id == lessonId))
                {
                    throw new ArgumentException($"Lesson with ID {lessonId} does not exist in the module");
                }
            }

            // Reorder lessons
            for (int i = 0; i < lessonIds.Count; i++)
            {
                var lesson = _lessons.First(l => l.Id == lessonIds[i]);
                lesson.UpdateOrderIndex(i);
            }

            ModifiedAt = DateTime.UtcNow;
        }
    }
}
