using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.UserAggregate
{
    public class UserSettingsConfiguration : EntityBaseConfiguration<UserSettings, UserSettingsId>
    {
        public override void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("UserSettings");

            // Notification settings
            builder.Property(s => s.EmailNotificationsEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.CourseUpdatesEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.AssignmentRemindersEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.NewMessageNotificationsEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.MarketingEmailsEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            // Display settings
            builder.Property<int>("ThemeId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(s => s.Theme)
                .WithMany()
                .HasForeignKey("ThemeId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property<int>("PreferredLanguageId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(s => s.PreferredLanguage)
                .WithMany()
                .HasForeignKey("PreferredLanguageId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.CompactViewEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            // Learning settings
            builder.Property(s => s.AutoPlayVideos)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.ShowCompletedCourses)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.DefaultVideoPlaybackSpeed)
                .IsRequired()
                .HasDefaultValue(100);

            // Privacy settings
            builder.Property<int>("ProfileVisibilityId")
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(s => s.ProfileVisibility)
                .WithMany()
                .HasForeignKey("ProfileVisibilityId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.ShowEnrollmentHistory)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.ShowAchievements)
                .IsRequired()
                .HasDefaultValue(true);

            // User relationship - the other side is configured in UserConfiguration
            builder.HasOne(s => s.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<UserSettings> builder)
        {
            builder.HasKey(s => s.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<UserSettings> builder)
        {
            builder.Property(s => s.Id)
                .HasConversion(
                    id => id.Value,
                    value => new UserSettingsId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<UserSettings> builder)
        {
            // Configure foreign key conversions for custom ID types
            builder.Property(s => s.UserId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));
        }
    }
}