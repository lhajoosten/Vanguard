using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.UserAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.UserAggregate
{
    public class UserProfileConfiguration : EntityBaseConfiguration<UserProfile, UserProfileId>
    {
        public override void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("UserProfiles");

            builder.Property(p => p.Bio)
                .HasMaxLength(2000);

            builder.Property(p => p.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(p => p.JobTitle)
                .HasMaxLength(100);

            builder.Property(p => p.Company)
                .HasMaxLength(100);

            builder.Property(p => p.Website)
                .HasMaxLength(200);

            builder.Property(p => p.LinkedInUrl)
                .HasMaxLength(200);

            builder.Property(p => p.GitHubUrl)
                .HasMaxLength(200);

            builder.Property(p => p.TwitterUrl)
                .HasMaxLength(200);

            // User relationship - the other side is configured in UserConfiguration
            builder.HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(p => p.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<UserProfile> builder)
        {
            builder.Property(p => p.Id)
                .HasConversion(
                    id => id.Value,
                    value => new UserProfileId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<UserProfile> builder)
        {
            // Configure foreign key conversions for custom ID types
            builder.Property(p => p.UserId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));
        }
    }
}