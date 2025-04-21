using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    public class LoginSucceededDomainEvent : DomainEventBase
    {
        public User User { get; }
        public VIdentity Identity { get; }

        public LoginSucceededDomainEvent(User user, VIdentity identity)
        {
            User = user;
            Identity = identity;
        }
    }
}