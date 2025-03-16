using Vanguard.Domain.Base;

namespace Vanguard.Domain.Enumerations
{
    public class QuestionType : Enumeration
    {
        public readonly static QuestionType SingleChoice = new(1, "Single Choice", "Radio buttons with one correct answer", "single_choice");
        public readonly static QuestionType MultipleChoice = new(2, "Multiple Choice", "Checkboxes with multiple correct answers", "multiple_choice");
        public readonly static QuestionType OpenText = new(3, "Open Text", "Free text response for manual grading", "open_text");
        public readonly static QuestionType TrueFalse = new(4, "True/False", "Simple true or false question", "true_false");
        public readonly static QuestionType Matching = new(5, "Matching", "Match items from two columns", "matching");
        public readonly static QuestionType Ordering = new(6, "Ordering", "Arrange items in correct order", "ordering");
        public readonly static QuestionType FillInBlank = new(7, "Fill in the Blank", "Complete sentence with missing words", "fill_blank");
        public readonly static QuestionType Hotspot = new(8, "Hotspot", "Select correct areas on an image", "hotspot");

        public string Description { get; private set; }
        public string TechnicalName { get; private set; }

        private QuestionType(int id, string name, string description, string technicalName) : base(id, name)
        {
            Description = description;
            TechnicalName = technicalName;
        }

        // Helper methods for question types
        public bool RequiresOptions()
        {
            return this != OpenText;
        }

        public bool AllowsMultipleSelections()
        {
            return this == MultipleChoice || this == Matching || this == Ordering;
        }

        public bool RequiresManualGrading()
        {
            return this == OpenText;
        }

        public bool IsBinary()
        {
            return this == TrueFalse;
        }

        public string GetComponentName()
        {
            return $"{TechnicalName}-question";
        }

        public string GetEditorComponentName()
        {
            return $"{TechnicalName}-question-editor";
        }

        public bool SupportsPartialCredit()
        {
            return this == MultipleChoice || this == Matching || this == Ordering || this == FillInBlank;
        }
    }
}
