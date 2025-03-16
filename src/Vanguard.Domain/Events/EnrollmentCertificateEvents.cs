using Vanguard.Domain.Abstraction;
using Vanguard.Domain.ValueObjects;

namespace Vanguard.Domain.Events
{
    public record CertificateIssuedEvent(
            EnrollmentCertificateId CertificateId,
            EnrollmentId EnrollmentId,
            UserId UserId,
            CourseId CourseId) : IDomainEvent
    {
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
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record CertificateExpirationUpdatedEvent(
        EnrollmentCertificateId CertificateId,
        EnrollmentId EnrollmentId,
        DateTime ExpiresAt) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
