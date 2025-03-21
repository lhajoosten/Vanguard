using Vanguard.Common.Abstractions;
using Vanguard.Common.Base;

namespace Vanguard.Domain.Events
{
    public record CertificateIssuedEvent(
            EnrollmentCertificateId CertificateId,
            EnrollmentId EnrollmentId,
            UserId UserId,
            CourseId CourseId) : IDomainEvent
    {
        public Guid Id { get; } = CertificateId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CertificateRevokedEvent(
        EnrollmentCertificateId CertificateId,
        EnrollmentId EnrollmentId,
        UserId UserId,
        CourseId CourseId,
        UserId RevokedById,
        string Reason) : IDomainEvent
    {
        public Guid Id { get; } = CertificateId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CertificateExpirationUpdatedEvent(
        EnrollmentCertificateId CertificateId,
        EnrollmentId EnrollmentId,
        DateTime ExpiresAt) : IDomainEvent
    {
        public Guid Id { get; } = CertificateId.Value;
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
