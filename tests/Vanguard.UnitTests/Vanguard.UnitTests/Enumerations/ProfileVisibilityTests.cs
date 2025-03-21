using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class ProfileVisibilityTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllVisibilityOptions()
        {
            // Act
            var allOptions = Vanguard.Common.Base.Enumeration.GetAll<ProfileVisibility>();

            // Assert
            Assert.Equal(5, allOptions.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectVisibility()
        {
            // Act
            var visibility = Vanguard.Common.Base.Enumeration.FromId<ProfileVisibility>(3);

            // Assert
            Assert.Equal(ProfileVisibility.FriendsOnly, visibility);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectVisibility()
        {
            // Act
            var visibility = Vanguard.Common.Base.Enumeration.FromName<ProfileVisibility>("Private");

            // Assert
            Assert.Equal(ProfileVisibility.Private, visibility);
        }

        [Theory]
        [InlineData(1, "Public")]
        [InlineData(2, "Private")]
        [InlineData(3, "Friends Only")]
        [InlineData(4, "Course Enrolled")]
        [InlineData(5, "Verified Only")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var visibility = Vanguard.Common.Base.Enumeration.FromId<ProfileVisibility>(id);

            // Assert
            Assert.Equal(expectedName, visibility.Name);
        }

        [Theory]
        [InlineData(1, "Visible to everyone")]
        [InlineData(2, "Visible only to you")]
        [InlineData(3, "Visible only to your friends")]
        [InlineData(4, "Visible only to those enrolled in the same courses")]
        [InlineData(5, "Visible only to verified members")]
        public void Description_ShouldMatchExpected(int id, string expectedDescription)
        {
            // Act
            var visibility = Vanguard.Common.Base.Enumeration.FromId<ProfileVisibility>(id);

            // Assert
            Assert.Equal(expectedDescription, visibility.Description);
        }

        [Theory]
        [InlineData(1, "globe")]
        [InlineData(2, "lock")]
        [InlineData(3, "users")]
        [InlineData(4, "graduation-cap")]
        [InlineData(5, "check-circle")]
        public void IconName_ShouldMatchExpected(int id, string expectedIconName)
        {
            // Act
            var visibility = Vanguard.Common.Base.Enumeration.FromId<ProfileVisibility>(id);

            // Assert
            Assert.Equal(expectedIconName, visibility.IconName);
        }

        [Fact]
        public void IsVisibleToEveryone_ShouldReturnTrueOnlyForPublic()
        {
            // Assert
            Assert.True(ProfileVisibility.Public.IsVisibleToEveryone());
            Assert.False(ProfileVisibility.Private.IsVisibleToEveryone());
            Assert.False(ProfileVisibility.FriendsOnly.IsVisibleToEveryone());
            Assert.False(ProfileVisibility.Enrolled.IsVisibleToEveryone());
            Assert.False(ProfileVisibility.Verified.IsVisibleToEveryone());
        }

        [Fact]
        public void IsVisibleToFriends_ShouldReturnTrueForPublicAndFriendsOnly()
        {
            // Assert
            Assert.True(ProfileVisibility.Public.IsVisibleToFriends());
            Assert.True(ProfileVisibility.FriendsOnly.IsVisibleToFriends());
            Assert.False(ProfileVisibility.Private.IsVisibleToFriends());
            Assert.False(ProfileVisibility.Enrolled.IsVisibleToFriends());
            Assert.False(ProfileVisibility.Verified.IsVisibleToFriends());
        }

        [Fact]
        public void IsVisibleToEnrolled_ShouldReturnTrueForPublicFriendsAndEnrolled()
        {
            // Assert
            Assert.True(ProfileVisibility.Public.IsVisibleToEnrolled());
            Assert.True(ProfileVisibility.FriendsOnly.IsVisibleToEnrolled());
            Assert.True(ProfileVisibility.Enrolled.IsVisibleToEnrolled());
            Assert.False(ProfileVisibility.Private.IsVisibleToEnrolled());
            Assert.False(ProfileVisibility.Verified.IsVisibleToEnrolled());
        }

        [Fact]
        public void IsVisibleToVerified_ShouldReturnTrueForAllExceptPrivate()
        {
            // Assert
            Assert.True(ProfileVisibility.Public.IsVisibleToVerified());
            Assert.True(ProfileVisibility.FriendsOnly.IsVisibleToVerified());
            Assert.True(ProfileVisibility.Enrolled.IsVisibleToVerified());
            Assert.True(ProfileVisibility.Verified.IsVisibleToVerified());
            Assert.False(ProfileVisibility.Private.IsVisibleToVerified());
        }
    }
}