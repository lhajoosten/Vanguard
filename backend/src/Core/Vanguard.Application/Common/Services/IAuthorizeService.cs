using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Application.Common.Services
{
    public interface IAuthorizeService
    {
        Task<bool> HasPermissionAsync(int userId, Permission permission, CancellationToken cancellationToken = default);
        Task<bool> HasPermissionAsync(User user, Permission permission, CancellationToken cancellationToken = default);
    }
}
