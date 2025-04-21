using Ardalis.GuardClauses;
using Vanguard.Core.Base;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class VIdentity : EntityBase
    {
        public int UserId { get; private set; }

        // Identity properties
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public bool IsPhoneNumberConfirmed { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        // Security-related properties
        public string SecurityStamp { get; private set; }
        public DateTime? LastLogin { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockoutEnd { get; private set; }
        public int AccessFailedCount { get; private set; }
        public bool TwoFactorEnabled { get; private set; }

        // Navigation property
        public User User { get; private set; }

        protected VIdentity() { }

        public VIdentity(User user, string email, string passwordHash)
        {
            Guard.Against.Null(user, nameof(user));
            Guard.Against.NullOrWhiteSpace(email, nameof(email));
            Guard.Against.NullOrWhiteSpace(passwordHash, nameof(passwordHash));

            User = user;
            UserId = user.Id;
            Email = new Email(email);
            PasswordHash = passwordHash;
            IsEmailConfirmed = false;
            IsPhoneNumberConfirmed = false;
            SecurityStamp = Guid.NewGuid().ToString();
            LastLogin = null;
            FailedLoginAttempts = 0;
            LockoutEnd = null;
            AccessFailedCount = 0;
            TwoFactorEnabled = false;
        }

        public void SetRefreshToken(string token, DateTime expiryTime)
        {
            Guard.Against.NullOrWhiteSpace(token, nameof(token));
            Guard.Against.OutOfRange(expiryTime, nameof(expiryTime), DateTime.UtcNow, DateTime.MaxValue, "Expiry time must be in the future.");

            RefreshToken = token;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

        internal void ConfirmEmail() => IsEmailConfirmed = true;
        internal void ConfirmPhoneNumber() => IsPhoneNumberConfirmed = true;

        internal void RecordSuccessfulLogin()
        {
            LastLogin = DateTime.UtcNow;
            FailedLoginAttempts = 0;
            LockoutEnd = null;
        }

        internal bool RecordFailedLogin()
        {
            FailedLoginAttempts++;

            bool accountLocked = false;
            if (FailedLoginAttempts >= 5)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                accountLocked = true;
            }

            return accountLocked;
        }
    }
}