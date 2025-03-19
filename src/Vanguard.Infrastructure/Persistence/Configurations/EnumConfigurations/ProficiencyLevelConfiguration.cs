using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class ProficiencyLevelConfiguration : EnumerationTypeConfiguration<ProficiencyLevel>
    {
        public override void Configure(EntityTypeBuilder<ProficiencyLevel> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
