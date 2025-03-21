using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class QuestionTypeConfiguration : EnumerationTypeConfiguration<QuestionType>
    {
        public override void Configure(EntityTypeBuilder<QuestionType> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.TechnicalName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
