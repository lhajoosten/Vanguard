using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vanguard.Common.Guards
{
    /// <summary>
    /// Provides guard methods for validating method arguments
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Guards against a null argument
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentNullException">Thrown when the argument is null</exception>
        public static void AgainstNull<T>(T argument, string parameterName)
            where T : class
        {
            if (argument is null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Guards against a null or empty string argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentNullException">Thrown when the argument is null</exception>
        /// <exception cref="ArgumentException">Thrown when the argument is empty</exception>
        public static void AgainstNullOrEmpty(string argument, string parameterName)
        {
            AgainstNull(argument, parameterName);

            if (argument.Length == 0)
                throw new ArgumentException("String cannot be empty", parameterName);
        }

        /// <summary>
        /// Guards against a null or whitespace string argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentNullException">Thrown when the argument is null</exception>
        /// <exception cref="ArgumentException">Thrown when the argument is empty or whitespace</exception>
        public static void AgainstNullOrWhiteSpace(string argument, string parameterName)
        {
            AgainstNull(argument, parameterName);

            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException("String cannot be empty or whitespace", parameterName);
        }

        /// <summary>
        /// Guards against a null or empty collection argument
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentNullException">Thrown when the argument is null</exception>
        /// <exception cref="ArgumentException">Thrown when the argument is empty</exception>
        public static void AgainstNullOrEmpty<T>(IEnumerable<T> argument, string parameterName)
        {
            AgainstNull(argument, parameterName);

            if (!argument.Any())
                throw new ArgumentException("Collection cannot be empty", parameterName);
        }

        /// <summary>
        /// Guards against an argument that does not meet a condition
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="argument">The argument to check</param>
        /// <param name="condition">The condition that must be true</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="message">The error message</param>
        /// <exception cref="ArgumentException">Thrown when the condition is false</exception>
        public static void AgainstCondition<T>(T argument, Func<T, bool> condition, string parameterName, string message)
        {
            if (!condition(argument))
                throw new ArgumentException(message, parameterName);
        }

        /// <summary>
        /// Guards against an argument that is outside a range
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="argument">The argument to check</param>
        /// <param name="min">The minimum value (inclusive)</param>
        /// <param name="max">The maximum value (inclusive)</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument is outside the range</exception>
        public static void AgainstOutOfRange<T>(T argument, T min, T max, string parameterName)
            where T : IComparable<T>
        {
            if (argument.CompareTo(min) < 0 || argument.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(parameterName, $"Value must be between {min} and {max}");
        }

        /// <summary>
        /// Guards against a negative numeric argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument is negative</exception>
        public static void AgainstNegative(int argument, string parameterName)
        {
            if (argument < 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative");
        }

        /// <summary>
        /// Guards against a negative numeric argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument is negative</exception>
        public static void AgainstNegative(long argument, string parameterName)
        {
            if (argument < 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative");
        }

        /// <summary>
        /// Guards against a negative or zero numeric argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument is negative or zero</exception>
        public static void AgainstNegativeOrZero(int argument, string parameterName)
        {
            if (argument <= 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative or zero");
        }

        /// <summary>
        /// Guards against a negative or zero numeric argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument is negative or zero</exception>
        public static void AgainstNegativeOrZero(long argument, string parameterName)
        {
            if (argument <= 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative or zero");
        }

        /// <summary>
        /// Guards against a negative numeric argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument is negative</exception>
        public static void AgainstNegative(decimal argument, string parameterName)
        {
            if (argument < 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative");
        }

        /// <summary>
        /// Guards against a negative or zero numeric argument
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument is negative or zero</exception>
        public static void AgainstNegativeOrZero(decimal argument, string parameterName)
        {
            if (argument <= 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative or zero");
        }

        /// <summary>
        /// Guards against a string that doesn't match a regular expression
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="regex">The regular expression</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentException">Thrown when the string doesn't match the regex</exception>
        public static void AgainstInvalidFormat(string argument, string regex, string parameterName)
        {
            AgainstNull(argument, parameterName);

            if (!Regex.IsMatch(argument, regex))
                throw new ArgumentException("Value has an invalid format", parameterName);
        }

        /// <summary>
        /// Guards against a string that exceeds a maximum length
        /// </summary>
        /// <param name="argument">The argument to check</param>
        /// <param name="maxLength">The maximum allowed length</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <exception cref="ArgumentException">Thrown when the string exceeds the maximum length</exception>
        public static void AgainstExceedingLength(string argument, int maxLength, string parameterName)
        {
            AgainstNull(argument, parameterName);

            if (argument.Length > maxLength)
                throw new ArgumentException($"String cannot exceed {maxLength} characters", parameterName);
        }
    }
}
