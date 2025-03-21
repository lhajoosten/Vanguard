using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.UserAggregate
{
    public class PermissionConfiguration : EntityBaseConfiguration<Permission, PermissionId>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("Permissions");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            // Unique index on name
            builder.HasIndex(p => p.Name)
                .IsUnique();
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(p => p.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<Permission> builder)
        {
            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new PermissionId(value));
        }
    }
}