using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Enumerations;

namespace Vanguard.Infrastructure.Persistence.Configurations.EnumConfigurations
{
    public class UIThemeConfiguration : EnumerationTypeConfiguration<UITheme>
    {
        public override void Configure(EntityTypeBuilder<UITheme> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.BackgroundColor)
                .HasMaxLength(20);

            builder.Property(e => e.TextColor)
                .HasMaxLength(20);

            builder.Property(e => e.CssClass)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
