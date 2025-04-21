using Ardalis.GuardClauses;
using Vanguard.Core.Base;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class UserProfile : EntityBase
    {
        public int UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? DisplayName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? ProfilePictureUrl { get; private set; }
        public string? Bio { get; private set; }

        // Navigation property
        public User User { get; private set; }

        protected UserProfile() { }

        public UserProfile(User user, string firstName, string lastName)
        {
            Guard.Against.Null(user, nameof(user));
            Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));

            User = user;
            UserId = user.Id;
            SetNames(firstName, lastName);
        }

        internal void SetNames(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            if (DisplayName == $"{FirstName} {LastName}")
            {
                DisplayName = $"{firstName} {lastName}";
            }
        }
    }
}