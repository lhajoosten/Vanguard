using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.CourseAggregate
{
    public class CourseModuleConfiguration : EntityBaseConfiguration<CourseModule, ModuleId>
    {
        public void Configure(EntityTypeBuilder<CourseModule> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("CourseModules");

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .HasMaxLength(2000);

            builder.Property(m => m.OrderIndex)
                .IsRequired();

            // Course relationship with explicit conversion
            builder.Property(m => m.CourseId)
                .HasConversion(
                    id => id.Value,
                    value => new CourseId(value));

            builder.HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many for Lessons
            builder.HasMany(m => m.Lessons)
                .WithOne(l => l.Module)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<CourseModule> builder)
        {
            builder.HasKey(m => m.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<CourseModule> builder)
        {
            builder.Property(m => m.Id)
                .HasConversion(
                    id => id.Value,
                    value => new ModuleId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<CourseModule> builder)
        {
            builder.Property(m => m.CourseId)
                .HasConversion(
                    id => id.Value,
                    value => new CourseId(value));
        }
    }
}