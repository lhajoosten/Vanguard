using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class LessonTypeTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllLessonTypes()
        {
            // Act
            var allTypes = Vanguard.Common.Base.Enumeration.GetAll<LessonType>();

            // Assert
            Assert.Equal(8, allTypes.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectLessonType()
        {
            // Act
            var lessonType = Vanguard.Common.Base.Enumeration.FromId<LessonType>(3);

            // Assert
            Assert.Equal(LessonType.Quiz, lessonType);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectLessonType()
        {
            // Act
            var lessonType = Vanguard.Common.Base.Enumeration.FromName<LessonType>("Assignment");

            // Assert
            Assert.Equal(LessonType.Assignment, lessonType);
        }

        [Theory]
        [InlineData(1, "Video")]
        [InlineData(2, "Text")]
        [InlineData(3, "Quiz")]
        [InlineData(4, "Assignment")]
        [InlineData(5, "Discussion")]
        [InlineData(6, "Presentation")]
        [InlineData(7, "Live Session")]
        [InlineData(8, "Simulation")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var lessonType = Vanguard.Common.Base.Enumeration.FromId<LessonType>(id);

            // Assert
            Assert.Equal(expectedName, lessonType.Name);
        }

        [Theory]
        [InlineData(1, "video")]
        [InlineData(2, "document")]
        [InlineData(3, "question-circle")]
        [InlineData(4, "tasks")]
        [InlineData(5, "comments")]
        [InlineData(6, "presentation")]
        [InlineData(7, "video-camera")]
        [InlineData(8, "vr-cardboard")]
        public void IconName_ShouldMatchExpected(int id, string expectedIconName)
        {
            // Act
            var lessonType = Vanguard.Common.Base.Enumeration.FromId<LessonType>(id);

            // Assert
            Assert.Equal(expectedIconName, lessonType.IconName);
        }

        [Theory]
        [InlineData(1, "video")]
        [InlineData(2, "article")]
        [InlineData(3, "quiz")]
        [InlineData(4, "assignment")]
        [InlineData(5, "discussion")]
        [InlineData(6, "presentation")]
        [InlineData(7, "live-session")]
        [InlineData(8, "simulation")]
        public void UrlSlug_ShouldMatchExpected(int id, string expectedUrlSlug)
        {
            // Act
            var lessonType = Vanguard.Common.Base.Enumeration.FromId<LessonType>(id);

            // Assert
            Assert.Equal(expectedUrlSlug, lessonType.UrlSlug);
        }

        [Fact]
        public void RequiresSubmission_ShouldReturnTrueOnlyForAssignment()
        {
            // Assert
            Assert.True(LessonType.Assignment.RequiresSubmission());
            Assert.False(LessonType.Video.RequiresSubmission());
            Assert.False(LessonType.Text.RequiresSubmission());
            Assert.False(LessonType.Quiz.RequiresSubmission());
            Assert.False(LessonType.Discussion.RequiresSubmission());
            Assert.False(LessonType.Presentation.RequiresSubmission());
            Assert.False(LessonType.LiveSession.RequiresSubmission());
            Assert.False(LessonType.Simulation.RequiresSubmission());
        }

        [Fact]
        public void IsInteractive_ShouldReturnTrueForInteractiveTypes()
        {
            // Assert
            Assert.True(LessonType.Quiz.IsInteractive());
            Assert.True(LessonType.Assignment.IsInteractive());
            Assert.True(LessonType.Discussion.IsInteractive());
            Assert.True(LessonType.LiveSession.IsInteractive());
            Assert.True(LessonType.Simulation.IsInteractive());
            Assert.False(LessonType.Video.IsInteractive());
            Assert.False(LessonType.Text.IsInteractive());
            Assert.False(LessonType.Presentation.IsInteractive());
        }

        [Fact]
        public void IsMedia_ShouldReturnTrueForMediaTypes()
        {
            // Assert
            Assert.True(LessonType.Video.IsMedia());
            Assert.True(LessonType.Presentation.IsMedia());
            Assert.False(LessonType.Text.IsMedia());
            Assert.False(LessonType.Quiz.IsMedia());
            Assert.False(LessonType.Assignment.IsMedia());
            Assert.False(LessonType.Discussion.IsMedia());
            Assert.False(LessonType.LiveSession.IsMedia());
            Assert.False(LessonType.Simulation.IsMedia());
        }

        [Fact]
        public void SupportsAttachments_ShouldReturnTrueForSupportedTypes()
        {
            // Assert
            Assert.True(LessonType.Assignment.SupportsAttachments());
            Assert.True(LessonType.Text.SupportsAttachments());
            Assert.True(LessonType.Discussion.SupportsAttachments());
            Assert.False(LessonType.Video.SupportsAttachments());
            Assert.False(LessonType.Quiz.SupportsAttachments());
            Assert.False(LessonType.Presentation.SupportsAttachments());
            Assert.False(LessonType.LiveSession.SupportsAttachments());
            Assert.False(LessonType.Simulation.SupportsAttachments());
        }

        [Theory]
        [MemberData(nameof(GetIconClass))]
        public void GetIconClass_ShouldReturnProperFontAwesomeClass(LessonType lessonType, string expected)
        {
            // Act
            var iconClass = lessonType.GetIconClass();

            // Assert
            Assert.Equal(expected, iconClass);
        }

        [Theory]
        [MemberData(nameof(GetEstimatedDurationData))]
        public void GetEstimatedDuration_ShouldReturnCalculatedTimespan(LessonType lessonType, int contentLength, int expectedMinutes)
        {
            // Act
            var duration = lessonType.GetEstimatedDuration(contentLength);

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(expectedMinutes), duration);
        }

        public static IEnumerable<object[]> GetEstimatedDurationData()
        {
            yield return new object[] { LessonType.Video, 60, 60 };
            yield return new object[] { LessonType.Text, 2000, 10 };
            yield return new object[] { LessonType.Quiz, 10, 20 };
            yield return new object[] { LessonType.Assignment, 3, 180 };
            yield return new object[] { LessonType.Discussion, 50, 30 };
            yield return new object[] { LessonType.Presentation, 20, 40 };
            yield return new object[] { LessonType.LiveSession, 45, 45 };
            yield return new object[] { LessonType.Simulation, 30, 30 };
        }

        public static IEnumerable<object[]> GetIconClass()
        {
            yield return new object[] { LessonType.Video, "fa fa-video" };
            yield return new object[] { LessonType.Text, "fa fa-document" };
            yield return new object[] { LessonType.Quiz, "fa fa-question-circle" };
            yield return new object[] { LessonType.Assignment, "fa fa-tasks" };
        }
    }
}