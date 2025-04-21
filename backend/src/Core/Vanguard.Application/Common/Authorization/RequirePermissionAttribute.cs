using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Vanguard.Application.Common.Services;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Application.Common.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _permissionName;

        public RequirePermissionAttribute(string permissionName)
        {
            _permissionName = permissionName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizeService>();
            var currentUserService = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

            var userId = currentUserService.UserId;
            if (userId <= 0)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Get the permission by name
            var permissionType = typeof(Permission);
            var permissionField = permissionType.GetField(_permissionName) ?? throw new InvalidOperationException($"Permission '{_permissionName}' not found");
            var permission = (Permission)permissionField.GetValue(null)!;

            if (!await authorizationService.HasPermissionAsync(userId, permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
