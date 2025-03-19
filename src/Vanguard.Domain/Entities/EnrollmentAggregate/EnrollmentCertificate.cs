using Ardalis.GuardClauses;
using Vanguard.Common.Base;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Domain.Enumerations;
using Vanguard.Domain.Events;

namespace Vanguard.Domain.Entities.EnrollmentAggregate
{
    public class EnrollmentCertificate : AggregateRootBase<EnrollmentCertificateId>
    {
        public EnrollmentId EnrollmentId { get; private set; } = null!;
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public UserId RecipientId { get; private set; } = null!;
        public CourseId CourseId { get; private set; } = null!;
        public string CertificateNumber { get; private set; } = string.Empty;
        public DateTime IssuedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; } = false;
        public DateTime? RevokedAt { get; private set; }
        public string RevocationReason { get; private set; } = string.Empty;
        public UserId? IssuedById { get; private set; }
        public string SignatureImageUrl { get; private set; } = string.Empty;

        // Navigation properties for EF Core
        public virtual Enrollment? Enrollment { get; private set; }
        public virtual User? Recipient { get; private set; }
        public virtual Course? Course { get; private set; }
        public virtual User? IssuedBy { get; private set; }

        private EnrollmentCertificate() { } // For EF Core

        private EnrollmentCertificate(
            EnrollmentCertificateId id,
            EnrollmentId enrollmentId,
            string title,
            string description,
            UserId recipientId,
            CourseId courseId,
            string certificateNumber,
            DateTime? expiresAt,
            UserId? issuedById,
            string signatureImageUrl) : base(id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(enrollmentId, nameof(enrollmentId), "Enrollment ID cannot be null");
            Guard.Against.NullOrWhiteSpace(title, nameof(title), "Certificate title cannot be empty");
            Guard.Against.Null(recipientId, nameof(recipientId), "Recipient ID cannot be null");
            Guard.Against.Null(courseId, nameof(courseId), "Course ID cannot be null");
            Guard.Against.NullOrWhiteSpace(certificateNumber, nameof(certificateNumber), "Certificate number cannot be empty");

            EnrollmentId = enrollmentId;
            Title = title;
            Description = description ?? string.Empty;
            RecipientId = recipientId;
            CourseId = courseId;
            CertificateNumber = certificateNumber;
            IssuedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            IssuedById = issuedById;
            SignatureImageUrl = signatureImageUrl ?? string.Empty;
        }

        public static EnrollmentCertificate Issue(
            Enrollment enrollment,
            string title,
            string description,
            string certificateNumber,
            DateTime? expiresAt = null,
            UserId? issuedById = null,
            string signatureImageUrl = "")
        {
            Guard.Against.Null(enrollment, nameof(enrollment));
            Guard.Against.InvalidInput(enrollment, nameof(enrollment), e => e.Status == EnrollmentStatus.Completed,
                "Cannot issue certificate for incomplete enrollment");

            var certificate = new EnrollmentCertificate(
                EnrollmentCertificateId.CreateUnique(),
                enrollment.Id,
                title,
                description,
                enrollment.UserId,
                enrollment.CourseId,
                certificateNumber,
                expiresAt,
                issuedById,
                signatureImageUrl);

            certificate.AddDomainEvent(new CertificateIssuedEvent(
                certificate.Id,
                enrollment.Id,
                enrollment.UserId,
                enrollment.CourseId));

            return certificate;
        }

        public void Revoke(string reason, UserId revokedById)
        {
            Guard.Against.NullOrWhiteSpace(reason, nameof(reason), "Revocation reason cannot be empty");
            Guard.Against.Null(revokedById, nameof(revokedById), "Revoker ID cannot be null");

            if (IsRevoked)
            {
                return; // Already revoked
            }

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            RevocationReason = reason;
            ModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new CertificateRevokedEvent(
                Id,
                EnrollmentId,
                RecipientId,
                CourseId,
                revokedById,
                reason));
        }

        public void UpdateExpiration(DateTime? expiresAt)
        {
            if (IsRevoked)
            {
                throw new InvalidOperationException("Cannot update expiration of a revoked certificate");
            }

            ExpiresAt = expiresAt;
            ModifiedAt = DateTime.UtcNow;

            if (expiresAt.HasValue)
            {
                AddDomainEvent(new CertificateExpirationUpdatedEvent(Id, EnrollmentId, expiresAt.Value));
            }
        }

        public void UpdateSignature(UserId issuedById, string signatureImageUrl)
        {
            Guard.Against.Null(issuedById, nameof(issuedById), "Issuer ID cannot be null");

            if (IsRevoked)
            {
                throw new InvalidOperationException("Cannot update signature of a revoked certificate");
            }

            IssuedById = issuedById;
            SignatureImageUrl = signatureImageUrl ?? string.Empty;
            ModifiedAt = DateTime.UtcNow;
        }

        public bool IsValid()
        {
            if (IsRevoked)
            {
                return false;
            }

            if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
    }
}