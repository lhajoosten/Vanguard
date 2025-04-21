using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    public class UserActivatedDomainEvent : DomainEventBase
    {
        public User User { get; }

        public UserActivatedDomainEvent(User user)
        {
            User = user;
        }
    }
}