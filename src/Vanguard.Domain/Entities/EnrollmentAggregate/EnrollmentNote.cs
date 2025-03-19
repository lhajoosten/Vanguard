using Ardalis.GuardClauses;
using Vanguard.Common.Base;

namespace Vanguard.Domain.Entities.EnrollmentAggregate
{
    public class EnrollmentNote : EntityBase<Guid>
    {
        public string Content { get; private set; } = string.Empty;

        private EnrollmentNote() { } // For EF Core

        private EnrollmentNote(Guid id, string content) : base(id)
        {
            Guard.Against.NullOrWhiteSpace(content, nameof(content), "Note content cannot be empty");

            Content = content;
        }

        public static EnrollmentNote Create(string content)
        {
            return new EnrollmentNote(Guid.NewGuid(), content);
        }

        public void Update(string content)
        {
            Guard.Against.NullOrWhiteSpace(content, nameof(content), "Note content cannot be empty");

            Content = content;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
