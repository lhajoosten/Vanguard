using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class DifficultyLevelTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllDifficultyLevels()
        {
            // Act
            var allLevels = Vanguard.Common.Base.Enumeration.GetAll<DifficultyLevel>();

            // Assert
            Assert.Equal(4, allLevels.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectDifficultyLevel()
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromId<DifficultyLevel>(3);

            // Assert
            Assert.Equal(DifficultyLevel.Hard, level);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectDifficultyLevel()
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromName<DifficultyLevel>("Medium");

            // Assert
            Assert.Equal(DifficultyLevel.Medium, level);
        }

        [Theory]
        [InlineData(1, "Easy")]
        [InlineData(2, "Medium")]
        [InlineData(3, "Hard")]
        [InlineData(4, "Expert")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromId<DifficultyLevel>(id);

            // Assert
            Assert.Equal(expectedName, level.Name);
        }

        [Theory]
        [InlineData(1, "Basic concepts suitable for beginners")]
        [InlineData(2, "Intermediate concepts requiring some background knowledge")]
        [InlineData(3, "Advanced concepts requiring strong foundational knowledge")]
        [InlineData(4, "Complex concepts requiring specialized knowledge and experience")]
        public void Description_ShouldMatchExpected(int id, string expectedDescription)
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromId<DifficultyLevel>(id);

            // Assert
            Assert.Equal(expectedDescription, level.Description);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        public void PointMultiplier_ShouldMatchId(int id, int expectedMultiplier)
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromId<DifficultyLevel>(id);

            // Assert
            Assert.Equal(expectedMultiplier, level.PointMultiplier);
        }

        [Fact]
        public void IsHarderThan_ShouldReturnTrue_WhenComparedToEasierLevel()
        {
            // Act & Assert
            Assert.True(DifficultyLevel.Hard.IsHarderThan(DifficultyLevel.Medium));
            Assert.True(DifficultyLevel.Expert.IsHarderThan(DifficultyLevel.Hard));
            Assert.True(DifficultyLevel.Medium.IsHarderThan(DifficultyLevel.Easy));
        }

        [Fact]
        public void IsHarderThan_ShouldReturnFalse_WhenComparedToHarderOrSameLevel()
        {
            // Act & Assert
            Assert.False(DifficultyLevel.Easy.IsHarderThan(DifficultyLevel.Medium));
            Assert.False(DifficultyLevel.Medium.IsHarderThan(DifficultyLevel.Hard));
            Assert.False(DifficultyLevel.Medium.IsHarderThan(DifficultyLevel.Medium));
        }

        [Fact]
        public void IsEasierThan_ShouldReturnTrue_WhenComparedToHarderLevel()
        {
            // Act & Assert
            Assert.True(DifficultyLevel.Medium.IsEasierThan(DifficultyLevel.Hard));
            Assert.True(DifficultyLevel.Easy.IsEasierThan(DifficultyLevel.Medium));
            Assert.True(DifficultyLevel.Hard.IsEasierThan(DifficultyLevel.Expert));
        }

        [Fact]
        public void IsEasierThan_ShouldReturnFalse_WhenComparedToEasierOrSameLevel()
        {
            // Act & Assert
            Assert.False(DifficultyLevel.Medium.IsEasierThan(DifficultyLevel.Easy));
            Assert.False(DifficultyLevel.Hard.IsEasierThan(DifficultyLevel.Medium));
            Assert.False(DifficultyLevel.Medium.IsEasierThan(DifficultyLevel.Medium));
        }

        [Theory]
        [MemberData(nameof(PointCalculations))]
        public void CalculatePoints_ShouldMultiplyByCorrectFactor(DifficultyLevel level, int basePoints, int expectedResult)
        {
            // Act
            var calculatedPoints = level.CalculatePoints(basePoints);

            // Assert
            Assert.Equal(expectedResult, calculatedPoints);
        }

        public static IEnumerable<object[]> PointCalculations()
        {
            yield return new object[] { DifficultyLevel.Easy, 100, 100 };
            yield return new object[] { DifficultyLevel.Medium, 100, 200 };
            yield return new object[] { DifficultyLevel.Hard, 100, 300 };
            yield return new object[] { DifficultyLevel.Expert, 100, 400 };
        }
    }
}