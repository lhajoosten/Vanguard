using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class EnrollmentStatusTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllEnrollmentStatuses()
        {
            // Act
            var allStatuses = Vanguard.Common.Base.Enumeration.GetAll<EnrollmentStatus>();

            // Assert
            Assert.Equal(6, allStatuses.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectStatus()
        {
            // Act
            var status = Vanguard.Common.Base.Enumeration.FromId<EnrollmentStatus>(3);

            // Assert
            Assert.Equal(EnrollmentStatus.Dropped, status);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectStatus()
        {
            // Act
            var status = Vanguard.Common.Base.Enumeration.FromName<EnrollmentStatus>("Completed");

            // Assert
            Assert.Equal(EnrollmentStatus.Completed, status);
        }

        [Theory]
        [InlineData(1, "Active")]
        [InlineData(2, "Completed")]
        [InlineData(3, "Dropped")]
        [InlineData(4, "On Hold")]
        [InlineData(5, "Expired")]
        [InlineData(6, "Pending")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var status = Vanguard.Common.Base.Enumeration.FromId<EnrollmentStatus>(id);

            // Assert
            Assert.Equal(expectedName, status.Name);
        }

        [Theory]
        [InlineData(1, "Currently taking the course")]
        [InlineData(2, "Successfully finished the course")]
        [InlineData(3, "Withdrawn from the course")]
        [InlineData(4, "Temporarily paused")]
        [InlineData(5, "Access period has ended")]
        [InlineData(6, "Enrollment awaiting approval")]
        public void Description_ShouldMatchExpected(int id, string expectedDescription)
        {
            // Act
            var status = Vanguard.Common.Base.Enumeration.FromId<EnrollmentStatus>(id);

            // Assert
            Assert.Equal(expectedDescription, status.Description);
        }

        [Theory]
        [InlineData(1, "green")]
        [InlineData(2, "blue")]
        [InlineData(3, "red")]
        [InlineData(4, "yellow")]
        [InlineData(5, "gray")]
        [InlineData(6, "orange")]
        public void ColorCode_ShouldMatchExpected(int id, string expectedColor)
        {
            // Act
            var status = Vanguard.Common.Base.Enumeration.FromId<EnrollmentStatus>(id);

            // Assert
            Assert.Equal(expectedColor, status.ColorCode);
        }

        [Fact]
        public void IsActive_ShouldReturnTrueOnlyForActiveStatus()
        {
            // Assert
            Assert.True(EnrollmentStatus.Active.IsActive());
            Assert.False(EnrollmentStatus.Completed.IsActive());
            Assert.False(EnrollmentStatus.Dropped.IsActive());
            Assert.False(EnrollmentStatus.OnHold.IsActive());
            Assert.False(EnrollmentStatus.Expired.IsActive());
            Assert.False(EnrollmentStatus.Pending.IsActive());
        }

        [Fact]
        public void IsCompleted_ShouldReturnTrueOnlyForCompletedStatus()
        {
            // Assert
            Assert.True(EnrollmentStatus.Completed.IsCompleted());
            Assert.False(EnrollmentStatus.Active.IsCompleted());
            Assert.False(EnrollmentStatus.Dropped.IsCompleted());
            Assert.False(EnrollmentStatus.OnHold.IsCompleted());
            Assert.False(EnrollmentStatus.Expired.IsCompleted());
            Assert.False(EnrollmentStatus.Pending.IsCompleted());
        }

        [Fact]
        public void IsTerminated_ShouldReturnTrueForDroppedAndExpiredStatuses()
        {
            // Assert
            Assert.True(EnrollmentStatus.Dropped.IsTerminated());
            Assert.True(EnrollmentStatus.Expired.IsTerminated());
            Assert.False(EnrollmentStatus.Active.IsTerminated());
            Assert.False(EnrollmentStatus.Completed.IsTerminated());
            Assert.False(EnrollmentStatus.OnHold.IsTerminated());
            Assert.False(EnrollmentStatus.Pending.IsTerminated());
        }

        [Fact]
        public void IsPaused_ShouldReturnTrueOnlyForOnHoldStatus()
        {
            // Assert
            Assert.True(EnrollmentStatus.OnHold.IsPaused());
            Assert.False(EnrollmentStatus.Active.IsPaused());
            Assert.False(EnrollmentStatus.Completed.IsPaused());
            Assert.False(EnrollmentStatus.Dropped.IsPaused());
            Assert.False(EnrollmentStatus.Expired.IsPaused());
            Assert.False(EnrollmentStatus.Pending.IsPaused());
        }

        [Fact]
        public void CanResume_ShouldReturnTrueForOnHoldAndPendingStatuses()
        {
            // Assert
            Assert.True(EnrollmentStatus.OnHold.CanResume());
            Assert.True(EnrollmentStatus.Pending.CanResume());
            Assert.False(EnrollmentStatus.Active.CanResume());
            Assert.False(EnrollmentStatus.Completed.CanResume());
            Assert.False(EnrollmentStatus.Dropped.CanResume());
            Assert.False(EnrollmentStatus.Expired.CanResume());
        }

        [Fact]
        public void CanComplete_ShouldReturnTrueOnlyForActiveStatus()
        {
            // Assert
            Assert.True(EnrollmentStatus.Active.CanComplete());
            Assert.False(EnrollmentStatus.Completed.CanComplete());
            Assert.False(EnrollmentStatus.Dropped.CanComplete());
            Assert.False(EnrollmentStatus.OnHold.CanComplete());
            Assert.False(EnrollmentStatus.Expired.CanComplete());
            Assert.False(EnrollmentStatus.Pending.CanComplete());
        }

        [Fact]
        public void CanModify_ShouldReturnTrueForActiveOnHoldAndPendingStatuses()
        {
            // Assert
            Assert.True(EnrollmentStatus.Active.CanModify());
            Assert.True(EnrollmentStatus.OnHold.CanModify());
            Assert.True(EnrollmentStatus.Pending.CanModify());
            Assert.False(EnrollmentStatus.Completed.CanModify());
            Assert.False(EnrollmentStatus.Dropped.CanModify());
            Assert.False(EnrollmentStatus.Expired.CanModify());
        }

        [Theory]
        [MemberData(nameof(GetStatusClassData))]
        public void GetStatusClass_ShouldReturnFormattedClassName(EnrollmentStatus status, string expectedClass)
        {
            // Act
            var statusClass = status.GetStatusClass();

            // Assert
            Assert.Equal(expectedClass, statusClass);
        }

        [Theory]
        [MemberData(nameof(GetStatusBadgeData))]
        public void GetStatusBadge_ShouldReturnProperHtmlForBadge(EnrollmentStatus status, string expectedBadge)
        {
            // Act
            var badge = status.GetStatusBadge();

            // Assert
            Assert.Equal(expectedBadge, badge);
        }

        public static IEnumerable<object[]> GetStatusClassData()
        {
            yield return new object[] { EnrollmentStatus.Active, "status-active" };
            yield return new object[] { EnrollmentStatus.Completed, "status-completed" };
            yield return new object[] { EnrollmentStatus.Dropped, "status-dropped" };
            yield return new object[] { EnrollmentStatus.OnHold, "status-on-hold" };
            yield return new object[] { EnrollmentStatus.Expired, "status-expired" };
            yield return new object[] { EnrollmentStatus.Pending, "status-pending" };
        }

        public static IEnumerable<object[]> GetStatusBadgeData()
        {
            yield return new object[] { EnrollmentStatus.Active, "<span class=\"badge bg-success\"><i class=\"fa fa-play-circle\"></i> Active</span>" };
            yield return new object[] { EnrollmentStatus.Completed, "<span class=\"badge bg-primary\"><i class=\"fa fa-check-circle\"></i> Completed</span>" };
        }
    }
}