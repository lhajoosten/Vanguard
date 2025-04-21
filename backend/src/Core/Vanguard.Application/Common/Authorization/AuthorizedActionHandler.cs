using Vanguard.Application.Common.Services;
using Vanguard.Core.Interfaces;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Application.Common.Authorization
{
    public class AuthorizedActionHandler
    {
        private readonly IAuthorizeService _authorizationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IQueryRepository<User> _userRepository;

        public AuthorizedActionHandler(
            IAuthorizeService authorizationService,
            ICurrentUserService currentUserService,
            IQueryRepository<User> userRepository)
        {
            _authorizationService = authorizationService;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<TResult> ExecuteAsync<TResult>(
            Permission requiredPermission,
            Func<Task<TResult>> action,
            CancellationToken cancellationToken = default)
        {
            // Check if the user is authenticated
            var userId = _currentUserService.UserId;
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            // Check if the user is active
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User is not active");
            }

            // Check if the user has the required permission
            if (!await _authorizationService.HasPermissionAsync(userId, requiredPermission, cancellationToken))
            {
                throw new ForbiddenAccessException($"User does not have the required permission: {requiredPermission.Name}");
            }

            return await action();
        }

        public async Task ExecuteAsync(
            Permission requiredPermission,
            Func<Task> action,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            if (!await _authorizationService.HasPermissionAsync(userId, requiredPermission, cancellationToken))
            {
                throw new ForbiddenAccessException($"User does not have the required permission: {requiredPermission.Name}");
            }

            await action();
        }
    }

    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message)
        {
        }
    }
}