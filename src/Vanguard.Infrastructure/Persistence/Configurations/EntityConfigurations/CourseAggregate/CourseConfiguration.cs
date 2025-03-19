using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.SkillAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.CourseAggregate
{
    public class CourseConfiguration : EntityBaseConfiguration<Course, CourseId>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            // Call base configuration first - this handles common configuration for all entities
            base.Configure(builder);

            // Table name
            builder.ToTable("Courses");

            // Basic property configurations
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500);

            builder.Property(c => c.EstimatedDurationMinutes)
                .IsRequired();

            builder.Property(c => c.EnrollmentCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.PublishedAt)
                .IsRequired(false);

            // Level enumeration reference
            builder.Property<int>("LevelId")
                .IsRequired()
                .HasDefaultValue(1);

            // Navigation configurations
            builder.HasOne(c => c.Level)
                .WithMany()
                .HasForeignKey("LevelId")
                .OnDelete(DeleteBehavior.Restrict);

            // Creator one-to-many relationship
            builder.HasOne(c => c.Creator)
                .WithMany(x => x.CreatedCourses)
                .HasForeignKey("CreatorId")
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many relationship for Course and Skills
            builder.HasMany(c => c.Skills)
                .WithMany()
                .UsingEntity(
                    "CourseSkills",
                    j => j.HasOne(typeof(Skill)).WithMany().HasForeignKey("SkillId"),
                    j => j.HasOne(typeof(Course)).WithMany().HasForeignKey("CourseId"),
                    j =>
                    {
                        j.Property<CourseId>("CourseId")
                            .HasConversion(
                                id => id.Value,
                                value => new CourseId(value));
                        j.Property<SkillId>("SkillId")
                            .HasConversion(
                                id => id.Value,
                                value => new SkillId(value));
                        j.HasKey("CourseId", "SkillId");
                    });

            // Many-to-many relationship for CourseTags
            builder.HasMany(c => c.Tags)
                .WithMany(t => t.Courses)
                .UsingEntity(
                    "CourseCourseTags",
                    j => j.HasOne(typeof(CourseTag)).WithMany().HasForeignKey("CourseTagId"),
                    j => j.HasOne(typeof(Course)).WithMany().HasForeignKey("CourseId"),
                    j =>
                    {
                        j.Property<CourseId>("CourseId")
                            .HasConversion(
                                id => id.Value,
                                value => new CourseId(value));
                        j.Property<CourseTagId>("CourseTagId")
                            .HasConversion(
                                id => id.Value,
                                value => new CourseTagId(value));
                        j.HasKey("CourseId", "CourseTagId");
                    });


            // One-to-many for Modules
            ConfigureOneToMany(
                builder,
                c => c.Modules,
                m => m.Course!,
                DeleteBehavior.Cascade
            );

            // One-to-many for Enrollments
            ConfigureOneToMany(
                builder,
                c => c.Enrollments,
                e => e.Course!,
                DeleteBehavior.Cascade
            );

            // One-to-many for CompletionRequirements
            ConfigureOneToMany(
               builder,
               c => c.CompletionRequirements,
               r => r.Course!,
               DeleteBehavior.Cascade
           );

            // Owned entity collection for reviews
            builder.OwnsMany(c => c.Reviews, reviewBuilder =>
            {
                reviewBuilder.ToTable("CourseReviews");
                reviewBuilder.WithOwner().HasForeignKey("CourseId");
                reviewBuilder.HasKey("Id", "CourseId");

                // Explicitly configure the Id property with a value converter if needed
                reviewBuilder.Property(r => r.Id)
                    .HasConversion(
                        id => id,
                        value => value);

                reviewBuilder.Property(r => r.UserId)
                    .HasConversion(
                        id => id.Value,
                        value => new UserId(value));

                reviewBuilder.Property(r => r.Rating)
                    .IsRequired();

                reviewBuilder.Property(r => r.Comment)
                    .HasMaxLength(1000);
            });
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<Course> builder)
        {
            builder.Property(c => c.Id)
                .HasConversion(
                    id => id.Value,
                    value => new CourseId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<Course> builder)
        {
            builder.Property(c => c.CreatorId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));
        }
    }
}