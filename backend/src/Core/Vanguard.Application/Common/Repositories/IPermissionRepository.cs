using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Application.Common.Repositories
{
    public interface IPermissionRepository
    {
        Task<IReadOnlyList<Permission>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
        Task<bool> UserHasPermissionAsync(int userId, int permissionId, CancellationToken cancellationToken = default);
        Task EnsureRoleHasDefaultPermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    }
}
