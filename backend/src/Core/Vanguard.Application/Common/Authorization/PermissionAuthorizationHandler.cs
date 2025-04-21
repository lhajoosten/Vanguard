using Microsoft.AspNetCore.Authorization;
using Vanguard.Application.Common.Services;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Application.Common.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IAuthorizeService _authorizationService;
        private readonly ICurrentUserService _currentUserService;

        public PermissionAuthorizationHandler(
            IAuthorizeService authorizationService,
            ICurrentUserService currentUserService)
        {
            _authorizationService = authorizationService;
            _currentUserService = currentUserService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Check if the user is authenticated
            // If not, skip the authorization check
            if (context.User == null || !context.User.Identity!.IsAuthenticated)
            {
                return;
            }

            // Get the user ID from the current user service
            var userId = _currentUserService.UserId;
            if (userId <= 0)
            {
                return;
            }

            // Get the permission by name
            var permissionType = typeof(Permission);
            var permissionField = permissionType.GetField(requirement.PermissionName);

            if (permissionField == null)
            {
                throw new System.InvalidOperationException($"Permission '{requirement.PermissionName}' not found");
            }

            var permission = (Permission)permissionField.GetValue(null)!;

            if (await _authorizationService.HasPermissionAsync(userId, permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}