using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.EnrollmentAggregate;

namespace Vanguard.Infrastructure.Persistence.Configurations.EntityConfigurations.EnrollmenAggregate
{
    public class EnrollmentCertificateConfiguration : EntityBaseConfiguration<EnrollmentCertificate, EnrollmentCertificateId>
    {
        public override void Configure(EntityTypeBuilder<EnrollmentCertificate> builder)
        {
            // Call base configuration first
            base.Configure(builder);

            builder.ToTable("EnrollmentCertificates");

            builder.Property(c => c.Title)
                 .IsRequired()
                 .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.CertificateNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.IssuedAt)
                .IsRequired();

            builder.Property(c => c.ExpiresAt)
                .IsRequired(false);

            builder.Property(c => c.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.RevokedAt)
                .IsRequired(false);

            builder.Property(c => c.RevocationReason)
                .HasMaxLength(1000);

            builder.Property(c => c.SignatureImageUrl)
                .HasMaxLength(500);

            // Enrollment relationship
            builder.HasOne(c => c.Enrollment)
                .WithMany(e => e.Certificates)
                .HasForeignKey(c => c.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Recipient relationship
            builder.HasOne(c => c.Recipient)
                .WithMany()
                .HasForeignKey(c => c.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course relationship
            builder.HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // IssuedBy relationship
            builder.HasOne(c => c.IssuedBy)
                .WithMany()
                .HasForeignKey(c => c.IssuedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint for certificate number
            builder.HasIndex(c => c.CertificateNumber).IsUnique();
        }

        protected override void ConfigurePrimaryKey(EntityTypeBuilder<EnrollmentCertificate> builder)
        {
            builder.HasKey(c => c.Id);
        }

        protected override void ConfigurePrimaryKeyConversion(EntityTypeBuilder<EnrollmentCertificate> builder)
        {
            builder.Property(c => c.Id)
                .HasConversion(
                    id => id.Value,
                    value => new EnrollmentCertificateId(value));
        }

        protected override void ConfigureForeignKeyConversions(EntityTypeBuilder<EnrollmentCertificate> builder)
        {
            // Configure foreign key conversions for custom ID types
            builder.Property(c => c.EnrollmentId)
                .HasConversion(
                    id => id.Value,
                    value => new EnrollmentId(value));

            builder.Property(c => c.RecipientId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));

            builder.Property(c => c.CourseId)
                .HasConversion(
                    id => id.Value,
                    value => new CourseId(value));

            // Special handling for nullable foreign key
            builder.Property(c => c.IssuedById)
                .HasConversion(
                    id => id != null ? id.Value : Guid.Empty,
                    value => value != Guid.Empty ? new UserId(value) : null);
        }
    }
}