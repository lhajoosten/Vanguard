using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class UIThemeTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllThemes()
        {
            // Act
            var allThemes = Vanguard.Common.Base.Enumeration.GetAll<UITheme>();

            // Assert
            Assert.Equal(6, allThemes.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectTheme()
        {
            // Act
            var theme = Vanguard.Common.Base.Enumeration.FromId<UITheme>(3);

            // Assert
            Assert.Equal(UITheme.System, theme);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectTheme()
        {
            // Act
            var theme = Vanguard.Common.Base.Enumeration.FromName<UITheme>("Dark");

            // Assert
            Assert.Equal(UITheme.Dark, theme);
        }

        [Theory]
        [InlineData(1, "Light")]
        [InlineData(2, "Dark")]
        [InlineData(3, "System")]
        [InlineData(4, "Sepia")]
        [InlineData(5, "High Contrast")]
        [InlineData(6, "Blue Light Filter")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var theme = Vanguard.Common.Base.Enumeration.FromId<UITheme>(id);

            // Assert
            Assert.Equal(expectedName, theme.Name);
        }

        [Theory]
        [InlineData(1, "#ffffff")]
        [InlineData(2, "#222222")]
        [InlineData(3, "")]
        [InlineData(4, "#f4ecd8")]
        [InlineData(5, "#000000")]
        [InlineData(6, "#f9f5e9")]
        public void BackgroundColor_ShouldMatchExpected(int id, string expectedColor)
        {
            // Act
            var theme = Vanguard.Common.Base.Enumeration.FromId<UITheme>(id);

            // Assert
            Assert.Equal(expectedColor, theme.BackgroundColor);
        }

        [Theory]
        [InlineData(1, "#333333")]
        [InlineData(2, "#f8f8f8")]
        [InlineData(3, "")]
        [InlineData(4, "#5b4636")]
        [InlineData(5, "#ffffff")]
        [InlineData(6, "#333333")]
        public void TextColor_ShouldMatchExpected(int id, string expectedColor)
        {
            // Act
            var theme = Vanguard.Common.Base.Enumeration.FromId<UITheme>(id);

            // Assert
            Assert.Equal(expectedColor, theme.TextColor);
        }

        [Theory]
        [InlineData(1, "light")]
        [InlineData(2, "dark")]
        [InlineData(3, "system")]
        [InlineData(4, "sepia")]
        [InlineData(5, "high-contrast")]
        [InlineData(6, "blue-light")]
        public void CssClass_ShouldMatchExpected(int id, string expectedClass)
        {
            // Act
            var theme = Vanguard.Common.Base.Enumeration.FromId<UITheme>(id);

            // Assert
            Assert.Equal(expectedClass, theme.CssClass);
        }

        [Theory]
        [InlineData(1, "Light mode with white background")]
        [InlineData(2, "Dark mode with black background")]
        [InlineData(3, "Follows system preference")]
        [InlineData(4, "Sepia tone for comfortable reading")]
        [InlineData(5, "High contrast for accessibility")]
        [InlineData(6, "Reduced blue light for evening use")]
        public void Description_ShouldMatchExpected(int id, string expectedDescription)
        {
            // Act
            var theme = Vanguard.Common.Base.Enumeration.FromId<UITheme>(id);

            // Assert
            Assert.Equal(expectedDescription, theme.Description);
        }

        [Fact]
        public void IsSystemTheme_ShouldReturnTrueOnlyForSystemTheme()
        {
            // Assert
            Assert.True(UITheme.System.IsSystemTheme());
            Assert.False(UITheme.Light.IsSystemTheme());
            Assert.False(UITheme.Dark.IsSystemTheme());
            Assert.False(UITheme.Sepia.IsSystemTheme());
            Assert.False(UITheme.HighContrast.IsSystemTheme());
            Assert.False(UITheme.BlueLight.IsSystemTheme());
        }

        [Fact]
        public void IsAccessibilityTheme_ShouldReturnTrueOnlyForHighContrastTheme()
        {
            // Assert
            Assert.True(UITheme.HighContrast.IsAccessibilityTheme());
            Assert.False(UITheme.Light.IsAccessibilityTheme());
            Assert.False(UITheme.Dark.IsAccessibilityTheme());
            Assert.False(UITheme.System.IsAccessibilityTheme());
            Assert.False(UITheme.Sepia.IsAccessibilityTheme());
            Assert.False(UITheme.BlueLight.IsAccessibilityTheme());
        }

        [Theory]
        [MemberData(nameof(GetCssVariablesData))]
        public void GetCssVariables_ShouldReturnCssString(UITheme theme, string expected)
        {
            // Act
            var cssVars = theme.GetCssVariables();

            // Assert
            Assert.Equal(expected, cssVars);
        }

        public static IEnumerable<object[]> GetCssVariablesData()
        {
            yield return new object[] { UITheme.Light, "--bg-color: #ffffff; --text-color: #333333;" };
            yield return new object[] { UITheme.Dark, "--bg-color: #222222; --text-color: #f8f8f8;" };
            yield return new object[] { UITheme.System, "" };
            yield return new object[] { UITheme.Sepia, "--bg-color: #f4ecd8; --text-color: #5b4636;" };
        }

        [Theory]
        [MemberData(nameof(GetIconName))]
        public void GetIconName_ShouldReturnCorrectIconName(UITheme theme, string expected)
        {
            // Act
            var iconName = theme.GetIconName();

            // Assert
            Assert.Equal(expected, iconName);
        }

        public static IEnumerable<object[]> GetIconName()
        {
            yield return new object[] { UITheme.Light, "sun" };
            yield return new object[] { UITheme.Dark, "moon" };
            yield return new object[] { UITheme.System, "desktop" };
            yield return new object[] { UITheme.Sepia, "book" };
            yield return new object[] { UITheme.HighContrast, "adjust" };
            yield return new object[] { UITheme.BlueLight, "eye" };
        }

        [Theory]
        [InlineData(true, "Dark")]
        [InlineData(false, "Light")]
        public void GetThemeFromPreference_ShouldReturnCorrectThemeBasedOnPreference(bool prefersDarkMode, string expectedThemeName)
        {
            // Act
            var theme = UITheme.GetThemeFromPreference(prefersDarkMode);

            // Assert
            Assert.Equal(expectedThemeName, theme.Name);
        }
    }
}