using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    public class LoginFailedDomainEvent : DomainEventBase
    {
        public User User { get; }
        public VIdentity Identity { get; }
        public int FailedAttempts { get; }
        public bool AccountLocked { get; }

        public LoginFailedDomainEvent(User user, VIdentity identity, int failedAttempts, bool accountLocked)
        {
            User = user;
            Identity = identity;
            FailedAttempts = failedAttempts;
            AccountLocked = accountLocked;
        }
    }
}