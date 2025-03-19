using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.CourseAggregate
{
    public class CourseTagConfiguration : EntityBaseConfiguration<CourseTag, CourseTagId>
    {
        public override void Configure(EntityTypeBuilder<CourseTag> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("CourseTags");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.Slug)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Unique indexes
            builder.HasIndex(t => t.Slug)
                .IsUnique();

            builder.HasIndex(t => t.Name)
                .IsUnique();
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<CourseTag> builder)
        {
            builder.HasKey(t => t.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<CourseTag> builder)
        {
            builder.Property(t => t.Id)
                .HasConversion(
                    id => id.Value,
                    value => new CourseTagId(value)
                );
        }
    }
}