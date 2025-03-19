using Vanguard.Common.Base;
using Vanguard.Domain.Entities.EnrollmentAggregate;

namespace Vanguard.Domain.Enumerations
{
    public class CompletionCriteria : Enumeration
    {
        public readonly static CompletionCriteria PercentageComplete = new(1, "Percentage Complete", "Progress percentage (0-100)", "progress");
        public readonly static CompletionCriteria LessonsCompleted = new(2, "Lessons Completed", "Number of lessons completed", "lessons");
        public readonly static CompletionCriteria MinimumGrade = new(3, "Minimum Grade", "Minimum final grade required (0-100)", "grade");
        public readonly static CompletionCriteria MinimumDaysActive = new(4, "Minimum Days Active", "Minimum number of days enrolled", "days");
        public readonly static CompletionCriteria AssignmentsCompleted = new(5, "Assignments Completed", "Number of assignments completed", "assignments");
        public readonly static CompletionCriteria QuizzesCompleted = new(6, "Quizzes Completed", "Number of quizzes completed", "quizzes");
        public readonly static CompletionCriteria PassFinalExam = new(7, "Pass Final Exam", "Must pass the final examination", "exam");
        public readonly static CompletionCriteria ParticipateInDiscussions = new(8, "Participate in Discussions", "Minimum number of discussion posts", "discussions");

        public string Description { get; private set; }
        public string SystemName { get; private set; }

        private CompletionCriteria(int id, string name, string description, string systemName) : base(id, name)
        {
            Description = description;
            SystemName = systemName;
        }

        // Methods for criteria evaluation
        public bool IsSatisfiedBy(Enrollment enrollment, int requiredValue)
        {
            if (enrollment == null)
                return false;

            return this switch
            {
                var c when c == PercentageComplete => enrollment.ProgressPercentage >= requiredValue,
                var c when c == LessonsCompleted => enrollment.LessonCompletions.Count(kv => kv.Value) >= requiredValue,
                var c when c == MinimumGrade => enrollment.FinalGrade.HasValue && enrollment.FinalGrade.Value >= requiredValue,
                var c when c == MinimumDaysActive => (DateTime.UtcNow - enrollment.CreatedAt).Days >= requiredValue,
                var c when c == AssignmentsCompleted => enrollment.CompletedAssignments >= requiredValue,
                var c when c == QuizzesCompleted => enrollment.CompletedQuizzes >= requiredValue,
                var c when c == PassFinalExam => enrollment.HasPassedFinalExam && enrollment.FinalExamScore >= requiredValue,
                var c when c == ParticipateInDiscussions => enrollment.DiscussionPostCount >= requiredValue,
                _ => false
            };
        }

        public string GetRequirementDescription(int requiredValue)
        {
            return this switch
            {
                var c when c == PercentageComplete => $"Complete {requiredValue}% of the course",
                var c when c == LessonsCompleted => $"Complete {requiredValue} lessons",
                var c when c == MinimumGrade => $"Achieve a grade of at least {requiredValue}%",
                var c when c == MinimumDaysActive => $"Be actively enrolled for at least {requiredValue} days",
                var c when c == AssignmentsCompleted => $"Complete {requiredValue} assignments",
                var c when c == QuizzesCompleted => $"Complete {requiredValue} quizzes",
                var c when c == PassFinalExam => $"Pass the final exam with at least {requiredValue}%",
                var c when c == ParticipateInDiscussions => $"Make at least {requiredValue} discussion posts",
                _ => $"Meet {Name} requirement: {requiredValue}"
            };
        }

        public string GetProgressDescription(Enrollment enrollment, int requiredValue)
        {
            if (enrollment == null)
                return "Not started";

            return this switch
            {
                var c when c == PercentageComplete => $"{enrollment.ProgressPercentage}% / {requiredValue}%",
                var c when c == LessonsCompleted => $"{enrollment.LessonCompletions.Count(kv => kv.Value)} / {requiredValue} lessons",
                var c when c == MinimumGrade => enrollment.FinalGrade.HasValue
                    ? $"Grade: {enrollment.FinalGrade.Value}% / {requiredValue}%"
                    : "No grade yet",
                var c when c == MinimumDaysActive => $"{(DateTime.UtcNow - enrollment.CreatedAt).Days} / {requiredValue} days",
                var c when c == AssignmentsCompleted => $"{enrollment.CompletedAssignments} / {requiredValue} assignments",
                var c when c == QuizzesCompleted => $"{enrollment.CompletedQuizzes} / {requiredValue} quizzes",
                var c when c == PassFinalExam => enrollment.HasTakenFinalExam
                    ? $"Exam score: {enrollment.FinalExamScore}% / {requiredValue}%"
                    : "Final exam not taken",
                var c when c == ParticipateInDiscussions => $"{enrollment.DiscussionPostCount} / {requiredValue} posts",
                _ => "Progress unknown"
            };
        }
    }
}
