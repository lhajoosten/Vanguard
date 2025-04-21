using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    namespace Vanguard.Domain.Events.Users
    {
        public class UserCreatedDomainEvent : DomainEventBase
        {
            public User User { get; }

            public UserCreatedDomainEvent(User user)
            {
                User = user;
            }
        }
    }
}
