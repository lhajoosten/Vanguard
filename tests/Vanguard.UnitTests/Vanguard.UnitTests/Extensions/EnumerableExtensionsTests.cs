using Vanguard.Common.Extensions;

namespace Vanguard.UnitTests.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_Should_Return_True_For_Null_Collection()
        {
            // Arrange
            IEnumerable<string> collection = null;

            // Act
            var result = collection.IsNullOrEmpty();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_True_For_Empty_Collection()
        {
            // Arrange
            var collection = Enumerable.Empty<int>();

            // Act
            var result = collection.IsNullOrEmpty();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_False_For_Non_Empty_Collection()
        {
            // Arrange
            var collection = new List<string> { "item1", "item2" };

            // Act
            var result = collection.IsNullOrEmpty();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ToReadOnlyList_Should_Return_Empty_Collection_For_Null_Input()
        {
            // Arrange
            IEnumerable<int> collection = null;

            // Act
            var result = collection.ToReadOnlyList();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.IsAssignableFrom<IReadOnlyList<int>>(result);
        }

        [Fact]
        public void ToReadOnlyList_Should_Return_ReadOnlyList_For_Array()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };

            // Act
            var result = array.ToReadOnlyList();

            // Assert
            Assert.IsAssignableFrom<IReadOnlyList<int>>(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0]);
            Assert.Equal(2, result[1]);
            Assert.Equal(3, result[2]);
        }

        [Fact]
        public void ToReadOnlyList_Should_Return_ReadOnlyList_For_List()
        {
            // Arrange
            var list = new List<string> { "a", "b", "c" };

            // Act
            var result = list.ToReadOnlyList();

            // Assert
            Assert.IsAssignableFrom<IReadOnlyList<string>>(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("a", result[0]);
            Assert.Equal("b", result[1]);
            Assert.Equal("c", result[2]);
        }

        [Fact]
        public void ToReadOnlyList_Should_Return_Same_Instance_For_IReadOnlyList()
        {
            // Arrange
            IReadOnlyList<int> readOnlyList = new List<int> { 1, 2, 3 }.AsReadOnly();

            // Act
            var result = readOnlyList.ToReadOnlyList();

            // Assert
            Assert.Same(readOnlyList, result);
        }

        [Fact]
        public void ToReadOnlyList_Should_Convert_IEnumerable_To_ReadOnlyList()
        {
            // Arrange
            IEnumerable<int> enumerable = Enumerable.Range(1, 5);

            // Act
            var result = enumerable.ToReadOnlyList();

            // Assert
            Assert.IsAssignableFrom<IReadOnlyList<int>>(result);
            Assert.Equal(5, result.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(i + 1, result[i]);
            }
        }
    }
}