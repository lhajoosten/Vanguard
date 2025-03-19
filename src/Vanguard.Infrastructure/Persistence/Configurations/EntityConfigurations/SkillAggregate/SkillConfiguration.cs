using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.SkillAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.SkillAggregate
{
    public class SkillConfiguration : EntityBaseConfiguration<Skill, SkillId>
    {
        public override void Configure(EntityTypeBuilder<Skill> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("Skills");

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            // Category relationship
            builder.HasOne(s => s.Category)
                .WithMany(c => c.Skills)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assessments relationship
            builder.HasMany(s => s.Assessments)
                .WithOne(a => a.Skill)
                .HasForeignKey(a => a.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many with Courses
            builder.HasMany<Course>()
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "CourseSkills",
                    j => j
                        .HasOne<Course>()
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Skill>()
                        .WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        // Store as Guid in DB
                        j.Property<CourseId>("CourseId")
                            .HasColumnName("CourseId")
                            .HasConversion(id => id.Value, value => new CourseId(value));
                        j.Property<SkillId>("SkillId")
                            .HasColumnName("SkillId")
                            .HasConversion(id => id.Value, value => new SkillId(value));
                        j.HasKey("CourseId", "SkillId");
                        j.HasIndex("SkillId");
                    }
                );
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<Skill> builder)
        {
            builder.Property(s => s.Id)
                .HasConversion(
                    id => id.Value,
                    value => new SkillId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<Skill> builder)
        {
            // Explicitly configure CategoryId
            builder.Property(s => s.CategoryId)
                .HasConversion(
                    id => id.Value,
                    value => new SkillCategoryId(value));
        }
    }
}