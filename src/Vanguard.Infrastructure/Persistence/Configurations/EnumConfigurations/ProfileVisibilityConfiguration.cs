using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class ProfileVisibilityConfiguration : EnumerationTypeConfiguration<ProfileVisibility>
    {
        public override void Configure(EntityTypeBuilder<ProfileVisibility> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.IconName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
