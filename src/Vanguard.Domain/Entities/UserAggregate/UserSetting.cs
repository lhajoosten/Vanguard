using Ardalis.GuardClauses;
using Vanguard.Domain.Base;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class UserSettings : Entity<UserSettingsId>
    {
        public UserId UserId { get; private set; } = null!;

        // Notification settings
        public bool EmailNotificationsEnabled { get; private set; } = true;
        public bool CourseUpdatesEnabled { get; private set; } = true;
        public bool AssignmentRemindersEnabled { get; private set; } = true;
        public bool NewMessageNotificationsEnabled { get; private set; } = true;
        public bool MarketingEmailsEnabled { get; private set; } = false;

        // Display settings
        public UITheme Theme { get; private set; } = UITheme.Light;
        public Language PreferredLanguage { get; private set; } = Language.English;
        public bool CompactViewEnabled { get; private set; } = false;

        // Learning settings
        public bool AutoPlayVideos { get; private set; } = true;
        public bool ShowCompletedCourses { get; private set; } = true;
        public int DefaultVideoPlaybackSpeed { get; private set; } = 100; // Percentage

        // Privacy settings
        public ProfileVisibility ProfileVisibility { get; private set; } = ProfileVisibility.Public;
        public bool ShowEnrollmentHistory { get; private set; } = true;
        public bool ShowAchievements { get; private set; } = true;

        // Navigation property for EF Core
        public virtual User? User { get; private set; }

        private UserSettings() { } // For EF Core

        private UserSettings(UserSettingsId id, UserId userId) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(userId, nameof(userId), "User ID cannot be null");

            UserId = userId;
        }

        public static UserSettings Create(UserId userId)
        {
            return new UserSettings(UserSettingsId.CreateUnique(), userId);
        }

        // Notification settings methods
        public void UpdateNotificationSettings(
            bool emailNotifications,
            bool courseUpdates,
            bool assignmentReminders,
            bool newMessageNotifications,
            bool marketingEmails)
        {
            EmailNotificationsEnabled = emailNotifications;
            CourseUpdatesEnabled = courseUpdates;
            AssignmentRemindersEnabled = assignmentReminders;
            NewMessageNotificationsEnabled = newMessageNotifications;
            MarketingEmailsEnabled = marketingEmails;
            ModifiedAt = DateTime.UtcNow;
        }

        // Display settings methods
        public void UpdateDisplaySettings(UITheme theme, Language language, bool compactView)
        {
            Theme = theme;
            PreferredLanguage = language;
            CompactViewEnabled = compactView;
            ModifiedAt = DateTime.UtcNow;
        }

        // Learning settings methods
        public void UpdateLearningSettings(bool autoPlayVideos, bool showCompletedCourses, int videoPlaybackSpeed)
        {
            Guard.Against.OutOfRange(videoPlaybackSpeed, nameof(videoPlaybackSpeed), 50, 200,
                "Video playback speed must be between 50% and 200%");

            AutoPlayVideos = autoPlayVideos;
            ShowCompletedCourses = showCompletedCourses;
            DefaultVideoPlaybackSpeed = videoPlaybackSpeed;
            ModifiedAt = DateTime.UtcNow;
        }

        // Privacy settings methods
        public void UpdatePrivacySettings(
            ProfileVisibility profileVisibility,
            bool showEnrollmentHistory,
            bool showAchievements)
        {
            ProfileVisibility = profileVisibility;
            ShowEnrollmentHistory = showEnrollmentHistory;
            ShowAchievements = showAchievements;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}