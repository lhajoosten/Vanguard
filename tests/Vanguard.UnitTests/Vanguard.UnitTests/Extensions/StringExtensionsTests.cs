using Vanguard.Common.Extensions;

namespace Vanguard.UnitTests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData("Hello World", "hello-world")]
        [InlineData("Hello  World", "hello-world")]
        [InlineData("Hello-World", "hello-world")]
        [InlineData("Hello_World", "helloworld")]
        [InlineData("Hello 123 World", "hello-123-world")]
        [InlineData("Héllö Wórld", "hello-world")]
        [InlineData("Hello,World!", "helloworld")]
        [InlineData("Hello - World", "hello-world")]
        [InlineData("---Hello World---", "hello-world")]
        public void ToSlug_Should_Create_Valid_Slug(string input, string expectedOutput)
        {
            // Act
            var result = input.ToSlug();

            // Assert
            Assert.Equal(expectedOutput, result);
        }

        [Theory]
        [InlineData("Hello", 10, "...", "Hello")]
        [InlineData("Hello World", 5, "...", "Hello...")]
        [InlineData("Hello World", 7, "...", "Hello...")]
        [InlineData("Hello World", 11, "...", "Hello World")]
        [InlineData("Hello World", 5, ".", "Hello.")]
        [InlineData("Hello World", 5, "", "Hello")]
        [InlineData("Hello World", 0, "...", "...")]
        [InlineData("Hello World", 1, "...", "...")]
        [InlineData("Hello World", 2, "...", "...")]
        [InlineData("Hello World", 3, "...", "...")]
        [InlineData(null, 5, "...", null)]
        [InlineData("", 5, "...", "")]
        public void Truncate_Should_Truncate_String_Correctly(string input, int maxLength, string suffix, string expectedOutput)
        {
            // Act
            var result = input.Truncate(maxLength, suffix);

            // Assert
            Assert.Equal(expectedOutput, result);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("test", null, false)]
        [InlineData("", "", false)]
        [InlineData("Hello World", "hello", true)]
        [InlineData("Hello World", "WORLD", true)]
        [InlineData("Hello World", "universe", false)]
        [InlineData("Hello World", "Hello World", true)]
        [InlineData("Hello World", "HELLO WORLD", true)]
        [InlineData("Hello World", "  hello  ", true)]
        public void ContainsIgnoreCase_Should_Check_Correctly(string text, string value, bool expectedResult)
        {
            // Act
            var result = text.ContainsIgnoreCase(value);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData(null, "test", false)]
        [InlineData("test", null, false)]
        [InlineData("", "", true)]
        [InlineData("hello", "hello", true)]
        [InlineData("hello", "HELLO", true)]
        [InlineData("hello", "world", false)]
        [InlineData("Hello World", "hello world", true)]
        [InlineData("Hello World", "HELLO WORLD", true)]
        [InlineData("Hello World", "Hello  World", false)]
        public void EqualsIgnoreCase_Should_Compare_Correctly(string text, string value, bool expectedResult)
        {
            // Act
            var result = text.EqualsIgnoreCase(value);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}