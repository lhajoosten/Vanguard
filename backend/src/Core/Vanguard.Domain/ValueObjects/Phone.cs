using Ardalis.GuardClauses;
using Vanguard.Core.Base;

namespace Vanguard.Domain.ValueObjects
{
    public class Phone : ValueObject
    {
        public string Number { get; private set; }

        public Phone(string number)
        {
            Guard.Against.NullOrWhiteSpace(number, nameof(number));
            Guard.Against.InvalidPhoneNumber(number, nameof(number));

            Number = number;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
        }
    }

    static internal partial class PhoneGuardExtensions
    {
        public static void InvalidPhoneNumber(this IGuardClause guardClause, string phoneNumber, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("Phone number cannot be null or empty.", parameterName);
            }

            if (!IsValidPhoneNumber(phoneNumber))
            {
                throw new ArgumentException($"Invalid Dutch phone number format: {phoneNumber}", parameterName);
            }
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Remove any spaces, hyphens, or parentheses for standardized checking
            string normalizedNumber = System.Text.RegularExpressions.Regex.Replace(
                phoneNumber, @"[\s\-\(\)]", "");

            // Dutch mobile numbers: starting with 06 followed by 8 digits
            if (DutchMobile().IsMatch(normalizedNumber))
            {
                return true;
            }

            // Dutch landline numbers: 10 digits total, starting with 0
            // Area codes can be 2-3 digits (after the leading 0)
            if (DutchLandline().IsMatch(normalizedNumber))
            {
                return true;
            }

            // Dutch international format: +31 followed by 9 digits (without the leading 0)
            if (DutchInternational().IsMatch(normalizedNumber))
            {
                return true;
            }

            // For numbers with the area code in parentheses
            if (phoneNumber.Contains("(") && phoneNumber.Contains(")"))
            {
                string withoutParentheses = System.Text.RegularExpressions.Regex.Replace(
                    phoneNumber, @"[\s\-\(\)]", "");
                if (System.Text.RegularExpressions.Regex.IsMatch(withoutParentheses, @"^0\d{9}$"))
                {
                    return true;
                }
            }

            return false;
        }

        [System.Text.RegularExpressions.GeneratedRegex(@"^06\d{8}$")]
        private static partial System.Text.RegularExpressions.Regex DutchMobile();

        [System.Text.RegularExpressions.GeneratedRegex(@"^0[1-9]\d{8}$")]
        private static partial System.Text.RegularExpressions.Regex DutchLandline();

        [System.Text.RegularExpressions.GeneratedRegex(@"^\+31\d{9}$")]
        private static partial System.Text.RegularExpressions.Regex DutchInternational();
    }
}
