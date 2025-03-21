using Vanguard.Common.Base;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.UnitTests.Aggregates
{
    public class SkillAssessmentTests
    {
        private readonly UserId _userId = UserId.CreateUnique();
        private readonly SkillId _skillId = SkillId.CreateUnique();

        [Fact]
        public void Create_ShouldCreateNewAssessment_WithCorrectProperties()
        {
            // Arrange
            var level = ProficiencyLevel.Intermediate;
            string evidence = "2 years experience";

            // Act
            var assessment = SkillAssessment.Create(_userId, _skillId, level, evidence);

            // Assert
            Assert.NotNull(assessment);
            Assert.Equal(_userId, assessment.UserId);
            Assert.Equal(_skillId, assessment.SkillId);
            Assert.Equal(level, assessment.Level);
            Assert.Equal(evidence, assessment.Evidence);
            Assert.False(assessment.IsVerified);
            Assert.Null(assessment.VerifiedById);
            Assert.Null(assessment.VerifiedAt);
            Assert.NotNull(assessment.AssessedAt);
            Assert.Empty(assessment.Questions);
            Assert.Empty(assessment.Answers);
            Assert.Empty(assessment.Results);

            // Verify domain event
            var domainEvent = Assert.Single(assessment.DomainEvents);
            var skillAssessedEvent = Assert.IsType<SkillAssessedEvent>(domainEvent);
            Assert.Equal(assessment.Id, skillAssessedEvent.AssessmentId);
            Assert.Equal(_userId, skillAssessedEvent.UserId);
            Assert.Equal(_skillId, skillAssessedEvent.SkillId);
            Assert.Equal(level, skillAssessedEvent.Level);
        }

        [Fact]
        public void Create_ShouldSucceed_WithoutEvidence()
        {
            // Act
            var assessment = SkillAssessment.Create(_userId, _skillId, ProficiencyLevel.Beginner);

            // Assert
            Assert.Equal(string.Empty, assessment.Evidence);
        }

        [Fact]
        public void Create_ShouldThrowException_WhenUserIdIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                SkillAssessment.Create(null, _skillId, ProficiencyLevel.Beginner));
        }

        [Fact]
        public void Create_ShouldThrowException_WhenSkillIdIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                SkillAssessment.Create(_userId, null, ProficiencyLevel.Beginner));
        }

        [Fact]
        public void UpdateAssessment_ShouldUpdateProperties()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var newLevel = ProficiencyLevel.Expert;
            string newEvidence = "Updated evidence";

            // Act
            assessment.UpdateAssessment(newLevel, newEvidence);

            // Assert
            Assert.Equal(newLevel, assessment.Level);
            Assert.Equal(newEvidence, assessment.Evidence);
            Assert.NotEqual(assessment.CreatedAt, assessment.AssessedAt);

            // Verify domain event
            var domainEvent = assessment.DomainEvents.Last();
            var assessmentUpdatedEvent = Assert.IsType<SkillAssessmentUpdatedEvent>(domainEvent);
            Assert.Equal(assessment.Id, assessmentUpdatedEvent.AssessmentId);
            Assert.Equal(_userId, assessmentUpdatedEvent.UserId);
            Assert.Equal(_skillId, assessmentUpdatedEvent.SkillId);
            Assert.Equal(newLevel, assessmentUpdatedEvent.Level);
        }

        [Fact]
        public void UpdateAssessment_ShouldResetVerificationStatus_WhenPreviouslyVerified()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var verifierId = UserId.CreateUnique();
            assessment.Verify(verifierId);
            Assert.True(assessment.IsVerified);

            // Act
            assessment.UpdateAssessment(ProficiencyLevel.Expert, "New evidence");

            // Assert
            Assert.False(assessment.IsVerified);
            Assert.Null(assessment.VerifiedAt);
            Assert.Null(assessment.VerifiedById);
        }

        [Fact]
        public void Verify_ShouldSetVerificationProperties()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var verifierId = UserId.CreateUnique();

            // Act
            assessment.Verify(verifierId);

            // Assert
            Assert.True(assessment.IsVerified);
            Assert.Equal(verifierId, assessment.VerifiedById);
            Assert.NotNull(assessment.VerifiedAt);

            // Verify domain event
            var domainEvent = assessment.DomainEvents.Last();
            var verifiedEvent = Assert.IsType<SkillAssessmentVerifiedEvent>(domainEvent);
            Assert.Equal(assessment.Id, verifiedEvent.AssessmentId);
            Assert.Equal(_userId, verifiedEvent.UserId);
            Assert.Equal(_skillId, verifiedEvent.SkillId);
            Assert.Equal(verifierId, verifiedEvent.VerifiedById);
        }

        [Fact]
        public void Verify_ShouldThrowException_WhenVerifierIsNull()
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => assessment.Verify(null));
        }

        [Fact]
        public void Verify_ShouldThrowException_WhenVerifyingOwnAssessment()
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                assessment.Verify(_userId));

            Assert.Contains("Users cannot verify their own skill assessments", exception.Message);
        }

        [Fact]
        public void RemoveVerification_ShouldResetVerificationStatus()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var verifierId = UserId.CreateUnique();
            assessment.Verify(verifierId);

            // Act
            assessment.RemoveVerification();

            // Assert
            Assert.False(assessment.IsVerified);
            Assert.Null(assessment.VerifiedAt);
            Assert.Equal(verifierId, assessment.VerifiedById); // Should keep verifier ID for audit

            // Verify domain event
            var domainEvent = assessment.DomainEvents.Last();
            var verificationRemovedEvent = Assert.IsType<SkillAssessmentVerificationRemovedEvent>(domainEvent);
            Assert.Equal(assessment.Id, verificationRemovedEvent.AssessmentId);
            Assert.Equal(_userId, verificationRemovedEvent.UserId);
            Assert.Equal(_skillId, verificationRemovedEvent.SkillId);
        }

        [Fact]
        public void RemoveVerification_ShouldDoNothing_WhenNotVerified()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            int initialEventCount = assessment.DomainEvents.Count;

            // Act
            assessment.RemoveVerification();

            // Assert
            Assert.False(assessment.IsVerified);
            Assert.Equal(initialEventCount, assessment.DomainEvents.Count);
        }

        [Fact]
        public void AddQuestion_ShouldAddNewQuestion_WithCorrectProperties()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            string text = "Test question";
            string explanation = "Explanation";
            var type = QuestionType.MultipleChoice;
            var difficulty = DifficultyLevel.Medium;
            int pointValue = 10;
            int orderIndex = 0;

            // Act
            var question = assessment.AddQuestion(
                text,
                explanation,
                type,
                difficulty,
                pointValue,
                orderIndex);

            // Assert
            Assert.NotNull(question);
            Assert.Equal(text, question.Text);
            Assert.Equal(explanation, question.Explanation);
            Assert.Equal(type, question.Type);
            Assert.Equal(difficulty, question.Difficulty);
            Assert.Equal(pointValue, question.PointValue);
            Assert.Equal(orderIndex, question.OrderIndex);
            Assert.Single(assessment.Questions);

            // Verify domain event
            var domainEvent = assessment.DomainEvents.Last();
            var questionAddedEvent = Assert.IsType<SkillAssessmentQuestionAddedEvent>(domainEvent);
            Assert.Equal(question.Id, questionAddedEvent.QuestionId);
            Assert.Equal(_skillId, questionAddedEvent.SkillId);
        }

        [Fact]
        public void AddQuestion_ShouldShiftExistingQuestions_WhenOrderIndexIsOccupied()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question1 = assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);
            var question2 = assessment.AddQuestion("Q2", "E2", QuestionType.MultipleChoice, DifficultyLevel.Medium, 10, 1);

            // Act - Add question at index 0 (should shift others)
            var newQuestion = assessment.AddQuestion(
                "New Question",
                "New Explanation",
                QuestionType.MultipleChoice,
                DifficultyLevel.Hard,
                15,
                0);

            // Assert
            Assert.Equal(3, assessment.Questions.Count);
            Assert.Equal(0, newQuestion.OrderIndex);
            Assert.Equal(1, question1.OrderIndex);
            Assert.Equal(2, question2.OrderIndex);
        }

        [Theory]
        [InlineData("", "Explanation")]
        [InlineData("  ", "Explanation")]
        [InlineData(null, "Explanation")]
        public void AddQuestion_ShouldThrowExceptionForInvalidText(string text, string explanation)
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                assessment.AddQuestion(
                    text,
                    explanation,
                    QuestionType.MultipleChoice,
                    DifficultyLevel.Medium,
                    10,
                    0));

            if (text == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("text", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Question text cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddQuestion_ShouldThrowException_WhenPointValueIsNotPositive(int pointValue)
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                assessment.AddQuestion(
                    "Question",
                    "Explanation",
                    QuestionType.MultipleChoice,
                    DifficultyLevel.Medium,
                    pointValue,
                    0));

            Assert.Contains("Point value must be positive", exception.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        public void AddQuestion_ShouldThrowException_WhenOrderIndexIsNegative(int orderIndex)
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                assessment.AddQuestion(
                    "Question",
                    "Explanation",
                    QuestionType.MultipleChoice,
                    DifficultyLevel.Medium,
                    10,
                    orderIndex));

            Assert.Contains("Order index must be non-negative", exception.Message);
        }

        [Fact]
        public void RemoveQuestion_ShouldRemoveQuestion_WhenQuestionExists()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question = assessment.AddQuestion(
                "Question",
                "Explanation",
                QuestionType.MultipleChoice,
                DifficultyLevel.Medium,
                10,
                0);

            // Act
            assessment.RemoveQuestion(question.Id);

            // Assert
            Assert.Empty(assessment.Questions);
        }

        [Fact]
        public void RemoveQuestion_ShouldReindexRemainingQuestions()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var q1 = assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);
            var q2 = assessment.AddQuestion("Q2", "E2", QuestionType.MultipleChoice, DifficultyLevel.Medium, 10, 1);
            var q3 = assessment.AddQuestion("Q3", "E3", QuestionType.MultipleChoice, DifficultyLevel.Hard, 15, 2);

            // Act - Remove middle question
            assessment.RemoveQuestion(q2.Id);

            // Assert
            Assert.Equal(2, assessment.Questions.Count);
            Assert.Equal(0, q1.OrderIndex);
            Assert.Equal(1, q3.OrderIndex);
        }

        [Fact]
        public void RemoveQuestion_ShouldThrowException_WhenQuestionDoesNotExist()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var nonExistentQuestionId = SkillAssessmentQuestionId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                assessment.RemoveQuestion(nonExistentQuestionId));
        }

        [Fact]
        public void ReorderQuestions_ShouldUpdateQuestionOrderIndices()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var q1 = assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);
            var q2 = assessment.AddQuestion("Q2", "E2", QuestionType.MultipleChoice, DifficultyLevel.Medium, 10, 1);
            var q3 = assessment.AddQuestion("Q3", "E3", QuestionType.MultipleChoice, DifficultyLevel.Hard, 15, 2);

            // New order: q3, q1, q2
            var newOrder = new List<SkillAssessmentQuestionId> { q3.Id, q1.Id, q2.Id };

            // Act
            assessment.ReorderQuestions(newOrder);

            // Assert
            Assert.Equal(0, q3.OrderIndex);
            Assert.Equal(1, q1.OrderIndex);
            Assert.Equal(2, q2.OrderIndex);
        }

        [Fact]
        public void ReorderQuestions_ShouldThrowException_WhenQuestionIdsListIsEmpty()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                assessment.ReorderQuestions(new List<SkillAssessmentQuestionId>()));
        }

        [Fact]
        public void ReorderQuestions_ShouldThrowException_WhenQuestionDoesNotExist()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question = assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);
            var nonExistentQuestionId = SkillAssessmentQuestionId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                assessment.ReorderQuestions(new List<SkillAssessmentQuestionId>
                {
                    question.Id,
                    nonExistentQuestionId
                }));
        }

        [Fact]
        public void UpdateQuestion_ShouldUpdateQuestionProperties()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question = assessment.AddQuestion(
                "Original Question",
                "Original Explanation",
                QuestionType.MultipleChoice,
                DifficultyLevel.Easy,
                5,
                0);

            string newText = "Updated Question";
            string newExplanation = "Updated Explanation";
            var newType = QuestionType.OpenText;
            var newDifficulty = DifficultyLevel.Hard;
            int newPointValue = 20;

            // Act
            assessment.UpdateQuestion(
                question.Id,
                newText,
                newExplanation,
                newType,
                newDifficulty,
                newPointValue);

            // Assert
            Assert.Equal(newText, question.Text);
            Assert.Equal(newExplanation, question.Explanation);
            Assert.Equal(newType, question.Type);
            Assert.Equal(newDifficulty, question.Difficulty);
            Assert.Equal(newPointValue, question.PointValue);
            Assert.Equal(0, question.OrderIndex); // OrderIndex should remain unchanged
        }

        [Theory]
        [InlineData("", "Explanation")]
        [InlineData("  ", "Explanation")]
        [InlineData(null, "Explanation")]
        public void UpdateQuestion_ShouldThrowExceptionForInvalidText(string text, string explanation)
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question = assessment.AddQuestion(
                "Original Question",
                "Original Explanation",
                QuestionType.MultipleChoice,
                DifficultyLevel.Easy,
                5,
                0);

            // Act & Assert
            var exception = Assert.ThrowsAny<ArgumentException>(() =>
                assessment.UpdateQuestion(
                    question.Id,
                    text,
                    explanation,
                    QuestionType.MultipleChoice,
                    DifficultyLevel.Medium,
                    10));

            if (text == null)
            {
                var nullException = Assert.IsType<ArgumentNullException>(exception);
                Assert.Equal("text", nullException.ParamName);
            }
            else
            {
                Assert.Contains("Question text cannot be empty", exception.Message);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateQuestion_ShouldThrowException_WhenPointValueIsNotPositive(int pointValue)
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question = assessment.AddQuestion(
                "Original Question",
                "Original Explanation",
                QuestionType.MultipleChoice,
                DifficultyLevel.Easy,
                5,
                0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                assessment.UpdateQuestion(
                    question.Id,
                    "Updated Question",
                    "Updated Explanation",
                    QuestionType.MultipleChoice,
                    DifficultyLevel.Medium,
                    pointValue));
        }

        [Fact]
        public void UpdateQuestion_ShouldThrowException_WhenQuestionDoesNotExist()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var nonExistentQuestionId = SkillAssessmentQuestionId.CreateUnique();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                assessment.UpdateQuestion(
                    nonExistentQuestionId,
                    "Question",
                    "Explanation",
                    QuestionType.MultipleChoice,
                    DifficultyLevel.Medium,
                    10));
        }

        [Fact]
        public void RecordAnswer_ShouldAddAnswerToAssessment()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question = assessment.AddQuestion(
                "Test Question",
                "Explanation",
                QuestionType.MultipleChoice,
                DifficultyLevel.Medium,
                10,
                0);

            var answer = SkillAssessmentAnswer.Create(_userId, question.Id, assessment.Id);

            // Act
            assessment.RecordAnswer(answer);

            // Assert
            Assert.Single(assessment.Answers);
            Assert.Contains(answer, assessment.Answers);
        }

        [Fact]
        public void RecordAnswer_ShouldThrowException_WhenAnswerIsForDifferentAssessment()
        {
            // Arrange
            var assessment1 = CreateTestAssessment();
            var assessment2 = SkillAssessment.Create(_userId, _skillId, ProficiencyLevel.Intermediate);

            var question = assessment1.AddQuestion(
                "Test Question",
                "Explanation",
                QuestionType.MultipleChoice,
                DifficultyLevel.Medium,
                10,
                0);

            var answer = SkillAssessmentAnswer.Create(_userId, question.Id, assessment2.Id);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => assessment1.RecordAnswer(answer));
        }

        [Fact]
        public void RecordAnswer_ShouldThrowException_WhenAnswerIsForNonExistentQuestion()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var nonExistentQuestionId = SkillAssessmentQuestionId.CreateUnique();

            var answer = SkillAssessmentAnswer.Create(_userId, nonExistentQuestionId, assessment.Id);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => assessment.RecordAnswer(answer));
        }

        [Fact]
        public void RecordResult_ShouldAddResultToAssessment()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question1 = assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);
            var question2 = assessment.AddQuestion("Q2", "E2", QuestionType.MultipleChoice, DifficultyLevel.Medium, 10, 1);

            var answer1 = SkillAssessmentAnswer.Create(_userId, question1.Id, assessment.Id);
            var answer2 = SkillAssessmentAnswer.Create(_userId, question2.Id, assessment.Id);

            assessment.RecordAnswer(answer1);
            assessment.RecordAnswer(answer2);

            // Act
            var result = assessment.RecordResult(_userId, new[] { answer1, answer2 });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_userId, result.UserId);
            Assert.Equal(_skillId, result.SkillId);
            Assert.Equal(assessment.Id, result.AssessmentId);
            Assert.Single(assessment.Results);
            Assert.Contains(result, assessment.Results);
        }

        [Fact]
        public void RecordResult_ShouldThrowException_WhenUserIdIsNull()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            var question = assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);
            var answer = SkillAssessmentAnswer.Create(_userId, question.Id, assessment.Id);
            assessment.RecordAnswer(answer);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                assessment.RecordResult(null, new[] { answer }));
        }

        [Fact]
        public void RecordResult_ShouldThrowException_WhenAnswersIsEmptyOrNull()
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                assessment.RecordResult(_userId, Array.Empty<SkillAssessmentAnswer>()));
        }

        [Fact]
        public void HasQuestions_ShouldReturnTrue_WhenQuestionsExist()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);

            // Act & Assert
            Assert.True(assessment.HasQuestions());
        }

        [Fact]
        public void HasQuestions_ShouldReturnFalse_WhenNoQuestionsExist()
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            Assert.False(assessment.HasQuestions());
        }

        [Fact]
        public void GetMaximumPossibleScore_ShouldSumPointValues()
        {
            // Arrange
            var assessment = CreateTestAssessment();
            assessment.AddQuestion("Q1", "E1", QuestionType.MultipleChoice, DifficultyLevel.Easy, 5, 0);
            assessment.AddQuestion("Q2", "E2", QuestionType.MultipleChoice, DifficultyLevel.Medium, 10, 1);
            assessment.AddQuestion("Q3", "E3", QuestionType.MultipleChoice, DifficultyLevel.Hard, 15, 2);

            // Expected: 5 + 10 + 15 = 30

            // Act & Assert
            Assert.Equal(30, assessment.GetMaximumPossibleScore());
        }

        [Fact]
        public void GetMaximumPossibleScore_ShouldReturnZero_WhenNoQuestionsExist()
        {
            // Arrange
            var assessment = CreateTestAssessment();

            // Act & Assert
            Assert.Equal(0, assessment.GetMaximumPossibleScore());
        }

        // Helper methods
        private SkillAssessment CreateTestAssessment()
        {
            return SkillAssessment.Create(_userId, _skillId, ProficiencyLevel.Intermediate, "Test evidence");
        }
    }
}