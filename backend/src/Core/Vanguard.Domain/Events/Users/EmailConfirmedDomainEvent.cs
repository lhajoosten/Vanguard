using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    public class EmailConfirmedDomainEvent : DomainEventBase
    {
        public User User { get; }
        public VIdentity Identity { get; }

        public EmailConfirmedDomainEvent(User user, VIdentity identity)
        {
            User = user;
            Identity = identity;
        }
    }
}