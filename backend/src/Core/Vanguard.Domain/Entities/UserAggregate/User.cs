using Ardalis.GuardClauses;
using Vanguard.Core.Base;
using Vanguard.Domain.Events.Users;
using Vanguard.Domain.Events.Users.Vanguard.Domain.Events.Users;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class User : AggregateRoot
    {
        private List<UserSetting> _settings = new();

        public string Username { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public bool IsActive { get; private set; }
        public int RoleId { get; private set; }
        public int ProfileId { get; private set; }
        public int VIdentityId { get; private set; }

        // Navigation properties
        public Role Role { get; private set; }
        public VIdentity VIdentity { get; private set; }
        public UserProfile Profile { get; private set; }
        public IReadOnlyCollection<UserSetting> Settings => _settings.AsReadOnly();

        protected User() : base() { }

        public User(string username, Role role)
        {
            Guard.Against.NullOrWhiteSpace(username, nameof(username));
            Guard.Against.Null(role, nameof(role));

            Username = username;
            Role = role;
            RoleId = role.Id;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;

            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        public void SetIdentity(VIdentity identity)
        {
            VIdentity = identity;
        }

        public void SetProfile(UserProfile profile)
        {
            Guard.Against.Null(profile, nameof(profile));
            Profile = profile;
            ProfileId = profile.Id;
        }

        public void SetRole(Role role)
        {
            Role = role;
            RoleId = role.Id;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new UserRoleChangedDomainEvent(this, role));
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            if (Profile is null)
            {
                Profile = new UserProfile(this, firstName, lastName);
            }
            else
            {
                Profile.SetNames(firstName, lastName);
            }

            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new ProfileUpdatedDomainEvent(this, Profile));
        }

        public void AddOrUpdateSetting(string key, string value)
        {
            var setting = Settings.FirstOrDefault(s => s.Key == key);
            if (setting is not null)
            {
                setting.UpdateValue(value);
            }
            else
            {
                _settings.Add(new UserSetting(this, key, value));
            }
        }

        public void RemoveSetting(string key)
        {
            var setting = Settings.FirstOrDefault(s => s.Key == key);
            if (setting is not null)
            {
                _settings.Remove(setting);
            }
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                LastModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new UserActivatedDomainEvent(this));
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                LastModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new UserDeactivatedDomainEvent(this));
            }
        }

        public void ConfirmEmail()
        {
            if (VIdentity is not null && !VIdentity.IsEmailConfirmed)
            {
                VIdentity.ConfirmEmail();
                LastModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new EmailConfirmedDomainEvent(this, VIdentity));
            }
        }

        public void ConfirmPhoneNumber()
        {
            if (VIdentity is not null && !VIdentity.IsPhoneNumberConfirmed)
            {
                VIdentity.ConfirmPhoneNumber();
                LastModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new PhoneNumberConfirmedDomainEvent(this, VIdentity));
            }
        }

        public void RecordSuccessfulLogin()
        {
            if (VIdentity is not null)
            {
                VIdentity.RecordSuccessfulLogin();
                LastModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new LoginSucceededDomainEvent(this, VIdentity));
            }
        }

        public bool RecordFailedLogin()
        {
            if (VIdentity is not null)
            {
                var accountLocked = VIdentity.RecordFailedLogin();
                LastModifiedAt = DateTime.UtcNow;
                AddDomainEvent(new LoginFailedDomainEvent(this, VIdentity, VIdentity.FailedLoginAttempts, accountLocked));
                return accountLocked;
            }

            return false;
        }
    }
}
