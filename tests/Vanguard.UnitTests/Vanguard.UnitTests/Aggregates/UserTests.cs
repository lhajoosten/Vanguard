using System;
using System.Linq;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;
using Xunit;

namespace Vanguard.UnitTests.Aggregates
{
    public class UserTests
    {
        [Fact]
        public void Create_ShouldCreateNewUser_WithCorrectProperties()
        {
            // Arrange
            string email = "test@example.com";
            string firstName = "John";
            string lastName = "Doe";

            // Act
            var user = User.Create(email, firstName, lastName);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal("John Doe", user.FullName);
            Assert.True(user.IsActive);
            Assert.Null(user.LastLoginAt);
            Assert.Empty(user.Roles);
            Assert.Null(user.Profile);
            Assert.Null(user.Settings);

            // Verify domain event
            var domainEvent = Assert.Single(user.DomainEvents);
            var userCreatedEvent = Assert.IsType<UserCreatedEvent>(domainEvent);
            Assert.Equal(user.Id, userCreatedEvent.UserId);
        }

        [Theory]
        [InlineData("", "John", "Doe")]
        [InlineData("  ", "John", "Doe")]
        [InlineData(null, "John", "Doe")]
        public void Create_ShouldThrowExceptionForInvalidEmail(string email, string firstName, string lastName)
        {
            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                User.Create(email, firstName, lastName));

