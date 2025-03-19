using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.SkillAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.SkillAggregate
{
    public class SkillAssessmentResultConfiguration : EntityBaseConfiguration<SkillAssessmentResult, SkillAssessmentResultId>
    {
        public override void Configure(EntityTypeBuilder<SkillAssessmentResult> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("SkillAssessmentResults");

            builder.Property(r => r.TotalScore)
                .IsRequired();

            builder.Property(r => r.MaximumPossibleScore)
                .IsRequired();

            builder.Property(r => r.CompletedAt)
                .IsRequired();

            builder.Property(r => r.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.VerifiedAt)
                .IsRequired(false);

            builder.Property(r => r.FeedbackNotes)
                .HasMaxLength(2000);

            // Assigned level enumeration reference
            builder.Property<int>("AssignedLevelId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(r => r.AssignedLevel)
                .WithMany()
                .HasForeignKey("AssignedLevelId")
                .OnDelete(DeleteBehavior.Restrict);

            // User relationship
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Skill relationship
            builder.HasOne(r => r.Skill)
                .WithMany()
                .HasForeignKey(r => r.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assessment relationship
            builder.HasOne(r => r.Assessment)
                .WithMany(a => a.Results)
                .HasForeignKey(r => r.AssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Answers collection
            builder.HasMany(r => r.Answers)
                .WithOne()
                .HasForeignKey("ResultId")
                .OnDelete(DeleteBehavior.Restrict);

            // VerifiedBy relationship
            builder.HasOne(r => r.VerifiedBy)
                .WithMany()
                .HasForeignKey(r => r.VerifiedById)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<SkillAssessmentResult> builder)
        {
            builder.HasKey(r => r.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<SkillAssessmentResult> builder)
        {
            builder.Property(r => r.Id)
                .HasConversion(
                    id => id.Value,
                    value => new SkillAssessmentResultId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<SkillAssessmentResult> builder)
        {
            // Configure foreign key conversions for custom ID types
            builder.Property(r => r.UserId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));

            builder.Property(r => r.SkillId)
                .HasConversion(
                    id => id.Value,
                    value => new SkillId(value));

            builder.Property(r => r.AssessmentId)
                .HasConversion(
                    id => id.Value,
                    value => new SkillAssessmentId(value));

            // Special handling for nullable VerifiedById
            builder.Property(r => r.VerifiedById)
                .HasConversion(
                    id => id != null ? id.Value : Guid.Empty,
                    value => value != Guid.Empty ? new UserId(value) : null);
        }
    }
}