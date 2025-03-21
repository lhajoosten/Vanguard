using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.EnrollmentAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.EnrollmenAggregate
{
    public class CompletionRequirementConfiguration : EntityBaseConfiguration<CompletionRequirement, CompletionRequirementId>
    {
        public override void Configure(EntityTypeBuilder<CompletionRequirement> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("CompletionRequirements");

            builder.Property(r => r.RequiredValue)
                .IsRequired();

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.IsRequired)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.OrderIndex)
                .IsRequired();

            // Criteria enumeration reference
            builder.Property<int>("CriteriaId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(r => r.Criteria)
                .WithMany()
                .HasForeignKey("CriteriaId")
                .OnDelete(DeleteBehavior.Restrict);

            // Course relationship
            builder.HasOne(r => r.Course)
                .WithMany(c => c.CompletionRequirements)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<CompletionRequirement> builder)
        {
            builder.HasKey(r => r.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<CompletionRequirement> builder)
        {
            builder.Property(r => r.Id)
                .HasConversion(
                    id => id.Value,
                    value => new CompletionRequirementId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<CompletionRequirement> builder)
        {
            // Explicitly configure CourseId
            builder.Property(r => r.CourseId)
                .HasConversion(
                    id => id.Value,
                    value => new CourseId(value));
        }
    }
}