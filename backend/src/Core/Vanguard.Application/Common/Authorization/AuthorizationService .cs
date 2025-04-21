using Ardalis.GuardClauses;
using Microsoft.Extensions.Caching.Memory;
using Vanguard.Application.Common.Repositories;
using Vanguard.Application.Common.Services;
using Vanguard.Core.Interfaces;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Application.Common.Authorization
{
    public class AuthorizationService : IAuthorizeService
    {
        private readonly IQueryRepository<User> _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMemoryCache _cache;

        public AuthorizationService(
            IQueryRepository<User> userRepository,
            IPermissionRepository permissionRepository,
            IMemoryCache cache)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _cache = cache;
        }

        public async Task<bool> HasPermissionAsync(int userId, Permission permission, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(permission, nameof(permission));

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null || !user.IsActive)
                return false;

            return await HasPermissionAsync(user, permission, cancellationToken);
        }

        public async Task<bool> HasPermissionAsync(User user, Permission permission, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(user, nameof(user));
            Guard.Against.Null(permission, nameof(permission));

            if (!user.IsActive)
                return false;

            var permissions = await GetCachedUserPermissionsAsync(user, cancellationToken);
            return permissions.Any(p => p.Id == permission.Id);
        }

        private async Task<IReadOnlyList<Permission>> GetCachedUserPermissionsAsync(User user, CancellationToken cancellationToken)
        {
            Guard.Against.Null(user, nameof(user));

            if (!user.IsActive)
                return new List<Permission>().AsReadOnly();

            var cacheKey = $"user_permissions_{user.Id}";
            if (!_cache.TryGetValue(cacheKey, out IReadOnlyList<Permission> permissions))
            {
                permissions = await _permissionRepository.GetPermissionsByRoleIdAsync(user.Role.Id, cancellationToken);
                _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(10));
            }
            return permissions!;
        }
    }
}
