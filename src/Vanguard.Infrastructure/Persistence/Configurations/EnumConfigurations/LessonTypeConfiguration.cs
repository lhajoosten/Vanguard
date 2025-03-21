using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class LessonTypeConfiguration : EnumerationTypeConfiguration<LessonType>
    {
        public override void Configure(EntityTypeBuilder<LessonType> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.IconName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.UrlSlug)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
