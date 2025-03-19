using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.CourseAggregate
{
    public class LessonConfiguration : EntityBaseConfiguration<Lesson, LessonId>
    {
        public override void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("Lessons");

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Description)
                .HasMaxLength(2000);

            builder.Property(l => l.Content)
                .HasColumnType("nvarchar(max)");

            builder.Property(l => l.OrderIndex)
                .IsRequired();

            builder.Property(l => l.DurationMinutes)
                .IsRequired()
                .HasDefaultValue(0);

            // Type enumeration reference
            builder.Property<int>("TypeId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(l => l.Type)
                .WithMany()
                .HasForeignKey("TypeId")
                .OnDelete(DeleteBehavior.Restrict);

            // Module relationship
            builder.HasOne(l => l.Module)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Owned entity collection for resources
            builder.OwnsMany(l => l.Resources, resourceBuilder =>
            {
                resourceBuilder.ToTable("LessonResources");
                resourceBuilder.WithOwner().HasForeignKey("LessonId");
                resourceBuilder.HasKey("Id", "LessonId");

                resourceBuilder.Property(r => r.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                resourceBuilder.Property(r => r.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                resourceBuilder.Property(r => r.Description)
                    .HasMaxLength(1000);
            });
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(l => l.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<Lesson> builder)
        {
            builder.Property(l => l.Id)
                .HasConversion(
                    id => id.Value,
                    value => new LessonId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<Lesson> builder)
        {
            // Explicitly configure ModuleId to handle the custom ID type
            builder.Property(l => l.ModuleId)
                .HasConversion(
                    id => id.Value,
                    value => new ModuleId(value));
        }
    }
}