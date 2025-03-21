using Vanguard.Common.Base;

namespace Vanguard.Domain.ValueObjects
{
    /// <summary>
    /// Represents an email address
    /// </summary>
    public class Email : ValueObject
    {
        private Email(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the email address
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new email address
        /// </summary>
        /// <param name="email">The email address</param>
        /// <returns>A new email address, or null if the input is invalid</returns>
        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            email = email.Trim();

            if (email.Length > 320 || !IsValidEmail(email))
                return null;

            return new Email(email);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
