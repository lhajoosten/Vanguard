using Vanguard.Common.Base;

namespace Vanguard.Domain.ValueObjects
{
    /// <summary>
    /// Represents a money value with a currency
    /// </summary>
    public class Money : ValueObject
    {
        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// Gets the amount
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Gets the currency code (ISO 4217)
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Creates a new money value
        /// </summary>
        /// <param name="amount">The amount</param>
        /// <param name="currency">The currency code (ISO 4217)</param>
        /// <returns>A new money value</returns>
        public static Money Create(decimal amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty", nameof(currency));

            return new Money(amount, currency.Trim().ToUpper());
        }

        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            return new Money(left.Amount - right.Amount, left.Currency);
        }

        public static Money operator *(Money left, decimal right)
        {
            return new Money(left.Amount * right, left.Currency);
        }

        public static Money operator /(Money left, decimal right)
        {
            if (right == 0)
                throw new DivideByZeroException();

            return new Money(left.Amount / right, left.Currency);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Amount} {Currency}";
        }
    }
}
