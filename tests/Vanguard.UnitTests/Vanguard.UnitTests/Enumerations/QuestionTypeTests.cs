using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class QuestionTypeTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllQuestionTypes()
        {
            // Act
            var allTypes = Vanguard.Common.Base.Enumeration.GetAll<QuestionType>();

            // Assert
            Assert.Equal(8, allTypes.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectQuestionType()
        {
            // Act
            var questionType = Vanguard.Common.Base.Enumeration.FromId<QuestionType>(3);

            // Assert
            Assert.Equal(QuestionType.OpenText, questionType);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectQuestionType()
        {
            // Act
            var questionType = Vanguard.Common.Base.Enumeration.FromName<QuestionType>("True/False");

            // Assert
            Assert.Equal(QuestionType.TrueFalse, questionType);
        }

        [Theory]
        [InlineData(1, "Single Choice")]
        [InlineData(2, "Multiple Choice")]
        [InlineData(3, "Open Text")]
        [InlineData(4, "True/False")]
        [InlineData(5, "Matching")]
        [InlineData(6, "Ordering")]
        [InlineData(7, "Fill in the Blank")]
        [InlineData(8, "Hotspot")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var questionType = Vanguard.Common.Base.Enumeration.FromId<QuestionType>(id);

            // Assert
            Assert.Equal(expectedName, questionType.Name);
        }

        [Theory]
        [InlineData(1, "Radio buttons with one correct answer")]
        [InlineData(2, "Checkboxes with multiple correct answers")]
        [InlineData(3, "Free text response for manual grading")]
        [InlineData(4, "Simple true or false question")]
        [InlineData(5, "Match items from two columns")]
        [InlineData(6, "Arrange items in correct order")]
        [InlineData(7, "Complete sentence with missing words")]
        [InlineData(8, "Select correct areas on an image")]
        public void Description_ShouldMatchExpected(int id, string expectedDescription)
        {
            // Act
            var questionType = Vanguard.Common.Base.Enumeration.FromId<QuestionType>(id);

            // Assert
            Assert.Equal(expectedDescription, questionType.Description);
        }

        [Theory]
        [InlineData(1, "single_choice")]
        [InlineData(2, "multiple_choice")]
        [InlineData(3, "open_text")]
        [InlineData(4, "true_false")]
        [InlineData(5, "matching")]
        [InlineData(6, "ordering")]
        [InlineData(7, "fill_blank")]
        [InlineData(8, "hotspot")]
        public void TechnicalName_ShouldMatchExpected(int id, string expectedTechnicalName)
        {
            // Act
            var questionType = Vanguard.Common.Base.Enumeration.FromId<QuestionType>(id);

            // Assert
            Assert.Equal(expectedTechnicalName, questionType.TechnicalName);
        }

        [Fact]
        public void RequiresOptions_ShouldReturnTrueForAllExceptOpenText()
        {
            // Assert
            Assert.True(QuestionType.SingleChoice.RequiresOptions());
            Assert.True(QuestionType.MultipleChoice.RequiresOptions());
            Assert.True(QuestionType.TrueFalse.RequiresOptions());
            Assert.True(QuestionType.Matching.RequiresOptions());
            Assert.True(QuestionType.Ordering.RequiresOptions());
            Assert.True(QuestionType.FillInBlank.RequiresOptions());
            Assert.True(QuestionType.Hotspot.RequiresOptions());
            Assert.False(QuestionType.OpenText.RequiresOptions());
        }

        [Fact]
        public void AllowsMultipleSelections_ShouldReturnTrueForSelectTypes()
        {
            // Assert
            Assert.True(QuestionType.MultipleChoice.AllowsMultipleSelections());
            Assert.True(QuestionType.Matching.AllowsMultipleSelections());
            Assert.True(QuestionType.Ordering.AllowsMultipleSelections());
            Assert.False(QuestionType.SingleChoice.AllowsMultipleSelections());
            Assert.False(QuestionType.OpenText.AllowsMultipleSelections());
            Assert.False(QuestionType.TrueFalse.AllowsMultipleSelections());
            Assert.False(QuestionType.FillInBlank.AllowsMultipleSelections());
            Assert.False(QuestionType.Hotspot.AllowsMultipleSelections());
        }

        [Fact]
        public void RequiresManualGrading_ShouldReturnTrueOnlyForOpenText()
        {
            // Assert
            Assert.True(QuestionType.OpenText.RequiresManualGrading());
            Assert.False(QuestionType.SingleChoice.RequiresManualGrading());
            Assert.False(QuestionType.MultipleChoice.RequiresManualGrading());
            Assert.False(QuestionType.TrueFalse.RequiresManualGrading());
            Assert.False(QuestionType.Matching.RequiresManualGrading());
            Assert.False(QuestionType.Ordering.RequiresManualGrading());
            Assert.False(QuestionType.FillInBlank.RequiresManualGrading());
            Assert.False(QuestionType.Hotspot.RequiresManualGrading());
        }

        [Fact]
        public void IsBinary_ShouldReturnTrueOnlyForTrueFalse()
        {
            // Assert
            Assert.True(QuestionType.TrueFalse.IsBinary());
            Assert.False(QuestionType.SingleChoice.IsBinary());
            Assert.False(QuestionType.MultipleChoice.IsBinary());
            Assert.False(QuestionType.OpenText.IsBinary());
            Assert.False(QuestionType.Matching.IsBinary());
            Assert.False(QuestionType.Ordering.IsBinary());
            Assert.False(QuestionType.FillInBlank.IsBinary());
            Assert.False(QuestionType.Hotspot.IsBinary());
        }

        [Theory]
        [MemberData(nameof(ComponentData))]
        public void GetComponentName_ShouldReturnFormattedComponentName(QuestionType questionType, string expected)
        {
            // Act
            var componentName = questionType.GetComponentName();

            // Assert
            Assert.Equal(expected, componentName);
        }

        public static IEnumerable<object[]> ComponentData()
        {
            yield return new object[] { QuestionType.SingleChoice, "single_choice-question" };
            yield return new object[] { QuestionType.MultipleChoice, "multiple_choice-question" };
            yield return new object[] { QuestionType.OpenText, "open_text-question" };
            yield return new object[] { QuestionType.TrueFalse, "true_false-question" };
        }

        [Theory]
        [MemberData(nameof(EditorComponentData))]
        public void GetEditorComponentName_ShouldReturnFormattedEditorName(QuestionType questionType, string expected)
        {
            // Act
            var editorComponentName = questionType.GetEditorComponentName();

            // Assert
            Assert.Equal(expected, editorComponentName);
        }

        public static IEnumerable<object[]> EditorComponentData()
        {
            yield return new object[] { QuestionType.SingleChoice, "single_choice-question-editor" };
            yield return new object[] { QuestionType.MultipleChoice, "multiple_choice-question-editor" };
            yield return new object[] { QuestionType.OpenText, "open_text-question-editor" };
            yield return new object[] { QuestionType.TrueFalse, "true_false-question-editor" };
        }

        [Fact]
        public void SupportsPartialCredit_ShouldReturnTrueForSupportedTypes()
        {
            // Assert
            Assert.True(QuestionType.MultipleChoice.SupportsPartialCredit());
            Assert.True(QuestionType.Matching.SupportsPartialCredit());
            Assert.True(QuestionType.Ordering.SupportsPartialCredit());
            Assert.True(QuestionType.FillInBlank.SupportsPartialCredit());
            Assert.False(QuestionType.SingleChoice.SupportsPartialCredit());
            Assert.False(QuestionType.OpenText.SupportsPartialCredit());
            Assert.False(QuestionType.TrueFalse.SupportsPartialCredit());
            Assert.False(QuestionType.Hotspot.SupportsPartialCredit());
        }
    }
}