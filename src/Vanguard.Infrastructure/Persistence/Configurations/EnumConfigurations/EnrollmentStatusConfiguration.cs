using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class EnrollmentStatusConfiguration : EnumerationTypeConfiguration<EnrollmentStatus>
    {
        public override void Configure(EntityTypeBuilder<EnrollmentStatus> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.ColorCode)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
