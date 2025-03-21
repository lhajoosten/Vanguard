using Ardalis.GuardClauses;
using Vanguard.Common.Base;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class Permission : EntityBase<PermissionId>
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        private Permission() { } // For EF Core

        private Permission(PermissionId id, string name, string description) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Permission name cannot be empty");
            Guard.Against.NullOrWhiteSpace(description, nameof(description), "Permission description cannot be empty");

            Name = name;
            Description = description;
        }

        public static Permission Create(string name, string description)
        {
            return new Permission(PermissionId.CreateUnique(), name, description);
        }
    }
}
