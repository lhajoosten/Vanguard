using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Vanguard.Common.Extensions
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Converts a string to a URL-friendly slug.
        /// </summary>
        /// <param name="text">The text to convert</param>
        /// <returns>A URL-friendly slug</returns>
        public static string ToSlug(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Convert to lowercase
            text = text.ToLowerInvariant();

            // Remove diacritics using normalization.
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }
            text = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // Replace whitespace with hyphens.
            text = WhiteSpaceRegEx().Replace(text, "-");

            // Remove invalid characters: keep only lowercase letters, digits, and hyphens.
            text = InvalidCharsRegEx().Replace(text, string.Empty);

            // Remove multiple hyphens.
            text = MultipleHyphensRegEx().Replace(text, "-");

            // Trim hyphens from the start and end.
            text = text.Trim('-');

            return text;
        }

        /// <summary>
        /// Truncates a string to a specified length and appends a suffix if truncated.
        /// If possible, truncation avoids splitting a word in half.
        /// </summary>
        /// <param name="text">The text to truncate.</param>
        /// <param name="maxLength">The maximum number of characters from the original text to use before appending the suffix.</param>
        /// <param name="suffix">The suffix to append if truncation occurs.</param>
        /// <returns>The truncated string with suffix, or the original string if no truncation is needed.</returns>
        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            // If maxLength is too short to even accommodate the suffix, return the suffix.
            if (maxLength <= suffix.Length)
                return suffix;

            // Take the initial substring of the specified maxLength.
            string truncated = text.Substring(0, maxLength);

            // Try to avoid breaking a word: if there is a space in the substring,
            // truncate up to the last space.
            int lastSpaceIndex = truncated.LastIndexOf(' ');
            if (lastSpaceIndex > 0)
            {
                truncated = truncated.Substring(0, lastSpaceIndex);
            }

            return truncated + suffix;
        }

        /// <summary>
        /// Checks if a string contains another string, ignoring case
        /// </summary>
        /// <param name="text">The text to search in</param>
        /// <param name="value">The value to search for</param>
        /// <returns>True if the text contains the value, ignoring case, false otherwise</returns>
        public static bool ContainsIgnoreCase(this string text, string value)
        {
            if (text == null || value == null)
                return false;

            value = value.Trim();
            if (value.Length == 0)
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

        [GeneratedRegex(@"\s+")]
        private static partial Regex WhiteSpaceRegEx();
        [GeneratedRegex(@"[^a-z0-9\-]")]
        private static partial Regex InvalidCharsRegEx();
        [GeneratedRegex(@"-+")]
        private static partial Regex MultipleHyphensRegEx();
    }
}
