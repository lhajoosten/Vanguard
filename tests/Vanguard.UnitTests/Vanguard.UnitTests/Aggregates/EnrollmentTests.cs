using Vanguard.Common.Base;
using Vanguard.Domain.Entities.EnrollmentAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.UnitTests.Aggregates
{
    public class EnrollmentTests
    {
        private readonly UserId _userId = UserId.CreateUnique();
        private readonly CourseId _courseId = CourseId.CreateUnique();

        [Fact]
        public void Create_ShouldCreateNewEnrollment_WithCorrectProperties()
        {
            // Act
            var enrollment = Enrollment.Create(_userId, _courseId);

            // Assert
            Assert.NotNull(enrollment);
            Assert.Equal(_userId, enrollment.UserId);
            Assert.Equal(_courseId, enrollment.CourseId);
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
            Assert.Null(enrollment.CompletedAt);
            Assert.NotNull(enrollment.LastAccessedAt);
            Assert.Equal(0, enrollment.ProgressPercentage);
            Assert.Null(enrollment.CurrentLessonId);
            Assert.Null(enrollment.FinalGrade);
            Assert.Equal(0, enrollment.CompletedAssignments);
            Assert.Equal(0, enrollment.CompletedQuizzes);
            Assert.False(enrollment.HasPassedFinalExam);
            Assert.False(enrollment.HasTakenFinalExam);
            Assert.Equal(0, enrollment.FinalExamScore);
            Assert.Equal(0, enrollment.DiscussionPostCount);
            Assert.Empty(enrollment.Certificates);
            Assert.Empty(enrollment.LessonCompletions);
            Assert.Empty(enrollment.Notes);
        }

        [Fact]
        public void Create_ShouldThrowException_WhenUserIdIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Enrollment.Create(null, _courseId));
        }

        [Fact]
        public void Create_ShouldThrowException_WhenCourseIdIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Enrollment.Create(_userId, null));
        }

        [Fact]
        public void UpdateLastAccessed_ShouldUpdateTimestamp()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var originalTimestamp = enrollment.LastAccessedAt;

            // Force delay to ensure timestamp difference
            Thread.Sleep(1);

            // Act
            enrollment.UpdateLastAccessed();

            // Assert
            Assert.NotEqual(originalTimestamp, enrollment.LastAccessedAt);
            Assert.True(enrollment.LastAccessedAt > originalTimestamp);
        }

        [Fact]
        public void UpdateLastAccessed_ShouldUpdateCurrentLesson_WhenLessonIdProvided()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var lessonId = LessonId.CreateUnique();

            // Act
            enrollment.UpdateLastAccessed(lessonId);

            // Assert
            Assert.Equal(lessonId, enrollment.CurrentLessonId);
        }

        [Fact]
        public void MarkLessonComplete_ShouldUpdateLessonCompletions()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var lessonId = LessonId.CreateUnique();
            int totalLessons = 4;

            // Act
            enrollment.MarkLessonComplete(lessonId, totalLessons);

            // Assert
            Assert.Single(enrollment.LessonCompletions);
            Assert.True(enrollment.LessonCompletions[lessonId]);
            Assert.Equal(25, enrollment.ProgressPercentage); // 1/4 = 25%

            // Verify domain event
            var domainEvent = enrollment.DomainEvents.Last();
            var lessonCompletedEvent = Assert.IsType<LessonCompletedEvent>(domainEvent);
            Assert.Equal(enrollment.Id, lessonCompletedEvent.EnrollmentId);
            Assert.Equal(lessonId, lessonCompletedEvent.LessonId);
        }

        [Fact]
        public void MarkLessonComplete_ShouldUpdateProgressToOneHundredPercent_WhenAllLessonsCompleted()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var lessonId = LessonId.CreateUnique();
            int totalLessons = 1; // Only one lesson

            // Act
            enrollment.MarkLessonComplete(lessonId, totalLessons);

            // Assert
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status); // Status stays Active
            Assert.Equal(100, enrollment.ProgressPercentage);
            Assert.Null(enrollment.CompletedAt);

            // Verify domain events - should only have LessonCompletedEvent, not EnrollmentCompletedEvent
            var lastDomainEvent = enrollment.DomainEvents.Last();
            Assert.IsType<LessonCompletedEvent>(lastDomainEvent);
        }

        [Fact]
        public void MarkLessonComplete_ShouldThrowException_WhenLessonIdIsNull()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => enrollment.MarkLessonComplete(null, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void MarkLessonComplete_ShouldThrowException_WhenTotalLessonsIsNotPositive(int totalLessons)
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var lessonId = LessonId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => enrollment.MarkLessonComplete(lessonId, totalLessons));
        }

        [Fact]
        public void MarkLessonIncomplete_ShouldUpdateLessonCompletions()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var lessonId = LessonId.CreateUnique();
            int totalLessons = 4;

            // First mark as complete
            enrollment.MarkLessonComplete(lessonId, totalLessons);
            Assert.True(enrollment.LessonCompletions[lessonId]);
            Assert.Equal(25, enrollment.ProgressPercentage);

            // Act - Mark as incomplete
            enrollment.MarkLessonIncomplete(lessonId, totalLessons);

            // Assert
            Assert.False(enrollment.LessonCompletions[lessonId]);
            Assert.Equal(0, enrollment.ProgressPercentage);
        }

        [Fact]
        public void MarkLessonIncomplete_ShouldRevertCompletedStatus_WhenWasPreviouslyCompleted()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var lessonId = LessonId.CreateUnique();
            int totalLessons = 1; // Only one lesson

            // Mark lesson as complete
            enrollment.MarkLessonComplete(lessonId, totalLessons);

            // Complete enrollment through requirements
            var req = CompletionRequirement.Create(
                _courseId,
                CompletionCriteria.AllLessonsCompleted,
                100,
                "Complete all lessons",
                true,
                0);

            enrollment.TryCompleteBasedOnRequirements(new List<CompletionRequirement> { req });
            Assert.Equal(EnrollmentStatus.Completed, enrollment.Status);
            Assert.NotNull(enrollment.CompletedAt);

            // Act - Mark as incomplete
            enrollment.MarkLessonIncomplete(lessonId, totalLessons);

            // Assert
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
            Assert.Null(enrollment.CompletedAt);
        }

        [Fact]
        public void MarkLessonIncomplete_ShouldDoNothing_WhenLessonNotMarkedComplete()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var lessonId = LessonId.CreateUnique();
            int totalLessons = 4;

            // Act
            enrollment.MarkLessonIncomplete(lessonId, totalLessons);

            // Assert
            Assert.DoesNotContain(lessonId, enrollment.LessonCompletions.Keys);
            Assert.Equal(0, enrollment.ProgressPercentage);
        }

        [Fact]
        public void Complete_ShouldUpdateEnrollmentStatus()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Act
            enrollment.Complete();

            // Assert
            Assert.Equal(EnrollmentStatus.Completed, enrollment.Status);
            Assert.NotNull(enrollment.CompletedAt);
            Assert.Equal(100, enrollment.ProgressPercentage);

            // Verify domain event
            var domainEvent = enrollment.DomainEvents.Last();
            var completedEvent = Assert.IsType<EnrollmentCompletedEvent>(domainEvent);
            Assert.Equal(enrollment.Id, completedEvent.EnrollmentId);
            Assert.Equal(enrollment.UserId, completedEvent.UserId);
            Assert.Equal(enrollment.CourseId, completedEvent.CourseId);
        }

        [Fact]
        public void Drop_ShouldUpdateEnrollmentStatus()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Act
            enrollment.Drop();

            // Assert
            Assert.Equal(EnrollmentStatus.Dropped, enrollment.Status);

            // Verify domain event
            var domainEvent = enrollment.DomainEvents.Last();
            var droppedEvent = Assert.IsType<EnrollmentDroppedEvent>(domainEvent);
            Assert.Equal(enrollment.Id, droppedEvent.EnrollmentId);
            Assert.Equal(enrollment.UserId, droppedEvent.UserId);
            Assert.Equal(enrollment.CourseId, droppedEvent.CourseId);
        }

        [Fact]
        public void PutOnHold_ShouldUpdateEnrollmentStatus()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Act
            enrollment.PutOnHold();

            // Assert
            Assert.Equal(EnrollmentStatus.OnHold, enrollment.Status);

            // Verify domain event
            var domainEvent = enrollment.DomainEvents.Last();
            var statusChangedEvent = Assert.IsType<EnrollmentStatusChangedEvent>(domainEvent);
            Assert.Equal(enrollment.Id, statusChangedEvent.EnrollmentId);
            Assert.Equal(EnrollmentStatus.OnHold, statusChangedEvent.Status);
        }

        [Fact]
        public void Reactivate_ShouldUpdateEnrollmentStatus()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            enrollment.PutOnHold();
            Assert.Equal(EnrollmentStatus.OnHold, enrollment.Status);

            // Act
            enrollment.Reactivate();

            // Assert
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status);

            // Verify domain event
            var domainEvent = enrollment.DomainEvents.Last();
            var statusChangedEvent = Assert.IsType<EnrollmentStatusChangedEvent>(domainEvent);
            Assert.Equal(enrollment.Id, statusChangedEvent.EnrollmentId);
            Assert.Equal(EnrollmentStatus.Active, statusChangedEvent.Status);
        }

        [Fact]
        public void AddNote_ShouldAddNoteToEnrollment()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            string noteContent = "Test note content";

            // Act
            enrollment.AddNote(noteContent);

            // Assert
            Assert.Single(enrollment.Notes);
            var note = enrollment.Notes.First();
            Assert.Equal(noteContent, note.Content);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void AddNote_ShouldThrowExceptionForInvalidContent(string content)
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                enrollment.AddNote(content));

            if (content == null)
            {
                Assert.IsType<ArgumentNullException>(exception);
            }
            else
            {
                Assert.IsType<ArgumentException>(exception);
            }
        }

        [Fact]
        public void RemoveNote_ShouldRemoveNoteFromEnrollment()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            enrollment.AddNote("Test note content");
            var note = enrollment.Notes.First();

            // Act
            enrollment.RemoveNote(note.Id);

            // Assert
            Assert.Empty(enrollment.Notes);
        }

        [Fact]
        public void RemoveNote_ShouldThrowException_WhenNoteDoesNotExist()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            var nonExistentNoteId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => enrollment.RemoveNote(nonExistentNoteId));
        }

        [Fact]
        public void IssueCertificate_ShouldCreateCertificate_WhenEnrollmentIsCompleted()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            enrollment.Complete();

            string title = "Course Completion";
            string description = "Certificate of completion";
            string certificateNumber = "CERT-001";

            // Act
            var certificate = enrollment.IssueCertificate(
                title,
                description,
                certificateNumber);

            // Assert
            Assert.NotNull(certificate);
            Assert.Equal(enrollment.Id, certificate.EnrollmentId);
            Assert.Equal(title, certificate.Title);
            Assert.Equal(description, certificate.Description);
            Assert.Equal(certificateNumber, certificate.CertificateNumber);
            Assert.Null(certificate.ExpiresAt);
            Assert.Null(certificate.IssuedById);
            Assert.Equal(string.Empty, certificate.SignatureImageUrl);
            Assert.Single(enrollment.Certificates);
            Assert.Contains(certificate, enrollment.Certificates);
        }

        [Fact]
        public void IssueCertificate_ShouldCreateCertificateWithAllProperties()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            enrollment.Complete();

            string title = "Course Completion";
            string description = "Certificate of completion";
            string certificateNumber = "CERT-001";
            DateTime expiresAt = DateTime.UtcNow.AddYears(1);
            var issuedById = UserId.CreateUnique();
            string signatureUrl = "https://example.com/signature.png";

            // Act
            var certificate = enrollment.IssueCertificate(
                title,
                description,
                certificateNumber,
                expiresAt,
                issuedById,
                signatureUrl);

            // Assert
            Assert.Equal(expiresAt, certificate.ExpiresAt);
            Assert.Equal(issuedById, certificate.IssuedById);
            Assert.Equal(signatureUrl, certificate.SignatureImageUrl);
        }

        [Fact]
        public void IssueCertificate_ShouldThrowException_WhenEnrollmentIsNotCompleted()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                enrollment.IssueCertificate(
                    "Course Completion",
                    "Certificate of completion",
                    "CERT-001"));

            Assert.Contains("Cannot issue certificate for incomplete enrollment", exception.Message);
        }

        [Fact]
        public void SetFinalGrade_ShouldUpdateGrade()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            int grade = 85;

            // Act
            enrollment.SetFinalGrade(grade);

            // Assert
            Assert.Equal(grade, enrollment.FinalGrade);

            // Verify domain event
            var domainEvent = enrollment.DomainEvents.Last();
            var gradeSetEvent = Assert.IsType<EnrollmentGradeSetEvent>(domainEvent);
            Assert.Equal(enrollment.Id, gradeSetEvent.EnrollmentId);
            Assert.Equal(enrollment.UserId, gradeSetEvent.UserId);
            Assert.Equal(enrollment.CourseId, gradeSetEvent.CourseId);
            Assert.Equal(grade, gradeSetEvent.Grade);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void SetFinalGrade_ShouldThrowException_WhenGradeIsOutOfRange(int grade)
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => enrollment.SetFinalGrade(grade));
        }

        [Fact]
        public void CompleteAssignment_ShouldIncrementCounter()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            int initialCount = enrollment.CompletedAssignments;

            // Act
            enrollment.CompleteAssignment();

            // Assert
            Assert.Equal(initialCount + 1, enrollment.CompletedAssignments);
        }

        [Fact]
        public void CompleteQuiz_ShouldIncrementCounter()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            int initialCount = enrollment.CompletedQuizzes;

            // Act
            enrollment.CompleteQuiz();

            // Assert
            Assert.Equal(initialCount + 1, enrollment.CompletedQuizzes);
        }

        [Fact]
        public void CheckAllRequirementsSatisfied_ShouldReturnTrue_WhenAllRequirementsAreSatisfied()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Set up completed lesson
            var lessonId = LessonId.CreateUnique();
            enrollment.MarkLessonComplete(lessonId, 1);

            // Set up a final grade
            enrollment.SetFinalGrade(85);

            // Set up requirements
            var req1 = CompletionRequirement.Create(
                _courseId,
                CompletionCriteria.MinimumGrade,
                80,
                "Minimum grade of 80%",
                true,
                0);

            var req2 = CompletionRequirement.Create(
                _courseId,
                CompletionCriteria.PercentageComplete,
                100,
                "Complete all lessons",
                true,
                1);

            var requirements = new List<CompletionRequirement> { req1, req2 };

            // Act
            bool result = enrollment.CheckAllRequirementsSatisfied(requirements);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckAllRequirementsSatisfied_ShouldReturnFalse_WhenAnyRequiredRequirementIsNotSatisfied()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Set up completed lesson
            var lessonId = LessonId.CreateUnique();
            enrollment.MarkLessonComplete(lessonId, 1);

            // Set up a final grade that doesn't meet the requirement
            enrollment.SetFinalGrade(75);

            // Set up requirements
            var req1 = CompletionRequirement.Create(
                _courseId,
                CompletionCriteria.MinimumGrade,
                80, // Requires 80%, but enrollment has 75%
                "Minimum grade of 80%",
                true,
                0);

            var req2 = CompletionRequirement.Create(
                _courseId,
                CompletionCriteria.LessonsCompleted,
                100,
                "Complete all lessons",
                true,
                1);

            var requirements = new List<CompletionRequirement> { req1, req2 };

            // Act
            bool result = enrollment.CheckAllRequirementsSatisfied(requirements);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryCompleteBasedOnRequirements_ShouldReturnTrueAndComplete_WhenAllRequirementsAreSatisfied()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status);

            // Set up completed lesson
            var lessonId = LessonId.CreateUnique();
            enrollment.MarkLessonComplete(lessonId, 1);

            // Verify status is still Active after marking lesson complete
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status);

            // Set up a final grade
            enrollment.SetFinalGrade(85);

            // Set up requirements
            var req1 = CompletionRequirement.Create(
                _courseId,
                CompletionCriteria.MinimumGrade,
                80,
                "Minimum grade of 80%",
                true,
                0);

            var req2 = CompletionRequirement.Create(
                _courseId,
                CompletionCriteria.AllLessonsCompleted, // Use the new criteria
                100,
                "Complete all lessons",
                true,
                1);

            var requirements = new List<CompletionRequirement> { req1, req2 };

            // Act
            bool result = enrollment.TryCompleteBasedOnRequirements(requirements);

            // Assert
            Assert.True(result);
            Assert.Equal(EnrollmentStatus.Completed, enrollment.Status);
            Assert.NotNull(enrollment.CompletedAt);
        }

        [Fact]
        public void SetFinalExamScore_ShouldUpdateScore()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            int score = 85;

            // Act
            enrollment.SetFinalExamScore(score);

            // Assert
            Assert.Equal(score, enrollment.FinalExamScore);
            Assert.True(enrollment.HasPassedFinalExam);
            Assert.True(enrollment.HasTakenFinalExam);

            // Verify domain event
            var domainEvent = enrollment.DomainEvents.Last();
            var scoreSetEvent = Assert.IsType<EnrollmentFinalExamScoreSetEvent>(domainEvent);
            Assert.Equal(enrollment.Id, scoreSetEvent.EnrollmentId);
            Assert.Equal(enrollment.UserId, scoreSetEvent.UserId);
            Assert.Equal(enrollment.CourseId, scoreSetEvent.CourseId);
            Assert.Equal(score, scoreSetEvent.FinalExamScore);
        }

        [Fact]
        public void SetFinalExamScore_ShouldMarkAsNotPassed_WhenScoreIsBelow70()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            int score = 65;

            // Act
            enrollment.SetFinalExamScore(score);

            // Assert
            Assert.Equal(score, enrollment.FinalExamScore);
            Assert.False(enrollment.HasPassedFinalExam);
            Assert.True(enrollment.HasTakenFinalExam);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void SetFinalExamScore_ShouldThrowException_WhenScoreIsOutOfRange(int score)
        {
            // Arrange
            var enrollment = CreateTestEnrollment();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => enrollment.SetFinalExamScore(score));
        }

        [Fact]
        public void AddDiscussionPost_ShouldIncrementCounter()
        {
            // Arrange
            var enrollment = CreateTestEnrollment();
            int initialCount = enrollment.DiscussionPostCount;

            // Act
            enrollment.AddDiscussionPost();

            // Assert
            Assert.Equal(initialCount + 1, enrollment.DiscussionPostCount);
        }

        // Helper methods
        private Enrollment CreateTestEnrollment()
        {
            return Enrollment.Create(_userId, _courseId);
        }
    }
}

