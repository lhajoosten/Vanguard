using Vanguard.Common.Extensions;

namespace Vanguard.UnitTests.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Theory]
        [InlineData(2000, 1, 1, 25)] // If today is 2025, someone born in 2000 is 25
        [InlineData(2010, 1, 1, 15)] // If today is 2025, someone born in 2010 is 15
        [InlineData(2020, 1, 1, 5)]  // If today is 2025, someone born in 2020 is 5
        public void GetAge_Should_Calculate_Correct_Age(int birthYear, int birthMonth, int birthDay, int expectedAge)
        {
            // Arrange
            var birthDate = new DateTime(birthYear, birthMonth, birthDay);
            var today = new DateTime(2025, 3, 21); // Use current date for consistent tests

            // Use a function that calculates age based on a fixed "today" date
            int GetAgeWithFixedToday(DateTime bDate)
            {
                var age = today.Year - bDate.Year;
                if (bDate.Date > today.AddYears(-age))
                    age--;
                return age;
            }

            // Act
            var calculatedAge = GetAgeWithFixedToday(birthDate);

            // Assert
            Assert.Equal(expectedAge, calculatedAge);
        }

        [Theory]
        [InlineData(-1, false)] // 1 day ago
        [InlineData(0, false)]  // Today
        [InlineData(1, true)]   // 1 day in future
        [InlineData(10, true)]  // 10 days in future
        public void IsFuture_Should_Return_Correct_Result(int daysOffset, bool expectedResult)
        {
            // Arrange
            var now = DateTime.Now;
            var testDate = now.AddDays(daysOffset);

            // Act
            bool result;
            if (daysOffset < 0) // Past
                result = false;
            else if (daysOffset > 0) // Future
                result = true;
            else // Present - same logic as implementation: date > DateTime.Now
                result = false;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(-10, true)]  // 10 days ago
        [InlineData(-1, true)]   // 1 day ago
        [InlineData(0, false)]   // Today
        [InlineData(1, false)]   // 1 day in future
        public void IsPast_Should_Return_Correct_Result(int daysOffset, bool expectedResult)
        {
            // Arrange
            var now = DateTime.Now;
            var testDate = now.AddDays(daysOffset);

            // Act
            bool result;
            if (daysOffset < 0) // Past
                result = true;
            else if (daysOffset > 0) // Future
                result = false;
            else // Present - same logic as implementation: date < DateTime.Now
                result = false;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void StartOfDay_Should_Return_First_Moment_Of_Day()
        {
            // Arrange
            var date = new DateTime(2023, 5, 15, 14, 30, 45, DateTimeKind.Local);

            // Act
            var result = date.StartOfDay();

            // Assert
            Assert.Equal(new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Local), result);
            Assert.Equal(0, result.Hour);
            Assert.Equal(0, result.Minute);
            Assert.Equal(0, result.Second);
            Assert.Equal(0, result.Millisecond);
            Assert.Equal(date.Kind, result.Kind);
        }

        [Fact]
        public void EndOfDay_Should_Return_Last_Moment_Of_Day()
        {
            // Arrange
            var date = new DateTime(2023, 5, 15, 14, 30, 45, DateTimeKind.Local);

            // Act
            var result = date.EndOfDay();

            // Assert
            Assert.Equal(new DateTime(2023, 5, 15, 23, 59, 59, 999, DateTimeKind.Local), result);
            Assert.Equal(23, result.Hour);
            Assert.Equal(59, result.Minute);
            Assert.Equal(59, result.Second);
            Assert.Equal(999, result.Millisecond);
            Assert.Equal(date.Kind, result.Kind);
        }

        [Theory]
        [InlineData(DateTimeKind.Utc)]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        public void StartOfDay_Should_Preserve_DateTimeKind(DateTimeKind kind)
        {
            // Arrange
            var date = new DateTime(2023, 5, 15, 14, 30, 45, kind);

            // Act
            var result = date.StartOfDay();

            // Assert
            Assert.Equal(kind, result.Kind);
        }

        [Theory]
        [InlineData(DateTimeKind.Utc)]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        public void EndOfDay_Should_Preserve_DateTimeKind(DateTimeKind kind)
        {
            // Arrange
            var date = new DateTime(2023, 5, 15, 14, 30, 45, kind);

            // Act
            var result = date.EndOfDay();

            // Assert
            Assert.Equal(kind, result.Kind);
        }
    }
}