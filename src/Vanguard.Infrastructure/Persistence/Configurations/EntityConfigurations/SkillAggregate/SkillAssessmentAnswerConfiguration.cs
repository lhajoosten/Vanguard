using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.SkillAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.SkillAggregate
{
    public class SkillAssessmentAnswerConfiguration : EntityBaseConfiguration<SkillAssessmentAnswer, SkillAssessmentAnswerId>
    {
        public override void Configure(EntityTypeBuilder<SkillAssessmentAnswer> builder)
        {
            base.Configure(builder);

            builder.ToTable("SkillAssessmentAnswers");

            builder.Property(a => a.TextAnswer)
                .HasMaxLength(2000);

            builder.Property(a => a.ScoreEarned)
                .IsRequired();

            builder.Property(a => a.AnsweredAt)
                .IsRequired();

            // User relationship
            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Question relationship
            builder.HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assessment relationship
            builder.HasOne(a => a.Assessment)
                .WithMany()
                .HasForeignKey(a => a.AssessmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Selected Options collection - stored as JSON
            builder.Property(a => a.SelectedOptions)
                .HasConversion(
                    options => string.Join(",", options.Select(o => o.Value)),
                    value => string.IsNullOrEmpty(value)
                        ? new List<SkillAssessmentQuestionOptionId>()
                        : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(id => new SkillAssessmentQuestionOptionId(Guid.Parse(id)))
                            .ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<IReadOnlyCollection<SkillAssessmentQuestionOptionId>>(
                        (c1, c2) => c1.Count == c2.Count && !c1.Except(c2).Any(),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    ));
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<SkillAssessmentAnswer> builder)
        {
            builder.HasKey(a => a.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<SkillAssessmentAnswer> builder)
        {
            builder.Property(a => a.Id)
                .HasConversion(
                    id => id.Value,
                    value => new SkillAssessmentAnswerId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<SkillAssessmentAnswer> builder)
        {
            builder.Property(a => a.UserId)
               .HasConversion(
                   id => id.Value,
                   value => new UserId(value));

            builder.Property(a => a.QuestionId)
                .HasConversion(
                    id => id.Value,
                    value => new SkillAssessmentQuestionId(value));

            builder.Property(a => a.AssessmentId)
                .HasConversion(
                    id => id.Value,
                    value => new SkillAssessmentId(value));
        }
    }
}