using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class LanguageConfiguration : EnumerationTypeConfiguration<Language>
    {
        public override void Configure(EntityTypeBuilder<Language> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.CultureCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.NativeName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.IsRightToLeft)
                .IsRequired();

            // Ignore the computed property
            builder.Ignore(e => e.Culture);
        }
    }
}
