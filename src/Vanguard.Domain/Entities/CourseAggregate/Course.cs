using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.EnrollmentAggregate;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.Domain.Entities.CourseAggregate
{
    public class Course : AggregateRootBase<CourseId>
    {
        private readonly List<CourseModule> _modules = [];
        private readonly List<Skill> _skills = [];
        private readonly List<CourseReview> _reviews = [];
        private readonly List<CompletionRequirement> _completionRequirements = [];
        private readonly List<CourseTag> _tags = [];

        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public UserId CreatorId { get; private set; } = null!;
        public string ImageUrl { get; private set; } = string.Empty;
        public ProficiencyLevel Level { get; private set; }
        public int EstimatedDurationMinutes { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public int EnrollmentCount { get; private set; }
        public decimal AverageRating => _reviews.Count != 0 ? (decimal)_reviews.Average(r => r.Rating) : 0;

        public IReadOnlyCollection<CourseModule> Modules => _modules.AsReadOnly();
        public IReadOnlyCollection<Skill> Skills => _skills.AsReadOnly();
        public IReadOnlyCollection<CourseReview> Reviews => _reviews.AsReadOnly();
        public IReadOnlyCollection<CompletionRequirement> CompletionRequirements => _completionRequirements.AsReadOnly();
        public IReadOnlyCollection<CourseTag> Tags => _tags.AsReadOnly();

        // Navigation properties for EF Core
        public virtual User? Creator { get; private set; }
        public virtual ICollection<Enrollment> Enrollments { get; private set; } = [];

        private Course() { } // For EF Core

        private Course(
            CourseId id,
            string title,
            string description,
            UserId creatorId,
            ProficiencyLevel level,
            int estimatedDurationMinutes = 0,
            string imageUrl = "") : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Course title cannot be empty");
            Guard.Against.Null(creatorId, nameof(creatorId), "Creator ID cannot be null");
            Guard.Against.Null(level, nameof(level), "Proficiency level cannot be null");
            Guard.Against.Negative(estimatedDurationMinutes, nameof(estimatedDurationMinutes), "Duration cannot be negative");

            Title = title;
            Description = description;
            CreatorId = creatorId;
            Level = level;
            EstimatedDurationMinutes = estimatedDurationMinutes;
            ImageUrl = imageUrl;
            EnrollmentCount = 0;
        }

        public static Course Create(
            string title,
            string description,
            UserId creatorId,
            int estimatedDurationMinutes = 0,
            string imageUrl = "")
        {
            var course = new Course(
                CourseId.CreateUnique(),
                title,
                description,
                creatorId,
                ProficiencyLevel.Beginner,
                estimatedDurationMinutes,
                imageUrl);

            course.AddDomainEvent(new CourseCreatedEvent(course.Id, creatorId));
            return course;
        }

        public void Update(
            string title,
            string description,
            ProficiencyLevel level,
            int estimatedDurationMinutes,
            string imageUrl)
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Course title cannot be empty");
            Guard.Against.NegativeOrZero(estimatedDurationMinutes, nameof(estimatedDurationMinutes), "Duration cannot be negative");

            Title = title;
            Description = description;
            Level = level;
            EstimatedDurationMinutes = estimatedDurationMinutes;
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? ImageUrl : imageUrl;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CourseUpdatedEvent(Id));
        }

        public CourseModule AddModule(string title, string description, int orderIndex)
        {
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Module title cannot be empty");
            Guard.Against.Negative(orderIndex, nameof(orderIndex), "Order index must be non-negative");

            // If order index is already occupied, shift other modules
            if (_modules.Any(m => m.OrderIndex == orderIndex))
            {
                foreach (var mod in _modules.Where(m => m.OrderIndex >= orderIndex).OrderByDescending(m => m.OrderIndex))
                {
                    mod.UpdateOrderIndex(mod.OrderIndex + 1);
                }
            }

            var module = CourseModule.Create(title, description, orderIndex);
            _modules.Add(module);
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CourseModuleAddedEvent(Id, module.Id));
            return module;
        }

        public void RemoveModule(ModuleId moduleId)
        {
            Guard.Against.Null(moduleId, nameof(moduleId));

            var module = _modules.FirstOrDefault(m => m.Id == moduleId);
            Guard.Against.Null(module, nameof(module), $"Module with ID {moduleId} not found in course");

            int removedIndex = module.OrderIndex;
            _modules.Remove(module);

            // Reindex remaining modules
            foreach (var m in _modules.Where(m => m.OrderIndex > removedIndex).OrderBy(m => m.OrderIndex))
            {
                m.UpdateOrderIndex(m.OrderIndex - 1);
            }

            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CourseModuleRemovedEvent(Id, moduleId));
        }

        public void ReorderModules(List<ModuleId> moduleIds)
        {
            Guard.Against.NullOrEmpty(moduleIds, nameof(moduleIds), "Module IDs cannot be empty");

            // Ensure all modules exist in the course
            foreach (var moduleId in moduleIds)
            {
                if (!_modules.Any(m => m.Id == moduleId))
                {
                    throw new ArgumentException($"Module with ID {moduleId} does not exist in the course");
                }
            }

            // Reorder modules
            for (int i = 0; i < moduleIds.Count; i++)
            {
                var module = _modules.First(m => m.Id == moduleIds[i]);
                module.UpdateOrderIndex(i);
            }

            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CourseModulesReorderedEvent(Id));
        }

        public void Publish()
        {
            Guard.Against.Zero(_modules.Count, nameof(_modules), "Cannot publish a course with no modules");

            if (_modules.Any(m => m.Lessons.Count == 0))
            {
                throw new InvalidOperationException("Cannot publish a course with empty modules");
            }

            PublishedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CoursePublishedEvent(Id));
        }

        public void Unpublish()
        {
            PublishedAt = null;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CourseUnpublishedEvent(Id));
        }

        public void IncrementEnrollmentCount()
        {
            EnrollmentCount++;
            ModifiedAt = DateTime.UtcNow;
        }

        public void DecrementEnrollmentCount()
        {
            if (EnrollmentCount > 0)
            {
                EnrollmentCount--;
                ModifiedAt = DateTime.UtcNow;
            }
        }

        public void AddReview(UserId userId, int rating, string comment)
        {
            Guard.Against.Null(userId, nameof(userId));
            Guard.Against.OutOfRange(rating, nameof(rating), 1, 5, "Rating must be between 1 and 5");

            // Check if user has already reviewed
            var existingReview = _reviews.FirstOrDefault(r => r.UserId == userId);
            if (existingReview != null)
            {
                existingReview.Update(rating, comment);
            }
            else
            {
                var review = CourseReview.Create(userId, rating, comment);
                _reviews.Add(review);
            }

            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CourseReviewAddedEvent(Id, userId));
        }

        public Enrollment EnrollUser(UserId userId)
        {
            Guard.Against.Null(userId, nameof(userId));

            if (PublishedAt == null)
            {
                throw new InvalidOperationException("Cannot enroll in an unpublished course");
            }

            IncrementEnrollmentCount();
            var enrollment = Enrollment.Create(userId, Id);
            AddDomainEvent(new UserEnrolledEvent(Id, userId));

            return enrollment;
        }

        public CompletionRequirement AddCompletionRequirement(
           CompletionCriteria criteria,
           int requiredValue,
           string description = "",
           bool isRequired = true,
           int orderIndex = 0)
        {
            Guard.Against.NegativeOrZero(requiredValue, nameof(requiredValue), "Required value must be positive");

            // If order index is already occupied, shift other requirements
            if (_completionRequirements.Any(r => r.OrderIndex == orderIndex))
            {
                foreach (var req in _completionRequirements
                    .Where(r => r.OrderIndex >= orderIndex)
                    .OrderByDescending(r => r.OrderIndex))
                {
                    req.UpdateOrderIndex(req.OrderIndex + 1);
                }
            }

            var requirement = CompletionRequirement.Create(
                Id,
                criteria,
                requiredValue,
                description,
                isRequired,
                orderIndex);

            _completionRequirements.Add(requirement);
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CourseCompletionRequirementAddedEvent(Id, requirement.Id));
            return requirement;
        }

        public void RemoveCompletionRequirement(CompletionRequirementId requirementId)
        {
            Guard.Against.Null(requirementId, nameof(requirementId));

            var requirement = _completionRequirements.FirstOrDefault(r => r.Id == requirementId);
            Guard.Against.Null(requirement, nameof(requirement), $"Requirement with ID {requirementId} not found in course");

            int removedIndex = requirement.OrderIndex;
            _completionRequirements.Remove(requirement);

            // Reindex remaining requirements
            foreach (var r in _completionRequirements
                .Where(r => r.OrderIndex > removedIndex)
                .OrderBy(r => r.OrderIndex))
            {
                r.UpdateOrderIndex(r.OrderIndex - 1);
            }

            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CourseCompletionRequirementRemovedEvent(Id, requirementId));
        }

        public void ReorderCompletionRequirements(List<CompletionRequirementId> requirementIds)
        {
            Guard.Against.NullOrEmpty(requirementIds, nameof(requirementIds), "Requirement IDs cannot be empty");

            // Ensure all requirements exist in the course
            foreach (var requirementId in requirementIds)
            {
                if (!_completionRequirements.Any(r => r.Id == requirementId))
                {
                    throw new ArgumentException($"Requirement with ID {requirementId} does not exist in the course");
                }
            }

            // Reorder requirements
            for (int i = 0; i < requirementIds.Count; i++)
            {
                var requirement = _completionRequirements.First(r => r.Id == requirementIds[i]);
                requirement.UpdateOrderIndex(i);
            }

            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CourseCompletionRequirementsReorderedEvent(Id));
        }

        public bool CheckEnrollmentCompletion(Enrollment enrollment)
        {
            Guard.Against.Null(enrollment, nameof(enrollment));

            if (enrollment.CourseId != Id)
            {
                throw new InvalidOperationException("Cannot check completion for an enrollment in a different course");
            }

            // If no requirements are set, use default 100% completion
            if (!_completionRequirements.Any())
            {
                return enrollment.ProgressPercentage == 100;
            }

            return enrollment.CheckAllRequirementsSatisfied(_completionRequirements);
        }

        public void AddTargetSkill(Skill skill)
        {
            Guard.Against.Null(skill, nameof(skill));

            if (!_skills.Contains(skill))
            {
                _skills.Add(skill);
                ModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new CourseTargetSkillAddedEvent(Id, skill.Id));
            }
        }

        public void RemoveTargetSkill(Skill skill)
        {
            Guard.Against.Null(skill, nameof(skill));

            if (!_skills.Contains(skill))
            {
                return;
            }
            _skills.Remove(skill);
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new CourseTargetSkillRemovedEvent(Id, skill.Id));
        }

        public void ClearTargetSkills()
        {
            if (_skills.Count != 0)
            {
                var skillIds = _skills.Select(s => s.Id).ToList();
                _skills.Clear();
                ModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new CourseTargetSkillsClearedEvent(Id, skillIds));
            }
        }

        public void AddTag(CourseTag tag)
        {
            Guard.Against.Null(tag, nameof(tag));

            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                ModifiedAt = DateTime.UtcNow;

                AddDomainEvent(new CourseTagAddedEvent(Id, tag.Id));
            }
        }

        public void RemoveTag(CourseTag tag)
        {
            Guard.Against.Null(tag, nameof(tag));

            if (!_tags.Contains(tag))
            {
                return;
            }
            _tags.Remove(tag);
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CourseTagRemovedEvent(Id, tag.Id));
        }

        public void ClearTags()
        {
            if (_tags.Count != 0)
            {
                var tagIds = _tags.Select(t => t.Id).ToList();
                _tags.Clear();
                ModifiedAt = DateTime.UtcNow;

                AddDomainEvent(new CourseTagsClearedEvent(Id, tagIds));
            }
        }
    }
}
