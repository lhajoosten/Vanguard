using Vanguard.Common.Base;

namespace Vanguard.Domain.Enumerations
{
    public class UITheme : Enumeration
    {
        public static UITheme Light = new(1, "Light", "#ffffff", "#333333", "light", "Light mode with white background");
        public static UITheme Dark = new(2, "Dark", "#222222", "#f8f8f8", "dark", "Dark mode with black background");
        public static UITheme System = new(3, "System", "", "", "system", "Follows system preference");
        public static UITheme Sepia = new(4, "Sepia", "#f4ecd8", "#5b4636", "sepia", "Sepia tone for comfortable reading");
        public static UITheme HighContrast = new(5, "High Contrast", "#000000", "#ffffff", "high-contrast", "High contrast for accessibility");
        public static UITheme BlueLight = new(6, "Blue Light Filter", "#f9f5e9", "#333333", "blue-light", "Reduced blue light for evening use");

        public string BackgroundColor { get; private set; }
        public string TextColor { get; private set; }
        public string CssClass { get; private set; }
        public string Description { get; private set; }

        private UITheme(int id, string name, string backgroundColor, string textColor, string cssClass, string description) : base(id, name)
        {
            BackgroundColor = backgroundColor;
            TextColor = textColor;
            CssClass = cssClass;
            Description = description;
        }

        // Methods for theme functionality
        public bool IsSystemTheme()
        {
            return this == System;
        }

        public bool IsAccessibilityTheme()
        {
            return this == HighContrast;
        }

        public string GetCssVariables()
        {
            if (IsSystemTheme())
            {
                return ""; // System theme handled by CSS @media (prefers-color-scheme)
            }

            return $"--bg-color: {BackgroundColor}; --text-color: {TextColor};";
        }

        public string GetIconName()
        {
            return this switch
            {
                var t when t == Light => "sun",
                var t when t == Dark => "moon",
                var t when t == System => "desktop",
                var t when t == Sepia => "book",
                var t when t == HighContrast => "adjust",
                var t when t == BlueLight => "eye",
                _ => "circle"
            };
        }

        public static UITheme GetThemeFromPreference(bool prefersDarkMode)
        {
            return prefersDarkMode ? Dark : Light;
        }
    }
}