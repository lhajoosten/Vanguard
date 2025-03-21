using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.SkillAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.SkillAggregate
{
    public class SkillAssessmentConfiguration : EntityBaseConfiguration<SkillAssessment, SkillAssessmentId>
    {
        public override void Configure(EntityTypeBuilder<SkillAssessment> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("SkillAssessments");

            builder.Property(a => a.Evidence)
                .HasMaxLength(2000);

            builder.Property(a => a.AssessedAt)
                .IsRequired();

            builder.Property(a => a.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.VerifiedAt)
                .IsRequired(false);

            // Level enumeration reference
            builder.Property<int>("LevelId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(a => a.Level)
                .WithMany()
                .HasForeignKey("LevelId")
                .OnDelete(DeleteBehavior.Restrict);

            // User relationship
            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Skill relationship
            builder.HasOne(a => a.Skill)
                .WithMany(s => s.Assessments)
                .HasForeignKey(a => a.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Questions relationship
            builder.HasMany(a => a.Questions)
                .WithOne(q => q.Assessment)
                .HasForeignKey(q => q.AssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Results relationship
            builder.HasMany(a => a.Results)
                .WithOne(r => r.Assessment)
                .HasForeignKey(r => r.AssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add Answers relationship to manage deletion explicitly
            builder.HasMany(a => a.Answers)
                .WithOne(ans => ans.Assessment)
                .HasForeignKey(ans => ans.AssessmentId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to break potential cascade cycles

            // VerifiedBy relationship
            builder.HasOne(a => a.VerifiedBy)
                .WithMany()
                .HasForeignKey(a => a.VerifiedById)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<SkillAssessment> builder)
        {
            builder.HasKey(a => a.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<SkillAssessment> builder)
        {
            builder.Property(a => a.Id)
                .HasConversion(
                    id => id.Value,
                    value => new SkillAssessmentId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<SkillAssessment> builder)
        {
            // Configure foreign key conversions for custom ID types
            builder.Property(a => a.UserId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));

            builder.Property(a => a.SkillId)
                .HasConversion(
                    id => id.Value,
                    value => new SkillId(value));

            // Special handling for nullable VerifiedById
            builder.Property(a => a.VerifiedById)
                .HasConversion(
                    id => id != null ? id.Value : Guid.Empty,
                    value => value != Guid.Empty ? new UserId(value) : null);
        }
    }
}