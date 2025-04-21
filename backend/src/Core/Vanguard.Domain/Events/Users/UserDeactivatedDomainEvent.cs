using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    public class UserDeactivatedDomainEvent : DomainEventBase
    {
        public User User { get; }

        public UserDeactivatedDomainEvent(User user)
        {
            User = user;
        }
    }
}