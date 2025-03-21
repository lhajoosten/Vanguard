using Vanguard.Common.Base;

namespace Vanguard.UnitTests.Common
{
    public class EnumerationTests
    {
        [Fact]
        public void GetAll_Should_Return_All_Enumeration_Values()
        {
            // Act
            var allColors = Enumeration.GetAll<TestColor>().ToList();

            // Assert
            Assert.Equal(3, allColors.Count);
            Assert.Contains(TestColor.Red, allColors);
            Assert.Contains(TestColor.Green, allColors);
            Assert.Contains(TestColor.Blue, allColors);
        }

        [Fact]
        public void FromId_Should_Return_Correct_Enumeration_Value()
        {
            // Act
            var color = Enumeration.FromId<TestColor>(2);

            // Assert
            Assert.Equal(TestColor.Green, color);
        }

        [Fact]
        public void FromId_Should_Throw_When_Id_Not_Found()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => Enumeration.FromId<TestColor>(99));
        }

        [Fact]
        public void FromName_Should_Return_Correct_Enumeration_Value()
        {
            // Act
            var color = Enumeration.FromName<TestColor>("Blue");

            // Assert
            Assert.Equal(TestColor.Blue, color);
        }

        [Fact]
        public void FromName_Should_Throw_When_Name_Not_Found()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => Enumeration.FromName<TestColor>("Purple"));
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Enumeration_Value()
        {
            // Arrange
            var color1 = TestColor.Red;
            var color2 = TestColor.Red;

            // Act & Assert
            Assert.True(color1.Equals(color2));
            Assert.True(color1 == color2);
            Assert.False(color1 != color2);
        }

        [Fact]
        public void Equals_Should_Return_False_For_Different_Enumeration_Values()
        {
            // Arrange
            var color1 = TestColor.Red;
            var color2 = TestColor.Blue;

            // Act & Assert
            Assert.False(color1.Equals(color2));
            Assert.False(color1 == color2);
            Assert.True(color1 != color2);
        }

        [Fact]
        public void Equals_Should_Return_False_For_Different_Types()
        {
            // Arrange
            var color = TestColor.Red;
            var size = TestSize.Small;

            // Act & Assert
            Assert.False(color.Equals(size));
        }

        [Fact]
        public void GetHashCode_Should_Return_Id_HashCode()
        {
            // Arrange
            var color = TestColor.Red;

            // Act & Assert
            Assert.Equal(1.GetHashCode(), color.GetHashCode());
        }

        [Fact]
        public void ToString_Should_Return_Name()
        {
            // Arrange
            var color = TestColor.Green;

            // Act & Assert
            Assert.Equal("Green", color.ToString());
        }

        [Fact]
        public void CompareTo_Should_Compare_By_Id()
        {
            // Arrange
            var red = TestColor.Red;
            var green = TestColor.Green;
            var blue = TestColor.Blue;

            // Act & Assert
            Assert.True(red.CompareTo(green) < 0);
            Assert.True(blue.CompareTo(green) > 0);
            Assert.Equal(0, red.CompareTo(red));
        }

        [Fact]
        public void Comparison_Operators_Should_Work_Correctly()
        {
            // Arrange
            var red = TestColor.Red;
            var green = TestColor.Green;
            var blue = TestColor.Blue;

            // Act & Assert
            Assert.True(red < green);
            Assert.True(red <= green);
            Assert.True(blue > green);
            Assert.True(blue >= green);
            Assert.True(red! <= red!);
            Assert.True(red! >= red!);
        }

        [Fact]
        public void Comparison_Operators_Should_Handle_Null_Values()
        {
            // Arrange
            var color = TestColor.Red;
            TestColor nullColor = null;

            // Act & Assert
            Assert.False(color < null!);
            Assert.False(color <= null!);
            Assert.True(color > null!);
            Assert.True(color >= null);
            Assert.False(null > color);
            Assert.True(null < color);
            Assert.True(null <= color);
            Assert.False(null >= color);
            Assert.True(nullColor == null);
            Assert.True(null == nullColor);
            Assert.False(nullColor != null);
            Assert.False(null != nullColor);
        }

        // Test classes for enumeration testing
        public class TestColor(int id, string name) : Enumeration(id, name)
        {
            public static readonly TestColor Red = new(1, "Red");
            public static readonly TestColor Green = new(2, "Green");
            public static readonly TestColor Blue = new(3, "Blue");
        }

        public class TestSize(int id, string name) : Enumeration(id, name)
        {
            public static readonly TestSize Small = new(1, "Small");
            public static readonly TestSize Medium = new(2, "Medium");
            public static readonly TestSize Large = new(3, "Large");
        }
    }
}