            if (email == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("email", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Email cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData("test@example.com", "", "Doe")]
        [InlineData("test@example.com", "  ", "Doe")]
        [InlineData("test@example.com", null, "Doe")]
        public void Create_ShouldThrowExceptionForInvalidFirstName(string email, string firstName, string lastName)
        {
            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                User.Create(email, firstName, lastName));

            if (firstName == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("firstName", nullException.ParamName);
            }
            else
            {
                Assert.Contains("First name cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData("test@example.com", "John", "")]
        [InlineData("test@example.com", "John", "  ")]
        [InlineData("test@example.com", "John", null)]
        public void Create_ShouldThrowExceptionForInvalidLastName(string email, string firstName, string lastName)
        {
            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                User.Create(email, firstName, lastName));

            if (lastName == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("lastName", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Last name cannot be empty", exception.Message);
            }
        }

        [Fact]
        public void AddRole_ShouldAddRoleToUser()
        {
            // Arrange
            var user = CreateTestUser();
            var role = CreateTestRole();

            // Act
            user.AddRole(role);

            // Assert
            Assert.Single(user.Roles);
            Assert.Contains(role, user.Roles);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var roleAddedEvent = Assert.IsType<UserRoleAddedEvent>(domainEvent);
            Assert.Equal(user.Id, roleAddedEvent.UserId);
            Assert.Equal(role.Id, roleAddedEvent.RoleId);
        }

        [Fact]
        public void AddRole_ShouldNotAddDuplicate_WhenRoleAlreadyExists()
        {
            // Arrange
            var user = CreateTestUser();
            var role = CreateTestRole();
            user.AddRole(role);

            // Act
            user.AddRole(role);

            // Assert
            Assert.Single(user.Roles);
        }

        [Fact]
        public void AddRole_ShouldThrowException_WhenRoleIsNull()
        {
            // Arrange
            var user = CreateTestUser();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => user.AddRole(null));
        }

        [Fact]
        public void RemoveRole_ShouldRemoveRoleFromUser()
        {
            // Arrange
            var user = CreateTestUser();
            var role = CreateTestRole();
            user.AddRole(role);

            // Act
            user.RemoveRole(role);

            // Assert
            Assert.Empty(user.Roles);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var roleRemovedEvent = Assert.IsType<UserRoleRemovedEvent>(domainEvent);
            Assert.Equal(user.Id, roleRemovedEvent.UserId);
            Assert.Equal(role.Id, roleRemovedEvent.RoleId);
        }

        [Fact]
        public void RemoveRole_ShouldDoNothing_WhenRoleNotInUser()
        {
            // Arrange
            var user = CreateTestUser();
            var role = CreateTestRole();

            // Act - Try to remove a role that wasn't added
            user.RemoveRole(role);

            // Assert
            Assert.Empty(user.Roles);
        }

        [Fact]
        public void RemoveRole_ShouldThrowException_WhenRoleIsNull()
        {
            // Arrange
            var user = CreateTestUser();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => user.RemoveRole(null));
        }

        [Fact]
        public void UpdateProfile_ShouldUpdateUserProperties()
        {
            // Arrange
            var user = CreateTestUser();
            string newFirstName = "Jane";
            string newLastName = "Smith";

            // Act
            user.UpdateProfile(newFirstName, newLastName);

            // Assert
            Assert.Equal(newFirstName, user.FirstName);
            Assert.Equal(newLastName, user.LastName);
            Assert.Equal("Jane Smith", user.FullName);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var profileUpdatedEvent = Assert.IsType<UserProfileUpdatedEvent>(domainEvent);
            Assert.Equal(user.Id, profileUpdatedEvent.UserId);
        }

        [Theory]
        [InlineData("", "Smith")]
        [InlineData("  ", "Smith")]
        [InlineData(null, "Smith")]
        public void UpdateProfile_ShouldThrowExceptionForInvalidFirstName(string firstName, string lastName)
        {
            // Arrange
            var user = CreateTestUser();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                user.UpdateProfile(firstName, lastName));

            if (firstName == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("firstName", nullException.ParamName);
            }
            else
            {
                Assert.Contains("First name cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData("Jane", "")]
        [InlineData("Jane", "  ")]
        [InlineData("Jane", null)]
        public void UpdateProfile_ShouldThrowExceptionForInvalidLastName(string firstName, string lastName)
        {
            // Arrange
            var user = CreateTestUser();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                user.UpdateProfile(firstName, lastName));

            if (lastName == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("lastName", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Last name cannot be empty", exception.Message);
            }
        }

        [Fact]
        public void UpdateLastLogin_ShouldUpdateTimestamp()
        {
            // Arrange
            var user = CreateTestUser();
            Assert.Null(user.LastLoginAt);

            // Act
            user.UpdateLastLogin();

            // Assert
            Assert.NotNull(user.LastLoginAt);
        }

        [Fact]
        public void CreateProfile_ShouldCreateUserProfile()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var profile = user.CreateProfile();

            // Assert
            Assert.NotNull(profile);
            Assert.Equal(user.Id, profile.UserId);
            Assert.Equal(profile, user.Profile);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var profileCreatedEvent = Assert.IsType<UserProfileCreatedEvent>(domainEvent);
            Assert.Equal(user.Id, profileCreatedEvent.UserId);
            Assert.Equal(profile.Id, profileCreatedEvent.ProfileId);
        }

        [Fact]
        public void CreateSettings_ShouldCreateUserSettings()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var settings = user.CreateSettings();

            // Assert
            Assert.NotNull(settings);
            Assert.Equal(user.Id, settings.UserId);
            Assert.Equal(settings, user.Settings);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var settingsCreatedEvent = Assert.IsType<UserSettingsCreatedEvent>(domainEvent);
            Assert.Equal(user.Id, settingsCreatedEvent.UserId);
            Assert.Equal(settings.Id, settingsCreatedEvent.SettingsId);
        }

        [Fact]
        public void Activate_ShouldActivateUser()
        {
            // Arrange
            var user = CreateTestUser();
            user.Deactivate();
            Assert.False(user.IsActive);

            // Act
            user.Activate();

            // Assert
            Assert.True(user.IsActive);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var activatedEvent = Assert.IsType<UserActivatedEvent>(domainEvent);
            Assert.Equal(user.Id, activatedEvent.UserId);
        }

        [Fact]
        public void Activate_ShouldDoNothing_WhenUserIsAlreadyActive()
        {
            // Arrange
            var user = CreateTestUser();
            Assert.True(user.IsActive);
            int initialEventCount = user.DomainEvents.Count;

            // Act
            user.Activate();

            // Assert
            Assert.True(user.IsActive);
            Assert.Equal(initialEventCount, user.DomainEvents.Count);
        }

        [Fact]
        public void Deactivate_ShouldDeactivateUser()
        {
            // Arrange
            var user = CreateTestUser();
            Assert.True(user.IsActive);

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.IsActive);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var deactivatedEvent = Assert.IsType<UserDeactivatedEvent>(domainEvent);
            Assert.Equal(user.Id, deactivatedEvent.UserId);
        }

