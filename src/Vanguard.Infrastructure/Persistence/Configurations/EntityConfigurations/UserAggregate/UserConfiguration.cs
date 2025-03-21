using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.UserAggregate
{
    public class UserConfiguration : EntityBaseConfiguration<User, UserId>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("Users");

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastLoginAt)
                .IsRequired(false);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Unique index on email
            builder.HasIndex(u => u.Email)
                .IsUnique();

            // One-to-one relationship with UserProfile
            builder.HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-one relationship with UserSettings
            builder.HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many relationship with Roles
            builder.HasMany(u => u.Roles)
                .WithMany()
                .UsingEntity(
                    "UserRoles",
                    j => j.HasOne(typeof(Role)).WithMany().HasForeignKey("RoleId"),
                    j => j.HasOne(typeof(User)).WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.Property<UserId>("UserId")
                            .HasConversion(
                                id => id.Value,
                                value => new UserId(value));
                        j.Property<RoleId>("RoleId")
                            .HasConversion(
                                id => id.Value,
                                value => new RoleId(value));
                        j.HasKey("UserId", "RoleId");
                    }
                );

            // Relationships for SkillAssessments
            builder.HasMany(u => u.SkillAssessments)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.VerifiedSkillAssessments)
                .WithOne(a => a.VerifiedBy)
                .HasForeignKey(a => a.VerifiedById)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));
        }
    }
}