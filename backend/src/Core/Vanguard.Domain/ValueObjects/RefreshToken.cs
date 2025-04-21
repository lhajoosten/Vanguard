using Ardalis.GuardClauses;
using Vanguard.Core.Base;

namespace Vanguard.Domain.ValueObjects
{
    public class RefreshToken : ValueObject
    {
        public string Token { get; private set; }
        public DateTime Expires { get; private set; }
        public DateTime Created { get; private set; }
        public string CreatedByIp { get; private set; }
        public DateTime? Revoked { get; private set; }
        public string RevokedByIp { get; private set; }
        public string ReplacedByToken { get; private set; }
        public string ReasonRevoked { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        protected RefreshToken() { }

        public RefreshToken(string token, DateTime expires, string createdByIp)
        {
            Guard.Against.NullOrWhiteSpace(token, nameof(token));
            Guard.Against.Null(expires, nameof(expires));
            Guard.Against.NullOrWhiteSpace(createdByIp, nameof(createdByIp));

            Token = token;
            Expires = expires;
            Created = DateTime.UtcNow;
            CreatedByIp = createdByIp;
        }

        public void Revoke(string revokedByIp, string reasonRevoked = null, string replacedByToken = null)
        {
            Guard.Against.NullOrWhiteSpace(revokedByIp, nameof(revokedByIp));

            Revoked = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
            ReasonRevoked = reasonRevoked;
            ReplacedByToken = replacedByToken;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Token;
            yield return Expires;
            yield return Created;
            yield return CreatedByIp;
            yield return Revoked;
            yield return RevokedByIp;
            yield return ReplacedByToken;
            yield return ReasonRevoked;
        }
    }
}