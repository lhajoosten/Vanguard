using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.EnrollmentAggregate;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.UnitTests.Aggregates
{
    public class CourseTests
    {
        private readonly UserId _creatorId = UserId.CreateUnique();

        [Fact]
        public void Create_ShouldCreateNewCourse_WithCorrectProperties()
        {
            // Arrange
            string title = "Test Course";
            string description = "Test Description";
            int duration = 120;
            string imageUrl = "https://example.com/image.jpg";

            // Act
            var course = Course.Create(
                title,
                description,
                _creatorId,
                duration,
                imageUrl);

            // Assert
            Assert.NotNull(course);
            Assert.Equal(title, course.Title);
            Assert.Equal(description, course.Description);
            Assert.Equal(_creatorId, course.CreatorId);
            Assert.Equal(duration, course.EstimatedDurationMinutes);
            Assert.Equal(imageUrl, course.ImageUrl);
            Assert.Equal(ProficiencyLevel.Beginner, course.Level);
            Assert.Equal(0, course.EnrollmentCount);
            Assert.False(course.PublishedAt.HasValue);

            // Verify domain event
            var domainEvent = Assert.Single(course.DomainEvents);
            var courseCreatedEvent = Assert.IsType<CourseCreatedEvent>(domainEvent);
            Assert.Equal(course.Id, courseCreatedEvent.CourseId);
            Assert.Equal(_creatorId, courseCreatedEvent.CreatorId);
        }

        [Fact]
        public void Create_ShouldSucceed_WithMinimalParameters()
        {
            // Arrange & Act
            var course = Course.Create(
                "Minimal Course",
                "Description",
                _creatorId);

            // Assert
            Assert.NotNull(course);
            Assert.Equal("Minimal Course", course.Title);
            Assert.Equal(0, course.EstimatedDurationMinutes);
            Assert.Equal(string.Empty, course.ImageUrl);
        }

        [Theory]
        [InlineData("", "Description")]
        [InlineData("  ", "Description")]
        [InlineData(null, "Description")]
        public void Create_ShouldThrowExceptionForInvalidTitle(string title, string description)
        {
            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                Course.Create(title, description, _creatorId));

            if (title == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("title", nullException.ParamName);
            }
            else
            {
                Assert.Contains("title", exception.Message, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void Create_ShouldThrowArgumentException_WhenCreatorIdIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                Course.Create("Title", "Description", null));
        }

        [Fact]
        public void Update_ShouldUpdateCourseProperties()
        {
            // Arrange
            var course = CreateTestCourse();
            string newTitle = "Updated Course";
            string newDescription = "Updated Description";
            var newLevel = ProficiencyLevel.Advanced;
            int newDuration = 240;
            string newImageUrl = "https://example.com/new-image.jpg";

            // Act
            course.Update(newTitle, newDescription, newLevel, newDuration, newImageUrl);

            // Assert
            Assert.Equal(newTitle, course.Title);
            Assert.Equal(newDescription, course.Description);
            Assert.Equal(newLevel, course.Level);
            Assert.Equal(newDuration, course.EstimatedDurationMinutes);
            Assert.Equal(newImageUrl, course.ImageUrl);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var courseUpdatedEvent = Assert.IsType<CourseUpdatedEvent>(domainEvent);
            Assert.Equal(course.Id, courseUpdatedEvent.CourseId);
        }

        [Fact]
        public void Update_ShouldNotChangeImageUrl_WhenNewImageUrlIsEmpty()
        {
            // Arrange
            var course = CreateTestCourse();
            string originalImage = course.ImageUrl;

            // Act
            course.Update(
                "Updated Course",
                "Updated Description",
                ProficiencyLevel.Intermediate,
                180,
                "");

            // Assert
            Assert.Equal(originalImage, course.ImageUrl);
        }

        [Theory]
        [InlineData("", "Description")]
        [InlineData("  ", "Description")]
        [InlineData(null, "Description")]
        public void Update_ShouldThrowExceptionForInvalidTitle(string title, string description)
        {
            // Arrange
            var course = CreateTestCourse();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                course.Update(title, description, ProficiencyLevel.Beginner, 120, ""));

            if (title == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("title", nullException.ParamName);
            }
            else
            {
                Assert.Contains("title", exception.Message, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Update_ShouldThrowArgumentException_WhenDurationIsZeroOrNegative(int duration)
        {
            // Arrange
            var course = CreateTestCourse();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                course.Update("Title", "Description", ProficiencyLevel.Beginner, duration, ""));

            Assert.Contains("Duration cannot be negative", exception.Message);
        }

        [Fact]
        public void AddModule_ShouldAddNewModule_WithCorrectProperties()
        {
            // Arrange
            var course = CreateTestCourse();
            string moduleTitle = "Test Module";
            string moduleDescription = "Module Description";
            int orderIndex = 0;

            // Act
            var module = course.AddModule(moduleTitle, moduleDescription, orderIndex);

            // Assert
            Assert.NotNull(module);
            Assert.Equal(moduleTitle, module.Title);
            Assert.Equal(moduleDescription, module.Description);
            Assert.Equal(orderIndex, module.OrderIndex);
            Assert.Single(course.Modules);
            Assert.Contains(module, course.Modules);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var moduleAddedEvent = Assert.IsType<CourseModuleAddedEvent>(domainEvent);
            Assert.Equal(course.Id, moduleAddedEvent.CourseId);
            Assert.Equal(module.Id, moduleAddedEvent.ModuleId);
        }

        [Fact]
        public void AddModule_ShouldShiftExistingModules_WhenOrderIndexIsOccupied()
        {
            // Arrange
            var course = CreateTestCourse();
            var module1 = course.AddModule("Module 1", "Description 1", 0);
            var module2 = course.AddModule("Module 2", "Description 2", 1);

            // Act - Add module at index 0 (should shift others)
            var newModule = course.AddModule("New Module", "New Description", 0);

            // Assert
            Assert.Equal(3, course.Modules.Count);
            Assert.Equal(0, newModule.OrderIndex);
            Assert.Equal(1, module1.OrderIndex);
            Assert.Equal(2, module2.OrderIndex);
        }

        [Theory]
        [InlineData("", "Description")]
        [InlineData("  ", "Description")]
        [InlineData(null, "Description")]
        public void AddModule_ShouldThrowArgumentExceptionOrArgumentNullException_WhenTitleIsInvalid(string? title, string description)
        {
            // Arrange
            var course = CreateTestCourse();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                course.AddModule(title!, description, 0));

            // This works because ArgumentNullException inherits from ArgumentException
            if (title == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("title", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Module title cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        public void AddModule_ShouldThrowArgumentException_WhenOrderIndexIsNegative(int orderIndex)
        {
            // Arrange
            var course = CreateTestCourse();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                course.AddModule("Title", "Description", orderIndex));

            Assert.Contains("Order index must be non-negative", exception.Message);
        }

        [Fact]
        public void RemoveModule_ShouldRemoveModule_WhenModuleExists()
        {
            // Arrange
            var course = CreateTestCourse();
            var module = course.AddModule("Test Module", "Description", 0);

            // Act
            course.RemoveModule(module.Id);

            // Assert
            Assert.Empty(course.Modules);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var moduleRemovedEvent = Assert.IsType<CourseModuleRemovedEvent>(domainEvent);
            Assert.Equal(course.Id, moduleRemovedEvent.CourseId);
            Assert.Equal(module.Id, moduleRemovedEvent.ModuleId);
        }

        [Fact]
        public void RemoveModule_ShouldReindexRemainingModules()
        {
            // Arrange
            var course = CreateTestCourse();
            var module1 = course.AddModule("Module 1", "Description 1", 0);
            var module2 = course.AddModule("Module 2", "Description 2", 1);
            var module3 = course.AddModule("Module 3", "Description 3", 2);

            // Act - Remove middle module
            course.RemoveModule(module2.Id);

            // Assert
            Assert.Equal(2, course.Modules.Count);
            Assert.Equal(0, module1.OrderIndex);
            Assert.Equal(1, module3.OrderIndex);
        }

        [Fact]
        public void RemoveModule_ShouldThrowException_WhenModuleDoesNotExist()
        {
            // Arrange
            var course = CreateTestCourse();
            var nonExistentModuleId = ModuleId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                course.RemoveModule(nonExistentModuleId));
        }

        [Fact]
        public void ReorderModules_ShouldUpdateModuleOrderIndices()
        {
            // Arrange
            var course = CreateTestCourse();
            var module1 = course.AddModule("Module 1", "Description 1", 0);
            var module2 = course.AddModule("Module 2", "Description 2", 1);
            var module3 = course.AddModule("Module 3", "Description 3", 2);

            // New order: module3, module1, module2
            var newOrder = new List<ModuleId> { module3.Id, module1.Id, module2.Id };

            // Act
            course.ReorderModules(newOrder);

            // Assert
            Assert.Equal(0, module3.OrderIndex);
            Assert.Equal(1, module1.OrderIndex);
            Assert.Equal(2, module2.OrderIndex);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var modulesReorderedEvent = Assert.IsType<CourseModulesReorderedEvent>(domainEvent);
            Assert.Equal(course.Id, modulesReorderedEvent.CourseId);
        }

        [Fact]
        public void ReorderModules_ShouldThrowException_WhenModuleIdsListIsEmpty()
        {
            // Arrange
            var course = CreateTestCourse();
            course.AddModule("Test Module", "Description", 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                course.ReorderModules(new List<ModuleId>()));
        }

        [Fact]
        public void ReorderModules_ShouldThrowException_WhenModuleDoesNotExist()
        {
            // Arrange
            var course = CreateTestCourse();
            var module = course.AddModule("Test Module", "Description", 0);
            var nonExistentModuleId = ModuleId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                course.ReorderModules(new List<ModuleId> { module.Id, nonExistentModuleId }));
        }

        [Fact]
        public void Publish_ShouldSetPublishedAtTimestamp()
        {
            // Arrange
            var course = CreateTestCourse();
            var module = course.AddModule("Test Module", "Description", 0);
            var lesson = module.AddLesson("Test Lesson", "Content", LessonType.Video, "Content of test lesson 1", 0);

            // Act
            course.Publish();

            // Assert
            Assert.NotNull(course.PublishedAt);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var publishedEvent = Assert.IsType<CoursePublishedEvent>(domainEvent);
            Assert.Equal(course.Id, publishedEvent.CourseId);
        }

        [Fact]
        public void Publish_ShouldThrowException_WhenCourseHasNoModules()
        {
            // Arrange
            var course = CreateTestCourse();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => course.Publish());
            Assert.Contains("Cannot publish a course with no modules", exception.Message);
        }

        [Fact]
        public void Publish_ShouldThrowException_WhenModuleHasNoLessons()
        {
            // Arrange
            var course = CreateTestCourse();
            course.AddModule("Empty Module", "Description", 0);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => course.Publish());
            Assert.Contains("Cannot publish a course with empty modules", exception.Message);
        }

        [Fact]
        public void Unpublish_ShouldClearPublishedAtTimestamp()
        {
            // Arrange
            var course = CreateTestCourse();
            var module = course.AddModule("Test Module", "Description", 0);
            var lesson = module.AddLesson("Test Lesson", "Content", LessonType.Video, "Content of test lesson 1", 0);
            course.Publish();

            // Act
            course.Unpublish();

            // Assert
            Assert.Null(course.PublishedAt);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var unpublishedEvent = Assert.IsType<CourseUnpublishedEvent>(domainEvent);
            Assert.Equal(course.Id, unpublishedEvent.CourseId);
        }

        [Fact]
        public void IncrementEnrollmentCount_ShouldIncrementTheCounter()
        {
            // Arrange
            var course = CreateTestCourse();
            int initialCount = course.EnrollmentCount;

            // Act
            course.IncrementEnrollmentCount();

            // Assert
            Assert.Equal(initialCount + 1, course.EnrollmentCount);
        }

        [Fact]
        public void DecrementEnrollmentCount_ShouldDecrementTheCounter_WhenCountIsPositive()
        {
            // Arrange
            var course = CreateTestCourse();
            course.IncrementEnrollmentCount();
            int initialCount = course.EnrollmentCount;

            // Act
            course.DecrementEnrollmentCount();

            // Assert
            Assert.Equal(initialCount - 1, course.EnrollmentCount);
        }

        [Fact]
        public void DecrementEnrollmentCount_ShouldNotDecrementTheCounter_WhenCountIsZero()
        {
            // Arrange
            var course = CreateTestCourse();
            Assert.Equal(0, course.EnrollmentCount); // Verify initial count is 0

            // Act
            course.DecrementEnrollmentCount();

            // Assert
            Assert.Equal(0, course.EnrollmentCount);
        }

        [Fact]
        public void AddReview_ShouldAddNewReview_WhenUserHasNotReviewed()
        {
            // Arrange
            var course = CreateTestCourse();
            var userId = UserId.CreateUnique();
            int rating = 4;
            string comment = "Great course!";

            // Act
            course.AddReview(userId, rating, comment);

            // Assert
            Assert.Single(course.Reviews);
            var review = course.Reviews.First();
            Assert.Equal(userId, review.UserId);
            Assert.Equal(rating, review.Rating);
            Assert.Equal(comment, review.Comment);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var reviewAddedEvent = Assert.IsType<CourseReviewAddedEvent>(domainEvent);
            Assert.Equal(course.Id, reviewAddedEvent.CourseId);
            Assert.Equal(userId, reviewAddedEvent.UserId);
        }

        [Fact]
        public void AddReview_ShouldUpdateExistingReview_WhenUserHasAlreadyReviewed()
        {
            // Arrange
            var course = CreateTestCourse();
            var userId = UserId.CreateUnique();
            course.AddReview(userId, 3, "Initial review");

            // Act
            course.AddReview(userId, 5, "Updated review");

            // Assert
            Assert.Single(course.Reviews);
            var review = course.Reviews.First();
            Assert.Equal(5, review.Rating);
            Assert.Equal("Updated review", review.Comment);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void AddReview_ShouldThrowException_WhenRatingIsOutOfRange(int rating)
        {
            // Arrange
            var course = CreateTestCourse();
            var userId = UserId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                course.AddReview(userId, rating, "Invalid rating"));
        }

        [Fact]
        public void EnrollUser_ShouldCreateEnrollment_WhenCourseIsPublished()
        {
            // Arrange
            var course = CreateTestCourse();
            var module = course.AddModule("Test Module", "Description", 0);
            var lesson = module.AddLesson("Test Lesson", "Content", LessonType.Video, "Content of test lesson 1", 0);
            course.Publish();
            var userId = UserId.CreateUnique();

            // Act
            var enrollment = course.EnrollUser(userId);

            // Assert
            Assert.NotNull(enrollment);
            Assert.Equal(userId, enrollment.UserId);
            Assert.Equal(course.Id, enrollment.CourseId);
            Assert.Equal(1, course.EnrollmentCount);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var enrolledEvent = Assert.IsType<UserEnrolledEvent>(domainEvent);
            Assert.Equal(course.Id, enrolledEvent.CourseId);
            Assert.Equal(userId, enrolledEvent.UserId);
        }

        [Fact]
        public void EnrollUser_ShouldThrowException_WhenCourseIsNotPublished()
        {
            // Arrange
            var course = CreateTestCourse();
            var userId = UserId.CreateUnique();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                course.EnrollUser(userId));

            Assert.Contains("Cannot enroll in an unpublished course", exception.Message);
        }

        [Fact]
        public void AddCompletionRequirement_ShouldAddRequirement_WithCorrectProperties()
        {
            // Arrange
            var course = CreateTestCourse();
            var criteria = CompletionCriteria.MinimumGrade;
            int requiredValue = 80;
            string description = "Minimum grade of 80%";
            bool isRequired = true;

            // Act
            var requirement = course.AddCompletionRequirement(
                criteria,
                requiredValue,
                description,
                isRequired);

            // Assert
            Assert.NotNull(requirement);
            Assert.Equal(course.Id, requirement.CourseId);
            Assert.Equal(criteria, requirement.Criteria);
            Assert.Equal(requiredValue, requirement.RequiredValue);
            Assert.Equal(description, requirement.Description);
            Assert.Equal(isRequired, requirement.IsRequired);
            Assert.Single(course.CompletionRequirements);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var requirementAddedEvent = Assert.IsType<CourseCompletionRequirementAddedEvent>(domainEvent);
            Assert.Equal(course.Id, requirementAddedEvent.CourseId);
            Assert.Equal(requirement.Id, requirementAddedEvent.RequirementId);
        }

        [Fact]
        public void AddCompletionRequirement_ShouldShiftExistingRequirements_WhenOrderIndexIsOccupied()
        {
            // Arrange
            var course = CreateTestCourse();
            var req1 = course.AddCompletionRequirement(CompletionCriteria.MinimumGrade, 70, "Req 1", true, 0);
            var req2 = course.AddCompletionRequirement(CompletionCriteria.TakeFinalExam, 1, "Req 2", true, 1);

            // Act - Add requirement at index 0 (should shift others)
            var newReq = course.AddCompletionRequirement(
                CompletionCriteria.LessonsCompleted,
                100,
                "New Req",
                true,
                0);

            // Assert
            Assert.Equal(3, course.CompletionRequirements.Count);
            Assert.Equal(0, newReq.OrderIndex);
            Assert.Equal(1, req1.OrderIndex);
            Assert.Equal(2, req2.OrderIndex);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddCompletionRequirement_ShouldThrowException_WhenRequiredValueIsNotPositive(int requiredValue)
        {
            // Arrange
            var course = CreateTestCourse();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                course.AddCompletionRequirement(
                    CompletionCriteria.MinimumGrade,
                    requiredValue,
                    "Invalid required value"));
        }

        [Fact]
        public void RemoveCompletionRequirement_ShouldRemoveRequirement_WhenRequirementExists()
        {
            // Arrange
            var course = CreateTestCourse();
            var requirement = course.AddCompletionRequirement(
                CompletionCriteria.MinimumGrade,
                80,
                "Minimum grade of 80%");

            // Act
            course.RemoveCompletionRequirement(requirement.Id);

            // Assert
            Assert.Empty(course.CompletionRequirements);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var requirementRemovedEvent = Assert.IsType<CourseCompletionRequirementRemovedEvent>(domainEvent);
            Assert.Equal(course.Id, requirementRemovedEvent.CourseId);
            Assert.Equal(requirement.Id, requirementRemovedEvent.RequirementId);
        }

        [Fact]
        public void RemoveCompletionRequirement_ShouldReindexRemainingRequirements()
        {
            // Arrange
            var course = CreateTestCourse();
            var req1 = course.AddCompletionRequirement(CompletionCriteria.MinimumGrade, 70, "Req 1", true, 0);
            var req2 = course.AddCompletionRequirement(CompletionCriteria.MinimumDaysActive, 5, "Req 2", true, 1);
            var req3 = course.AddCompletionRequirement(CompletionCriteria.LessonsCompleted, 100, "Req 3", true, 2);

            // Act - Remove middle requirement
            course.RemoveCompletionRequirement(req2.Id);

            // Assert
            Assert.Equal(2, course.CompletionRequirements.Count);
            Assert.Equal(0, req1.OrderIndex);
            Assert.Equal(1, req3.OrderIndex);
        }

        [Fact]
        public void RemoveCompletionRequirement_ShouldThrowException_WhenRequirementDoesNotExist()
        {
            // Arrange
            var course = CreateTestCourse();
            var nonExistentRequirementId = CompletionRequirementId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                course.RemoveCompletionRequirement(nonExistentRequirementId));
        }

        [Fact]
        public void ReorderCompletionRequirements_ShouldUpdateRequirementOrderIndices()
        {
            // Arrange
            var course = CreateTestCourse();
            var req1 = course.AddCompletionRequirement(CompletionCriteria.MinimumGrade, 70, "Req 1", true, 0);
            var req2 = course.AddCompletionRequirement(CompletionCriteria.MinimumDaysActive, 5, "Req 2", true, 1);
            var req3 = course.AddCompletionRequirement(CompletionCriteria.LessonsCompleted, 100, "Req 3", true, 2);

            // New order: req3, req1, req2
            var newOrder = new List<CompletionRequirementId> { req3.Id, req1.Id, req2.Id };

            // Act
            course.ReorderCompletionRequirements(newOrder);

            // Assert
            Assert.Equal(0, req3.OrderIndex);
            Assert.Equal(1, req1.OrderIndex);
            Assert.Equal(2, req2.OrderIndex);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var requirementsReorderedEvent = Assert.IsType<CourseCompletionRequirementsReorderedEvent>(domainEvent);
            Assert.Equal(course.Id, requirementsReorderedEvent.CourseId);
        }

        [Fact]
        public void ReorderCompletionRequirements_ShouldThrowException_WhenRequirementIdsListIsEmpty()
        {
            // Arrange
            var course = CreateTestCourse();
            course.AddCompletionRequirement(CompletionCriteria.MinimumGrade, 80, "Test Requirement");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                course.ReorderCompletionRequirements(new List<CompletionRequirementId>()));
        }

        [Fact]
        public void ReorderCompletionRequirements_ShouldThrowException_WhenRequirementDoesNotExist()
        {
            // Arrange
            var course = CreateTestCourse();
            var requirement = course.AddCompletionRequirement(CompletionCriteria.MinimumGrade, 80, "Test Requirement");
            var nonExistentRequirementId = CompletionRequirementId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                course.ReorderCompletionRequirements(new List<CompletionRequirementId>
                {
                    requirement.Id,
                    nonExistentRequirementId
                }));
        }

        [Fact]
        public void CheckEnrollmentCompletion_ShouldReturnTrue_WhenProgressIs100AndNoRequirements()
        {
            // Arrange
            var course = CreateTestCourse();
            var module = course.AddModule("Test Module", "Description", 0);

            var lesson = module.AddLesson(
                "Test Lesson",
                "Description",
                LessonType.Video,
                "Content of test lesson 1",
                0
            );

            course.Publish();

            var userId = UserId.CreateUnique();
            var enrollment = course.EnrollUser(userId);

            var lessonId = lesson.Id; 
            enrollment.MarkLessonComplete(lessonId, 1);

            // Act
            bool isCompleted = course.CheckEnrollmentCompletion(enrollment);

            // Assert
            Assert.True(isCompleted);
        }

        [Fact]
        public void CheckEnrollmentCompletion_ShouldThrowException_WhenEnrollmentIsForDifferentCourse()
        {
            // Arrange
            var course1 = CreateTestCourse();
            var module = course1.AddModule("Test Module", "Description", 0);
            var lesson = module.AddLesson("Test Lesson", "Content", LessonType.Video, "Content of test lesson 1", 0);
            course1.Publish();

            var course2 = CreateTestCourse();
            var module2 = course2.AddModule("Test Module 2", "Description 2", 0);
            var lesson2 = module2.AddLesson("Test Lesson 2", "Content 2", LessonType.Video, "Content of test lesson 2", 0);
            course2.Publish();

            var userId = UserId.CreateUnique();
            var enrollment = course2.EnrollUser(userId);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                course1.CheckEnrollmentCompletion(enrollment));

            Assert.Contains("Cannot check completion for an enrollment in a different course", exception.Message);
        }

        [Fact]
        public void AddTargetSkill_ShouldAddSkillToCourse()
        {
            // Arrange
            var course = CreateTestCourse();
            var skill = CreateTestSkill();

            // Act
            course.AddTargetSkill(skill);

            // Assert
            Assert.Single(course.Skills);
            Assert.Contains(skill, course.Skills);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var skillAddedEvent = Assert.IsType<CourseTargetSkillAddedEvent>(domainEvent);
            Assert.Equal(course.Id, skillAddedEvent.CourseId);
            Assert.Equal(skill.Id, skillAddedEvent.SkillId);
        }

        [Fact]
        public void AddTargetSkill_ShouldNotAddDuplicate_WhenSkillAlreadyExists()
        {
            // Arrange
            var course = CreateTestCourse();
            var skill = CreateTestSkill();
            course.AddTargetSkill(skill);

            // Act
            course.AddTargetSkill(skill);

            // Assert
            Assert.Single(course.Skills);
        }

        [Fact]
        public void RemoveTargetSkill_ShouldRemoveSkillFromCourse()
        {
            // Arrange
            var course = CreateTestCourse();
            var skill = CreateTestSkill();
            course.AddTargetSkill(skill);

            // Act
            course.RemoveTargetSkill(skill);

            // Assert
            Assert.Empty(course.Skills);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var skillRemovedEvent = Assert.IsType<CourseTargetSkillRemovedEvent>(domainEvent);
            Assert.Equal(course.Id, skillRemovedEvent.CourseId);
            Assert.Equal(skill.Id, skillRemovedEvent.SkillId);
        }

        [Fact]
        public void RemoveTargetSkill_ShouldDoNothing_WhenSkillNotInCourse()
        {
            // Arrange
            var course = CreateTestCourse();
            var skill = CreateTestSkill();

            // Act - Try to remove a skill that wasn't added
            course.RemoveTargetSkill(skill);

            // Assert
            Assert.Empty(course.Skills);
        }

        [Fact]
        public void ClearTargetSkills_ShouldRemoveAllSkills()
        {
            // Arrange
            var course = CreateTestCourse();
            var skill1 = CreateTestSkill();
            var skill2 = CreateTestSkill();
            course.AddTargetSkill(skill1);
            course.AddTargetSkill(skill2);

            // Act
            course.ClearTargetSkills();

            // Assert
            Assert.Empty(course.Skills);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var skillsClearedEvent = Assert.IsType<CourseTargetSkillsClearedEvent>(domainEvent);
            Assert.Equal(course.Id, skillsClearedEvent.CourseId);
            Assert.Equal(2, skillsClearedEvent.SkillIds.Count());
            Assert.Contains(skill1.Id, skillsClearedEvent.SkillIds);
            Assert.Contains(skill2.Id, skillsClearedEvent.SkillIds);
        }

        [Fact]
        public void ClearTargetSkills_ShouldDoNothing_WhenNoSkillsExist()
        {
            // Arrange
            var course = CreateTestCourse();
            int initialDomainEventCount = course.DomainEvents.Count;

            // Act
            course.ClearTargetSkills();

            // Assert
            Assert.Equal(initialDomainEventCount, course.DomainEvents.Count);
        }

        [Fact]
        public void AddTag_ShouldAddTagToCourse()
        {
            // Arrange
            var course = CreateTestCourse();
            var tag = CreateTestTag();

            // Act
            course.AddTag(tag);

            // Assert
            Assert.Single(course.Tags);
            Assert.Contains(tag, course.Tags);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var tagAddedEvent = Assert.IsType<CourseTagAddedEvent>(domainEvent);
            Assert.Equal(course.Id, tagAddedEvent.CourseId);
            Assert.Equal(tag.Id, tagAddedEvent.TagId);
        }

        [Fact]
        public void AddTag_ShouldNotAddDuplicate_WhenTagAlreadyExists()
        {
            // Arrange
            var course = CreateTestCourse();
            var tag = CreateTestTag();
            course.AddTag(tag);

            // Act
            course.AddTag(tag);

            // Assert
            Assert.Single(course.Tags);
        }

        [Fact]
        public void RemoveTag_ShouldRemoveTagFromCourse()
        {
            // Arrange
            var course = CreateTestCourse();
            var tag = CreateTestTag();
            course.AddTag(tag);

            // Act
            course.RemoveTag(tag);

            // Assert
            Assert.Empty(course.Tags);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var tagRemovedEvent = Assert.IsType<CourseTagRemovedEvent>(domainEvent);
            Assert.Equal(course.Id, tagRemovedEvent.CourseId);
            Assert.Equal(tag.Id, tagRemovedEvent.TagId);
        }

        [Fact]
        public void RemoveTag_ShouldDoNothing_WhenTagNotInCourse()
        {
            // Arrange
            var course = CreateTestCourse();
            var tag = CreateTestTag();

            // Act - Try to remove a tag that wasn't added
            course.RemoveTag(tag);

            // Assert
            Assert.Empty(course.Tags);
        }

        [Fact]
        public void ClearTags_ShouldRemoveAllTags()
        {
            // Arrange
            var course = CreateTestCourse();
            var tag1 = CreateTestTag();
            var tag2 = CreateTestTag();
            course.AddTag(tag1);
            course.AddTag(tag2);

            // Act
            course.ClearTags();

            // Assert
            Assert.Empty(course.Tags);

            // Verify domain event
            var domainEvent = course.DomainEvents.Last();
            var tagsClearedEvent = Assert.IsType<CourseTagsClearedEvent>(domainEvent);
            Assert.Equal(course.Id, tagsClearedEvent.CourseId);
            Assert.Equal(2, tagsClearedEvent.TagIds.Count());
            Assert.Contains(tag1.Id, tagsClearedEvent.TagIds);
            Assert.Contains(tag2.Id, tagsClearedEvent.TagIds);
        }

        [Fact]
        public void ClearTags_ShouldDoNothing_WhenNoTagsExist()
        {
            // Arrange
            var course = CreateTestCourse();
            int initialDomainEventCount = course.DomainEvents.Count;

            // Act
            course.ClearTags();

            // Assert
            Assert.Equal(initialDomainEventCount, course.DomainEvents.Count);
        }

        [Fact]
        public void AverageRating_ShouldCalculateCorrectly_WithMultipleReviews()
        {
            // Arrange
            var course = CreateTestCourse();
            var user1 = UserId.CreateUnique();
            var user2 = UserId.CreateUnique();
            var user3 = UserId.CreateUnique();

            course.AddReview(user1, 5, "Excellent");
            course.AddReview(user2, 3, "Average");
            course.AddReview(user3, 4, "Good");

            // Expected average: (5 + 3 + 4) / 3 = 4

            // Act & Assert
            Assert.Equal(4.0m, course.AverageRating);
        }

        [Fact]
        public void AverageRating_ShouldBeZero_WhenNoReviews()
        {
            // Arrange
            var course = CreateTestCourse();

            // Act & Assert
            Assert.Equal(0.0m, course.AverageRating);
        }

        // Helper methods
        private Course CreateTestCourse()
        {
            return Course.Create(
                "Test Course",
                "Test Course Description",
                _creatorId,
                60,
                "https://example.com/image.jpg");
        }

        private static Skill CreateTestSkill()
        {
            var category = SkillCategory.Create("Programming", "Programming skills");
            return Skill.Create("C#", "C# programming language", category);
        }

        private static CourseTag CreateTestTag()
        {
            return CourseTag.Create("programming", "Programming courses");
        }
    }
}