        [Fact]
        public void Deactivate_ShouldDoNothing_WhenUserIsAlreadyInactive()
        {
            // Arrange
            var user = CreateTestUser();
            user.Deactivate();
            Assert.False(user.IsActive);
            int initialEventCount = user.DomainEvents.Count;

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.IsActive);
            Assert.Equal(initialEventCount, user.DomainEvents.Count);
        }

        [Fact]
        public void AssessSkill_ShouldCreateSkillAssessment()
        {
            // Arrange
            var user = CreateTestUser();
            var skill = CreateTestSkill();
            var level = ProficiencyLevel.Intermediate;
            string evidence = "I've been using this skill for 2 years";

            // Act
            var assessment = user.AssessSkill(skill, level, evidence);

            // Assert
            Assert.NotNull(assessment);
            Assert.Equal(user.Id, assessment.UserId);
            Assert.Equal(skill.Id, assessment.SkillId);
            Assert.Equal(level, assessment.Level);
            Assert.Equal(evidence, assessment.Evidence);

            // Verify domain event
            var domainEvent = user.DomainEvents.Last();
            var skillAssessedEvent = Assert.IsType<UserSkillAssessedEvent>(domainEvent);
            Assert.Equal(user.Id, skillAssessedEvent.UserId);
            Assert.Equal(skill.Id, skillAssessedEvent.SkillId);
            Assert.Equal(assessment.Id, skillAssessedEvent.AssessmentId);
        }

        [Fact]
        public void AssessSkill_ShouldThrowException_WhenSkillIsNull()
        {
            // Arrange
            var user = CreateTestUser();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                user.AssessSkill(null, ProficiencyLevel.Beginner));
        }

        [Fact]
        public void VerifySkillAssessment_ShouldVerifyAssessment()
        {
            // Arrange
            var user = CreateTestUser();
            var otherUser = User.Create("other@example.com", "Other", "User");
            var skill = CreateTestSkill();

            // Create an assessment for the other user
            var assessment = otherUser.AssessSkill(skill, ProficiencyLevel.Advanced);
            Assert.False(assessment.IsVerified);

            // Act
            user.VerifySkillAssessment(assessment);

            // Assert
            Assert.True(assessment.IsVerified);
            Assert.Equal(user.Id, assessment.VerifiedById);
        }

        [Fact]
        public void VerifySkillAssessment_ShouldThrowException_WhenAssessmentIsNull()
        {
            // Arrange
            var user = CreateTestUser();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => user.VerifySkillAssessment(null));
        }

        [Fact]
        public void VerifySkillAssessment_ShouldThrowException_WhenVerifyingOwnAssessment()
        {
            // Arrange
            var user = CreateTestUser();
            var skill = CreateTestSkill();
            var assessment = user.AssessSkill(skill, ProficiencyLevel.Intermediate);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                user.VerifySkillAssessment(assessment));

            Assert.Contains("Users cannot verify their own skill assessments", exception.Message);
        }

        // Helper methods
        private User CreateTestUser()
        {
            return User.Create("test@example.com", "John", "Doe");
        }

        private static Role CreateTestRole()
        {
            return Role.Create("Instructor");
        }

        private static Skill CreateTestSkill()
        {
            var category = SkillCategory.Create("Programming", "Programming skills category");
            return Skill.Create("C#", "C# programming language", category);
        }
    }
}