using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.UnitTests.Aggregates
{
    public class SkillTests
    {
        [Fact]
        public void Create_ShouldCreateNewSkill_WithCorrectProperties()
        {
            // Arrange
            string name = "C#";
            string description = "C# programming language";
            var category = CreateTestCategory();

            // Act
            var skill = Skill.Create(name, description, category);

            // Assert
            Assert.NotNull(skill);
            Assert.Equal(name, skill.Name);
            Assert.Equal(description, skill.Description);
            Assert.Equal(category.Id, skill.CategoryId);
            Assert.Equal(category, skill.Category);

            // Verify domain event
            var domainEvent = Assert.Single(skill.DomainEvents);
            var skillCreatedEvent = Assert.IsType<SkillCreatedEvent>(domainEvent);
            Assert.Equal(skill.Id, skillCreatedEvent.SkillId);
        }

        [Theory]
        [InlineData("", "Description")]
        [InlineData("  ", "Description")]
        [InlineData(null, "Description")]
        public void Create_ShouldThrowExceptionForInvalidName(string name, string description)
        {
            // Arrange
            var category = CreateTestCategory();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                Skill.Create(name, description, category));

            if (name == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("name", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Skill name cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData("Name", "")]
        [InlineData("Name", "  ")]
        [InlineData("Name", null)]
        public void Create_ShouldThrowExceptionForInvalidDescription(string name, string description)
        {
            // Arrange
            var category = CreateTestCategory();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                Skill.Create(name, description, category));

            if (description == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("description", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Skill description cannot be empty", exception.Message);
            }
        }

        [Fact]
        public void Create_ShouldThrowArgumentException_WhenCategoryIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                Skill.Create("Name", "Description", null));

            Assert.Contains("Skill category cannot be null", exception.Message);
        }

        [Fact]
        public void Update_ShouldUpdateSkillProperties()
        {
            // Arrange
            var skill = CreateTestSkill();
            string newName = "Updated Skill";
            string newDescription = "Updated Description";
            var newCategory = SkillCategory.Create("New Category", "New category description");

            // Act
            skill.Update(newName, newDescription, newCategory);

            // Assert
            Assert.Equal(newName, skill.Name);
            Assert.Equal(newDescription, skill.Description);
            Assert.Equal(newCategory.Id, skill.CategoryId);
            Assert.Equal(newCategory, skill.Category);

            // Verify domain event
            var domainEvent = skill.DomainEvents.Last();
            var skillUpdatedEvent = Assert.IsType<SkillUpdatedEvent>(domainEvent);
            Assert.Equal(skill.Id, skillUpdatedEvent.SkillId);
        }

        [Theory]
        [InlineData("", "Description")]
        [InlineData("  ", "Description")]
        [InlineData(null, "Description")]
        public void Update_ShouldThrowExceptionForInvalidName(string name, string description)
        {
            // Arrange
            var skill = CreateTestSkill();
            var category = CreateTestCategory();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                skill.Update(name, description, category));

            if (name == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("name", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Skill name cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData("Name", "")]
        [InlineData("Name", "  ")]
        [InlineData("Name", null)]
        public void Update_ShouldThrowExceptionForInvalidDescription(string name, string description)
        {
            // Arrange
            var skill = CreateTestSkill();
            var category = CreateTestCategory();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                skill.Update(name, description, category));

            if (description == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("description", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Skill description cannot be empty", exception.Message);
            }
        }

        [Fact]
        public void Update_ShouldThrowArgumentException_WhenCategoryIsNull()
        {
            // Arrange
            var skill = CreateTestSkill();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                skill.Update("Name", "Description", null));

            Assert.Contains("Skill category cannot be null", exception.Message);
        }

        [Fact]
        public void AssessFor_ShouldCreateAssessment_WithCorrectProperties()
        {
            // Arrange
            var skill = CreateTestSkill();
            var user = CreateTestUser();
            var level = ProficiencyLevel.Intermediate;
            string evidence = "I've used this skill for 2 years";

            // Act
            var assessment = skill.AssessFor(user, level, evidence);

            // Assert
            Assert.NotNull(assessment);
            Assert.Equal(user.Id, assessment.UserId);
            Assert.Equal(skill.Id, assessment.SkillId);
            Assert.Equal(level, assessment.Level);
            Assert.Equal(evidence, assessment.Evidence);
            Assert.False(assessment.IsVerified);

            // Verify domain event
            var domainEvent = skill.DomainEvents.Last();
            var skillAssessedEvent = Assert.IsType<SkillAssessedForUserEvent>(domainEvent);
            Assert.Equal(skill.Id, skillAssessedEvent.SkillId);
            Assert.Equal(user.Id, skillAssessedEvent.UserId);
            Assert.Equal(assessment.Id, skillAssessedEvent.AssessmentId);
        }

        [Fact]
        public void AssessFor_ShouldThrowArgumentException_WhenUserIsNull()
        {
            // Arrange
            var skill = CreateTestSkill();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                skill.AssessFor(null, ProficiencyLevel.Beginner));
        }

        // Helper methods
        private static SkillCategory CreateTestCategory()
        {
            return SkillCategory.Create("Programming", "Programming skills category");
        }

        private static Skill CreateTestSkill()
        {
            var category = CreateTestCategory();
            return Skill.Create("C#", "C# programming language", category);
        }

        private static User CreateTestUser()
        {
            return User.Create("test@example.com", "John", "Doe");
        }
    }
}
