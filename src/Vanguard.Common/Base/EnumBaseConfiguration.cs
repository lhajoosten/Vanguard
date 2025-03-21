using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vanguard.Common.Base
{
    // Base configuration for all Enumeration types
    public abstract class EnumerationTypeConfiguration<TEnum> : IEntityTypeConfiguration<TEnum>
        where TEnum : Enumeration
    {
        public virtual void Configure(EntityTypeBuilder<TEnum> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever(); // Enumeration IDs are pre-defined

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
