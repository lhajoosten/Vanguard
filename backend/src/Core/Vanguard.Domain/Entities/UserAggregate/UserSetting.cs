using Ardalis.GuardClauses;
using Vanguard.Core.Base;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class UserSetting : EntityBase
    {
        public int UserId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }

        // Navigation property
        public User User { get; private set; }

        protected UserSetting() { }

        public UserSetting(User user, string key, string value)
        {
            Guard.Against.Null(user, nameof(user));
            Guard.Against.NullOrWhiteSpace(key, nameof(key));
            Guard.Against.Null(value, nameof(value));

            User = user;
            UserId = user.Id;
            Key = key;
            Value = value;
        }

        internal void UpdateValue(string value)
        {
            Guard.Against.Null(value, nameof(value));
            Value = value;
        }
    }
}
