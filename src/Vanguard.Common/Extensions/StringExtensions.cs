using System.Text.RegularExpressions;

namespace Vanguard.Common.Extensions
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to a URL-friendly slug
        /// </summary>
        /// <param name="text">The text to convert</param>
        /// <returns>A URL-friendly slug</returns>
        public static string ToSlug(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Convert to lowercase
            text = text.ToLowerInvariant();

            // Remove diacritics (accents)
            var bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            text = System.Text.Encoding.ASCII.GetString(bytes);

            // Replace spaces with hyphens
            text = Regex.Replace(text, @"\s+", "-");

            // Remove invalid characters
            text = Regex.Replace(text, @"[^a-z0-9\-]", string.Empty);

            // Remove multiple hyphens
            text = Regex.Replace(text, @"-+", "-");

            // Trim hyphens from start and end
            text = text.Trim('-');

            return text;
        }

        /// <summary>
        /// Truncates a string to a specified length
        /// </summary>
        /// <param name="text">The text to truncate</param>
        /// <param name="maxLength">The maximum length</param>
        /// <param name="suffix">The suffix to add if truncated</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            var truncatedLength = maxLength - suffix.Length;
            if (truncatedLength <= 0)
                return suffix;

            return text.Substring(0, truncatedLength) + suffix;
        }

        /// <summary>
        /// Checks if a string contains another string, ignoring case
        /// </summary>
        /// <param name="text">The text to search in</param>
        /// <param name="value">The value to search for</param>
        /// <returns>True if the text contains the value, ignoring case, false otherwise</returns>
        public static bool ContainsIgnoreCase(this string text, string value)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value))
                return false;

            return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Checks if a string equals another string, ignoring case
        /// </summary>
        /// <param name="text">The text to compare</param>
        /// <param name="value">The value to compare with</param>
        /// <returns>True if the text equals the value, ignoring case, false otherwise</returns>
        public static bool EqualsIgnoreCase(this string text, string value)
        {
            if (text == null && value == null)
                return true;

            if (text == null || value == null)
                return false;

            return text.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
