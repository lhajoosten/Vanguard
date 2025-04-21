using Microsoft.AspNetCore.Authorization;

namespace Vanguard.Application.Common.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permissionName)
        {
            Policy = $"Permission:{permissionName}";
        }
    }
}