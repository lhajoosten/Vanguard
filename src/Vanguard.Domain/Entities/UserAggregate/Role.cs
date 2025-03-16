﻿using Ardalis.GuardClauses;
using Vanguard.Domain.Base;

namespace Vanguard.Domain.Entities.UserAggregate
{
    public class Role : Entity<Guid>
    {
        private readonly List<Permission> _permissions = [];

        public string Name { get; private set; } = string.Empty;
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        private Role() { } // For EF Core

        private Role(Guid id, string name) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "Role name cannot be empty");

            Name = name;
        }

        public static Role Create(string name)
        {
            return new Role(Guid.NewGuid(), name);
        }

        public void AddPermission(Permission permission)
        {
            Guard.Against.Null(permission, nameof(permission));

            if (!_permissions.Contains(permission))
            {
                _permissions.Add(permission);
            }
        }

        public void RemovePermission(Permission permission)
        {
            Guard.Against.Null(permission, nameof(permission));

            if (!_permissions.Contains(permission))
            {
                return;
            }
            _permissions.Remove(permission);
        }
    }
}
