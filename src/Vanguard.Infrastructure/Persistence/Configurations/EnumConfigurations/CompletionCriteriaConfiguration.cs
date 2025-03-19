using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class CompletionCriteriaConfiguration : EnumerationTypeConfiguration<CompletionCriteria>
    {
        public override void Configure(EntityTypeBuilder<CompletionCriteria> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.SystemName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
