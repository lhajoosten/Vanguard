using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.EnrollmentAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.EnrollmenAggregate
{
    public class EnrollmentConfiguration : EntityBaseConfiguration<Enrollment, EnrollmentId>
    {
        public override void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("Enrollments");

            builder.Property(e => e.ProgressPercentage)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.CompletedAt)
                .IsRequired(false);

            builder.Property(e => e.LastAccessedAt)
                .IsRequired(false);

            builder.Property(e => e.FinalGrade)
                .IsRequired(false);

            builder.Property(e => e.CompletedAssignments)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.CompletedQuizzes)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.HasPassedFinalExam)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.FinalExamScore)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.DiscussionPostCount)
                .IsRequired()
                .HasDefaultValue(0);

            // Status enumeration reference
            builder.Property<int>("StatusId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey("StatusId")
                .OnDelete(DeleteBehavior.Restrict);

            // User relationship
            builder.HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course relationship
            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many for Certificates
            ConfigureOneToMany(
                builder,
                e => e.Certificates,
                c => c.Enrollment!,
                DeleteBehavior.Cascade
            );

            // Store lesson completions as JSON
            builder.Property(e => e.LessonCompletions)
                .HasConversion(
                    dict => JsonSerializer.Serialize(dict.ToDictionary(kv => kv.Key.Value, kv => kv.Value),
                        new JsonSerializerOptions { WriteIndented = false }),
                    json => JsonSerializer.Deserialize<Dictionary<Guid, bool>>(json,
                        new JsonSerializerOptions { WriteIndented = false })!
                        .ToDictionary(kv => new LessonId(kv.Key), kv => kv.Value))
                .Metadata.SetValueComparer(
                    new ValueComparer<IReadOnlyDictionary<LessonId, bool>>(
                        (c1, c2) => c1.Count == c2.Count && !c1.Except(c2).Any(),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    ));

            // Notes owned entity collection
            builder.OwnsMany(e => e.Notes, noteBuilder =>
            {
                noteBuilder.ToTable("EnrollmentNotes");
                noteBuilder.WithOwner().HasForeignKey("EnrollmentId");
                noteBuilder.HasKey("Id", "EnrollmentId");

                noteBuilder.Property(n => n.Content)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            // Unique constraint for user-course combination
            builder.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(e => e.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<Enrollment> builder)
        {
            builder.Property(e => e.Id)
                .HasConversion(
                    id => id.Value,
                    value => new EnrollmentId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<Enrollment> builder)
        {
            // Configure foreign key conversions for custom ID types
            builder.Property(e => e.UserId)
               .HasConversion(
                   id => id.Value,
                   value => new UserId(value));

            builder.Property(e => e.CourseId)
                .HasConversion(
                    id => id.Value,
                    value => new CourseId(value));

            // Special handling for nullable foreign key
            builder.Property(e => e.CurrentLessonId)
                .HasConversion(
                    id => id != null ? id.Value : Guid.Empty,
                    value => value != Guid.Empty ? new LessonId(value) : null);
        }
    }
}