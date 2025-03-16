using System.Globalization;
using Vanguard.Domain.Base;

namespace Vanguard.Domain.Enumerations
{
    public class Language : Enumeration
    {
        public readonly static Language English = new(1, "English", "en-US", "English", false);
        public readonly static Language Spanish = new(2, "Spanish", "es-ES", "Español", false);
        public readonly static Language French = new(3, "French", "fr-FR", "Français", false);
        public readonly static Language German = new(4, "German", "de-DE", "Deutsch", false);
        public readonly static Language Chinese = new(5, "Chinese", "zh-CN", "中文", false);
        public readonly static Language Japanese = new(6, "Japanese", "ja-JP", "日本語", false);
        public readonly static Language Arabic = new(7, "Arabic", "ar-SA", "العربية", true);
        public readonly static Language Russian = new(8, "Russian", "ru-RU", "Русский", false);
        public readonly static Language Portuguese = new(9, "Portuguese", "pt-BR", "Português", false);
        public readonly static Language Hindi = new(10, "Hindi", "hi-IN", "हिन्दी", false);

        public string CultureCode { get; private set; }
        public string NativeName { get; private set; }
        public bool IsRightToLeft { get; private set; }
        public CultureInfo Culture => new(CultureCode);

        private Language(int id, string name, string cultureCode, string nativeName, bool isRightToLeft) : base(id, name)
        {
            CultureCode = cultureCode;
            NativeName = nativeName;
            IsRightToLeft = isRightToLeft;
        }

        // Methods to help with language-related functionality
        public static Language FromCultureCode(string cultureCode)
        {
            return GetAll<Language>().FirstOrDefault(l => l.CultureCode == cultureCode)
                ?? English; // Default to English if not found
        }

        public static IEnumerable<Language> GetSupportedLanguages()
        {
            return GetAll<Language>();
        }

        public bool SupportsLocalization()
        {
            // Implementation would check if resources exist for this language
            // This is a placeholder
            return true;
        }

        public string GetLanguageDirectionHtml()
        {
            return IsRightToLeft ? "dir=\"rtl\"" : "dir=\"ltr\"";
        }
    }
}
