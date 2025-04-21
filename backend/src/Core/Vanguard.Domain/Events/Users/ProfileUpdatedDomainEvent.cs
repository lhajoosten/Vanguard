using Vanguard.Core.Events;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Domain.Events.Users
{
    public class ProfileUpdatedDomainEvent : DomainEventBase
    {
        public User User { get; }
        public UserProfile Profile { get; }

        public ProfileUpdatedDomainEvent(User user, UserProfile profile)
        {
            User = user;
            Profile = profile;
        }
    }
}