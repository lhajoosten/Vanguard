using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.CourseAggregate
{
    public class CourseReview : Entity<Guid>
    {
        public UserId UserId { get; private set; } = null!;
        public int Rating { get; private set; }
        public string Comment { get; private set; } = string.Empty;

        private CourseReview() { } // For EF Core

        private CourseReview(Guid id, UserId userId, int rating, string comment) : base(id)
        {
            Guard.Against.Null(userId, nameof(userId), "User ID cannot be null");
            Guard.Against.OutOfRange(rating, nameof(rating), 1, 5, "Rating must be between 1 and 5");

            UserId = userId;
            Rating = rating;
            Comment = comment;
        }

        public static CourseReview Create(UserId userId, int rating, string comment)
        {
            return new CourseReview(Guid.NewGuid(), userId, rating, comment);
        }

        public void Update(int rating, string comment)
        {
            Guard.Against.OutOfRange(rating, nameof(rating), 1, 5, "Rating must be between 1 and 5");

            Rating = rating;
            Comment = comment;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
