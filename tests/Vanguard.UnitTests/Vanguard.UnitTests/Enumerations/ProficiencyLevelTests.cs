using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class ProficiencyLevelTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllProficiencyLevels()
        {
            // Act
            var allLevels = Vanguard.Common.Base.Enumeration.GetAll<ProficiencyLevel>();

            // Assert
            Assert.Equal(4, allLevels.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectProficiencyLevel()
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromId<ProficiencyLevel>(3);

            // Assert
            Assert.Equal(ProficiencyLevel.Advanced, level);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectProficiencyLevel()
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromName<ProficiencyLevel>("Intermediate");

            // Assert
            Assert.Equal(ProficiencyLevel.Intermediate, level);
        }

        [Theory]
        [InlineData(1, "Beginner")]
        [InlineData(2, "Intermediate")]
        [InlineData(3, "Advanced")]
        [InlineData(4, "Expert")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromId<ProficiencyLevel>(id);

            // Assert
            Assert.Equal(expectedName, level.Name);
        }

        [Theory]
        [InlineData(1, "Basic understanding, needs guidance")]
        [InlineData(2, "Working knowledge, can perform independently")]
        [InlineData(3, "Deep understanding, can teach others")]
        [InlineData(4, "Mastery, can innovate and contribute to the field")]
        public void Description_ShouldMatchExpected(int id, string expectedDescription)
        {
            // Act
            var level = Vanguard.Common.Base.Enumeration.FromId<ProficiencyLevel>(id);

            // Assert
            Assert.Equal(expectedDescription, level.Description);
        }

        [Fact]
        public void IsHigherThan_ShouldReturnTrue_WhenComparedToLowerLevel()
        {
            // Act & Assert
            Assert.True(ProficiencyLevel.Intermediate.IsHigherThan(ProficiencyLevel.Beginner));
            Assert.True(ProficiencyLevel.Advanced.IsHigherThan(ProficiencyLevel.Intermediate));
            Assert.True(ProficiencyLevel.Expert.IsHigherThan(ProficiencyLevel.Advanced));
        }

        [Fact]
        public void IsHigherThan_ShouldReturnFalse_WhenComparedToHigherOrSameLevel()
        {
            // Act & Assert
            Assert.False(ProficiencyLevel.Beginner.IsHigherThan(ProficiencyLevel.Intermediate));
            Assert.False(ProficiencyLevel.Intermediate.IsHigherThan(ProficiencyLevel.Advanced));
            Assert.False(ProficiencyLevel.Intermediate.IsHigherThan(ProficiencyLevel.Intermediate));
        }

        [Fact]
        public void IsEqualOrHigherThan_ShouldReturnTrue_WhenComparedToLowerOrSameLevel()
        {
            // Act & Assert
            Assert.True(ProficiencyLevel.Intermediate.IsEqualOrHigherThan(ProficiencyLevel.Beginner));
            Assert.True(ProficiencyLevel.Intermediate.IsEqualOrHigherThan(ProficiencyLevel.Intermediate));
            Assert.False(ProficiencyLevel.Intermediate.IsEqualOrHigherThan(ProficiencyLevel.Expert));
        }

        [Fact]
        public void IsLowerThan_ShouldReturnTrue_WhenComparedToHigherLevel()
        {
            // Act & Assert
            Assert.True(ProficiencyLevel.Beginner.IsLowerThan(ProficiencyLevel.Intermediate));
            Assert.True(ProficiencyLevel.Intermediate.IsLowerThan(ProficiencyLevel.Advanced));
            Assert.True(ProficiencyLevel.Advanced.IsLowerThan(ProficiencyLevel.Expert));
        }

        [Fact]
        public void IsLowerThan_ShouldReturnFalse_WhenComparedToLowerOrSameLevel()
        {
            // Act & Assert
            Assert.False(ProficiencyLevel.Intermediate.IsLowerThan(ProficiencyLevel.Beginner));
            Assert.False(ProficiencyLevel.Advanced.IsLowerThan(ProficiencyLevel.Intermediate));
            Assert.False(ProficiencyLevel.Intermediate.IsLowerThan(ProficiencyLevel.Intermediate));
        }

        [Fact]
        public void IsEqualOrLowerThan_ShouldReturnTrue_WhenComparedToHigherOrSameLevel()
        {
            // Act & Assert
            Assert.True(ProficiencyLevel.Beginner.IsEqualOrLowerThan(ProficiencyLevel.Intermediate));
            Assert.True(ProficiencyLevel.Intermediate.IsEqualOrLowerThan(ProficiencyLevel.Intermediate));
            Assert.False(ProficiencyLevel.Expert.IsEqualOrLowerThan(ProficiencyLevel.Advanced));
        }
    }
}