using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    public class UserRoleChangedDomainEvent : DomainEventBase
    {
        public User User { get; }
        public Role NewRole { get; }

        public UserRoleChangedDomainEvent(User user, Role newRole)
        {
            User = user;
            NewRole = newRole;
        }
    }
}