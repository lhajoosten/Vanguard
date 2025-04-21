using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Application.Common.Authorization
{
    public interface IRequireAuthorization
    {
        IEnumerable<Permission> RequiredPermissions { get; }
    }
}