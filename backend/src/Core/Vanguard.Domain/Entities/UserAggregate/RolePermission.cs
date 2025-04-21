using Ardalis.GuardClauses;
using Vanguard.Core.Base;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class RolePermission : EntityBase
    {
        public int RoleId { get; private set; }
        public int PermissionId { get; private set; }

        public Role Role { get; private set; }
        public Permission Permission { get; private set; }

        protected RolePermission() { }

        public RolePermission(Role role, Permission permission)
        {
            Guard.Against.Null(role, nameof(role));
            Guard.Against.Null(permission, nameof(permission));

            RoleId = role.Id;
            PermissionId = permission.Id;
            Role = role;
            Permission = permission;
        }
    }
}
