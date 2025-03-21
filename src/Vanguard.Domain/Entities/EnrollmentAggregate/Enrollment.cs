using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.Domain.Entities.EnrollmentAggregate
{
    public class Enrollment : AggregateRootBase<EnrollmentId>
    {
        private readonly Dictionary<LessonId, bool> _lessonCompletions = [];
        private readonly List<EnrollmentNote> _notes = [];
        private readonly List<EnrollmentCertificate> _certificates = [];

        public UserId UserId { get; private set; } = null!;
        public CourseId CourseId { get; private set; } = null!;
        public EnrollmentStatus Status { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? LastAccessedAt { get; private set; }
        public int ProgressPercentage { get; private set; }
        public LessonId? CurrentLessonId { get; private set; }
        public int? FinalGrade { get; private set; }
        public int CompletedAssignments { get; private set; } = 0;
        public int CompletedQuizzes { get; private set; } = 0;
        public bool HasPassedFinalExam { get; private set; }
        public bool HasTakenFinalExam => FinalExamScore > 0;
        public int FinalExamScore { get; private set; }
        public int DiscussionPostCount { get; private set; }

        public IReadOnlyCollection<EnrollmentCertificate> Certificates => _certificates.AsReadOnly();
        public IReadOnlyDictionary<LessonId, bool> LessonCompletions => _lessonCompletions;
        public IReadOnlyCollection<EnrollmentNote> Notes => _notes.AsReadOnly();

        // Navigation properties for EF Core
        public virtual Course? Course { get; private set; }
        public virtual User? User { get; private set; }

        private Enrollment() { } // For EF Core

        private Enrollment(EnrollmentId id, UserId userId, CourseId courseId) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(userId, nameof(userId), "User ID cannot be null");
            Guard.Against.Null(courseId, nameof(courseId), "Course ID cannot be null");

            UserId = userId;
            CourseId = courseId;
            Status = EnrollmentStatus.Active;
            ProgressPercentage = 0;
            LastAccessedAt = DateTime.UtcNow;
        }

        public static Enrollment Create(UserId userId, CourseId courseId)
        {
            return new Enrollment(EnrollmentId.CreateUnique(), userId, courseId);
        }

        public void UpdateLastAccessed(LessonId? currentLessonId = null)
        {
            LastAccessedAt = DateTime.UtcNow;

            if (currentLessonId != null)
            {
                CurrentLessonId = currentLessonId;
            }

            ModifiedAt = DateTime.UtcNow;
        }

        public void MarkLessonComplete(LessonId lessonId, int totalLessonsCount)
        {
            Guard.Against.Null(lessonId, nameof(lessonId), "Lesson ID cannot be null");
            Guard.Against.NegativeOrZero(totalLessonsCount, nameof(totalLessonsCount), "Total lessons count must be positive");

            _lessonCompletions[lessonId] = true;
            UpdateProgress(totalLessonsCount);
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new LessonCompletedEvent(Id, lessonId));
        }

        public void MarkLessonIncomplete(LessonId lessonId, int totalLessonsCount)
        {
            Guard.Against.Null(lessonId, nameof(lessonId), "Lesson ID cannot be null");
            Guard.Against.NegativeOrZero(totalLessonsCount, nameof(totalLessonsCount), "Total lessons count must be positive");

            if (_lessonCompletions.ContainsKey(lessonId))
            {
                _lessonCompletions[lessonId] = false;
                UpdateProgress(totalLessonsCount);
                ModifiedAt = DateTime.UtcNow;

                // If was completed but now not all lessons are complete, revert to active
                if (Status == EnrollmentStatus.Completed && ProgressPercentage < 100)
                {
                    Status = EnrollmentStatus.Active;
                    CompletedAt = null;
                }
            }
        }

        private void UpdateProgress(int totalLessonsCount)
        {
            if (totalLessonsCount <= 0)
                return;

            int completedCount = _lessonCompletions.Count(kv => kv.Value);
            ProgressPercentage = (int)Math.Round((double)completedCount / totalLessonsCount * 100);
        }

        public void Complete()
        {
            Status = EnrollmentStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            ProgressPercentage = 100;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new EnrollmentCompletedEvent(Id, UserId, CourseId));
        }

        public void Drop()
        {
            Status = EnrollmentStatus.Dropped;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new EnrollmentDroppedEvent(Id, UserId, CourseId));
        }

        public void PutOnHold()
        {
            Status = EnrollmentStatus.OnHold;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new EnrollmentStatusChangedEvent(Id, Status));
        }

        public void Reactivate()
        {
            Status = EnrollmentStatus.Active;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new EnrollmentStatusChangedEvent(Id, Status));
        }

        public void AddNote(string content)
        {
            Guard.Against.NullOrWhiteSpace(content, nameof(content), "Note content cannot be empty");

            var note = EnrollmentNote.Create(content);
            _notes.Add(note);
            ModifiedAt = DateTime.UtcNow;
        }

        public void RemoveNote(Guid noteId)
        {
            var note = _notes.FirstOrDefault(n => n.Id == noteId);
            Guard.Against.Null(note, nameof(note), $"Note with ID {noteId} not found");

            _notes.Remove(note);
            ModifiedAt = DateTime.UtcNow;
        }

        public EnrollmentCertificate IssueCertificate(
            string title,
            string description,
            string certificateNumber,
            DateTime? expiresAt = null,
            UserId? issuedById = null,
            string signatureImageUrl = "")
        {
            if (Status != EnrollmentStatus.Completed)
            {
                throw new InvalidOperationException("Cannot issue certificate for incomplete enrollment");
            }

            var certificate = EnrollmentCertificate.Issue(
                this,
                title,
                description,
                certificateNumber,
                expiresAt,
                issuedById,
                signatureImageUrl);

            _certificates.Add(certificate);
            ModifiedAt = DateTime.UtcNow;

            return certificate;
        }

        // Method to set final grade
        public void SetFinalGrade(int grade)
        {
            Guard.Against.OutOfRange(grade, nameof(grade), 0, 100, "Grade must be between 0 and 100");

            FinalGrade = grade;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new EnrollmentGradeSetEvent(Id, UserId, CourseId, grade));
        }

        // Methods to track assignments and quizzes
        public void CompleteAssignment()
        {
            CompletedAssignments++;
            ModifiedAt = DateTime.UtcNow;
        }

        public void CompleteQuiz()
        {
            CompletedQuizzes++;
            ModifiedAt = DateTime.UtcNow;
        }

        // Method to check if all requirements are satisfied
        public bool CheckAllRequirementsSatisfied(IEnumerable<CompletionRequirement> requirements)
        {
            Guard.Against.Null(requirements, nameof(requirements));

            // Check if all required requirements are satisfied
            var requiredRequirements = requirements.Where(r => r.IsRequired);

            return !requiredRequirements.Any() || requiredRequirements.All(r => r.IsSatisfiedBy(this));
        }

        // Method to auto-complete based on requirements
        public bool TryCompleteBasedOnRequirements(IEnumerable<CompletionRequirement> requirements)
        {
            if (Status == EnrollmentStatus.Completed)
            {
                return true; // Already completed
            }

            if (CheckAllRequirementsSatisfied(requirements))
            {
                Complete();
                return true;
            }

            return false;
        }

        // Method to set final exam score
        public void SetFinalExamScore(int score)
        {
            Guard.Against.OutOfRange(score, nameof(score), 0, 100, "Score must be between 0 and 100");
            FinalExamScore = score;
            HasPassedFinalExam = score >= 70;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new EnrollmentFinalExamScoreSetEvent(Id, UserId, CourseId, score));
        }

        // Method to track discussion posts
        public void AddDiscussionPost()
        {
            DiscussionPostCount++;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
