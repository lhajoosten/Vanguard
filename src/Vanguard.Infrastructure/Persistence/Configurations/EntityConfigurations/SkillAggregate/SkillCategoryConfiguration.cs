using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.SkillAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.SkillAggregate
{
    public class SkillCategoryConfiguration : EntityBaseConfiguration<SkillCategory, SkillCategoryId>
    {
        public override void Configure(EntityTypeBuilder<SkillCategory> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("SkillCategories");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            // Unique index on name
            builder.HasIndex(c => c.Name)
                .IsUnique();

            // One-to-many for Skills
            builder.HasMany(c => c.Skills)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<SkillCategory> builder)
        {
            builder.HasKey(c => c.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<SkillCategory> builder)
        {
            builder.Property(c => c.Id)
                .HasConversion(
                    id => id.Value,
                    value => new SkillCategoryId(value));
        }
    }
}