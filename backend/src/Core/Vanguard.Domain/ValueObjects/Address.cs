using Ardalis.GuardClauses;
using Vanguard.Core.Base;

namespace Vanguard.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }

        public Address(string street, string city, string state, string zipCode)
        {
            Guard.Against.NullOrEmpty(street, nameof(street));
            Guard.Against.NullOrEmpty(city, nameof(city));
            Guard.Against.NullOrEmpty(state, nameof(state));
            Guard.Against.NullOrEmpty(zipCode, nameof(zipCode));
            Guard.Against.InvalidZipCode(zipCode, nameof(zipCode));

            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return ZipCode;
        }
    }

    static internal partial class ZipCodeGuardExtensions
    {
        public static void InvalidZipCode(this IGuardClause guardClause, string zipCode, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentException("ZIP code cannot be null or whitespace.", parameterName);
            }

            if (!IsValidZipCode(zipCode))
            {
                throw new ArgumentException($"Invalid ZIP code format: {zipCode}", parameterName);
            }
        }

        private static bool IsValidZipCode(string zipCode)
        {
            // Dutch postal code validation (format: "1234 AB" or "1234AB")
            string dutchPattern = @"^[1-9]\d{3}\s?[A-Za-z]{2}$";
            if (System.Text.RegularExpressions.Regex.IsMatch(zipCode, dutchPattern))
            {
                return true;
            }

            return false;
        }
    }
}
