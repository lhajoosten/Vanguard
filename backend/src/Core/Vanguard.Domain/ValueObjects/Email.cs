using System.Text.RegularExpressions;
using Vanguard.Core.Base;

namespace Vanguard.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        private static readonly HashSet<string> CommonFreeEmailDomains = new(StringComparer.OrdinalIgnoreCase)
        {
            "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "icloud.com", "protonmail.com"
        };

        public string Address { get; }
        public string Username { get; }
        public string Domain { get; }

        public Email(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Email address cannot be null or empty.", nameof(address));
            }

            // Normalize by trimming and converting to lowercase
            address = address.Trim().ToLowerInvariant();

            if (!IsValidEmail(address))
            {
                throw new ArgumentException($"Invalid email address format: {address}", nameof(address));
            }

            Address = address;

            // Split into username and domain parts
            var parts = address.Split('@');
            Username = parts[0];
            Domain = parts[1];
        }

        public static Email Create(string address)
        {
            return new Email(address);
        }

        public bool IsFreeEmailService()
        {
            return CommonFreeEmailDomains.Contains(Domain);
        }

        public bool HasSameDomain(Email other)
        {
            return string.Equals(Domain, other.Domain, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Address;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address.ToLowerInvariant(); // Case-insensitive comparison
        }

        private static bool IsValidEmail(string email)
        {
            // More comprehensive regex for email validation
            // This follows most RFC 5322 rules but is still practical
            string pattern = @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";

            try
            {
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch (RegexMatchTimeoutException)
            {
                return false; // Handle potential regex timeout
            }
        }
    }
}
