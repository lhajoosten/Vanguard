using Vanguard.Domain.Base;

namespace Vanguard.Domain.Enumerations
{
    public class ProfileVisibility : Enumeration
    {
        public readonly static ProfileVisibility Public = new(1, "Public", "Visible to everyone", "globe");
        public readonly static ProfileVisibility Private = new(2, "Private", "Visible only to you", "lock");
        public readonly static ProfileVisibility FriendsOnly = new(3, "Friends Only", "Visible only to your friends", "users");
        public readonly static ProfileVisibility Enrolled = new(4, "Course Enrolled", "Visible only to those enrolled in the same courses", "graduation-cap");
        public readonly static ProfileVisibility Verified = new(5, "Verified Only", "Visible only to verified members", "check-circle");

        public string Description { get; private set; }
        public string IconName { get; private set; }

        private ProfileVisibility(int id, string name, string description, string iconName) : base(id, name)
        {
            Description = description;
            IconName = iconName;
        }

        // Helper methods for visibility checks
        public bool IsVisibleToEveryone()
        {
            return this == Public;
        }

        public bool IsVisibleToFriends()
        {
            return this == Public || this == FriendsOnly;
        }

        public bool IsVisibleToEnrolled()
        {
            return this == Public || this == FriendsOnly || this == Enrolled;
        }

        public bool IsVisibleToVerified()
        {
            return this == Public || this == FriendsOnly || this == Enrolled || this == Verified;
        }

        public string GetIconClass()
        {
            return $"fa fa-{IconName}";
        }

        public string GetPrivacyDescription()
        {
            return $"Your profile is {Description.ToLower()}";
        }

        public string GetVisibilityBadge()
        {
            return $"<span class=\"badge bg-{GetBadgeColor()}\"><i class=\"fa fa-{IconName}\"></i> {Name}</span>";
        }

        private string GetBadgeColor()
        {
            return this switch
            {
                var v when v == Public => "success",
                var v when v == Private => "danger",
                var v when v == FriendsOnly => "primary",
                var v when v == Enrolled => "info",
                var v when v == Verified => "warning",
                _ => "secondary"
            };
        }
    }
}
