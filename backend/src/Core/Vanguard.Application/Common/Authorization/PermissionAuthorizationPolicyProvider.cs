using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Vanguard.Application.Common.Authorization
{
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check if the policy already exists
            var policy = await base.GetPolicyAsync(policyName);

            if (policy != null)
            {
                return policy;
            }

            // If the policy name starts with "Permission", create a policy requiring that permission
            if (policyName.StartsWith("Permission:"))
            {
                var permissionName = policyName["Permission:".Length..];
                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.AddRequirements(new PermissionRequirement(permissionName));
                return policyBuilder.Build();
            }

            // Return the fallback policy
            return await base.GetPolicyAsync(policyName);
        }
    }
}