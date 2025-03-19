using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.UserAggregate
{
    public class RoleConfiguration : EntityBaseConfiguration<Role, RoleId>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("Roles");

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Unique index on name
            builder.HasIndex(r => r.Name)
                .IsUnique();

            // Many-to-many relationship with Permissions
            builder.HasMany(r => r.Permissions)
                .WithMany()
                .UsingEntity(
                    "RolePermissions",
                    j => j.HasOne(typeof(Permission)).WithMany().HasForeignKey("PermissionId"),
                    j => j.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"),
                    j =>
                    {
                        j.Property<RoleId>("RoleId")
                            .HasConversion(
                                id => id.Value,
                                value => new RoleId(value));
                        j.Property<PermissionId>("PermissionId")
                            .HasConversion(
                                id => id.Value,
                                value => new PermissionId(value));
                        j.HasKey("RoleId", "PermissionId");
                    }
                );
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<Role> builder)
        {
            builder.Property(r => r.Id)
                .HasConversion(
                    id => id.Value,
                    value => new RoleId(value));
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<Role> builder)
        {
            builder.Property(r => r.Id)
                .HasConversion(
                    id => id.Value,
                    value => new RoleId(value));
        }
    }
}