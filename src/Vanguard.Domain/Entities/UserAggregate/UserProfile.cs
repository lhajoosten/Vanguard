using Ardalis.GuardClauses;
using Vanguard.Common.Base;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class UserProfile : EntityBase<UserProfileId>
    {
        public UserId UserId { get; private set; } = null!;
        public string Bio { get; private set; } = string.Empty;
        public string AvatarUrl { get; private set; } = string.Empty;
        public string JobTitle { get; private set; } = string.Empty;
        public string Company { get; private set; } = string.Empty;
        public string Website { get; private set; } = string.Empty;
        public string LinkedInUrl { get; private set; } = string.Empty;
        public string GitHubUrl { get; private set; } = string.Empty;
        public string TwitterUrl { get; private set; } = string.Empty;

        // Navigation property for EF Core
        public virtual User? User { get; private set; }

        private UserProfile() { } // For EF Core

        private UserProfile(UserProfileId id, UserId userId) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(userId, nameof(userId), "User ID cannot be null");

            UserId = userId;
        }

        public static UserProfile Create(UserId userId)
        {
            return new UserProfile(UserProfileId.CreateUnique(), userId);
        }

        public void UpdateBio(string bio)
        {
            Bio = bio ?? string.Empty;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateAvatar(string avatarUrl)
        {
            AvatarUrl = avatarUrl ?? string.Empty;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateProfessionalInfo(string jobTitle, string company)
        {
            JobTitle = jobTitle ?? string.Empty;
            Company = company ?? string.Empty;
            ModifiedAt = DateTime.UtcNow;
        }

        public void UpdateSocialLinks(string website, string linkedInUrl, string gitHubUrl, string twitterUrl)
        {
            Website = website ?? string.Empty;
            LinkedInUrl = linkedInUrl ?? string.Empty;
            GitHubUrl = gitHubUrl ?? string.Empty;
            TwitterUrl = twitterUrl ?? string.Empty;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
