using Vanguard.Common.Base;

namespace Vanguard.Domain.ValueObjects
{
    /// <summary>
    /// Represents a phone number
    /// </summary>
    public class PhoneNumber : ValueObject
    {
        private PhoneNumber(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the phone number
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new phone number
        /// </summary>
        /// <param name="phoneNumber">The phone number</param>
        /// <returns>A new phone number, or null if the input is invalid</returns>
        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            // Remove all non-digit characters
            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Simple validation: phone numbers should have at least 10 digits
            if (digits.Length < 10)
                return null;

            return new PhoneNumber(digits);
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
