using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.SkillAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.SkillAggregate
{
    public class SkillAssessmentQuestionConfiguration : EntityBaseConfiguration<SkillAssessmentQuestion, SkillAssessmentQuestionId>
    {
        public override void Configure(EntityTypeBuilder<SkillAssessmentQuestion> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("SkillAssessmentQuestions");

            builder.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(q => q.Explanation)
                .HasMaxLength(2000);

            builder.Property(q => q.PointValue)
                .IsRequired();

            builder.Property(q => q.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(q => q.OrderIndex)
                .IsRequired();

            // Type enumeration reference
            builder.Property<int>("TypeId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(q => q.Type)
                .WithMany()
                .HasForeignKey("TypeId")
                .OnDelete(DeleteBehavior.Restrict);

            // Difficulty enumeration reference
            builder.Property<int>("DifficultyId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(q => q.Difficulty)
                .WithMany()
                .HasForeignKey("DifficultyId")
                .OnDelete(DeleteBehavior.Restrict);

            // Skill relationship
            builder.HasOne(q => q.Skill)
                .WithMany()
                .HasForeignKey(q => q.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assessment relationship
            builder.HasOne(q => q.Assessment)
                .WithMany(a => a.Questions)
                .HasForeignKey(q => q.AssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Owned entity collection for options
            builder.OwnsMany(q => q.Options, optionBuilder =>
            {
                optionBuilder.ToTable("SkillAssessmentQuestionOptions");
                optionBuilder.WithOwner().HasForeignKey("QuestionId");
                optionBuilder.HasKey("Id", "QuestionId");

                // Explicitly configure the Id property with a value converter
                optionBuilder.Property(o => o.Id)
                    .HasConversion(
                        id => id.Value,
                        value => new SkillAssessmentQuestionOptionId(value));

                optionBuilder.Property(o => o.Text)
                    .IsRequired()
                    .HasMaxLength(500);

                optionBuilder.Property(o => o.IsCorrect)
                    .IsRequired();

                optionBuilder.Property(o => o.OrderIndex)
                    .IsRequired();
            });
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<SkillAssessmentQuestion> builder)
        {
            builder.HasKey(q => q.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<SkillAssessmentQuestion> builder)
        {
            builder.Property(q => q.Id)
                .HasConversion(
                    id => id.Value,
                    value => new SkillAssessmentQuestionId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<SkillAssessmentQuestion> builder)
        {
            // Configure foreign key conversions for custom ID types
            builder.Property(q => q.SkillId)
                .HasConversion(
                    id => id.Value,
                    value => new SkillId(value));
        }
    }
}