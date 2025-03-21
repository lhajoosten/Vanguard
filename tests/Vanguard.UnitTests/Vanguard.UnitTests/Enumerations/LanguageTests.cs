using System.Globalization;
using Vanguard.Domain.Enumerations;

namespace Vanguard.UnitTests.Enumerations
{
    public class LanguageTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllLanguages()
        {
            // Act
            var allLanguages = Vanguard.Common.Base.Enumeration.GetAll<Language>();

            // Assert
            Assert.Equal(10, allLanguages.Count());
        }

        [Fact]
        public void FromId_ShouldReturnCorrectLanguage()
        {
            // Act
            var language = Vanguard.Common.Base.Enumeration.FromId<Language>(3);

            // Assert
            Assert.Equal(Language.French, language);
        }

        [Fact]
        public void FromName_ShouldReturnCorrectLanguage()
        {
            // Act
            var language = Vanguard.Common.Base.Enumeration.FromName<Language>("German");

            // Assert
            Assert.Equal(Language.German, language);
        }

        [Theory]
        [InlineData(1, "English")]
        [InlineData(2, "Spanish")]
        [InlineData(3, "French")]
        [InlineData(4, "German")]
        [InlineData(5, "Chinese")]
        [InlineData(6, "Japanese")]
        [InlineData(7, "Arabic")]
        [InlineData(8, "Russian")]
        [InlineData(9, "Portuguese")]
        [InlineData(10, "Hindi")]
        public void GetById_ShouldHaveCorrectName(int id, string expectedName)
        {
            // Act
            var language = Vanguard.Common.Base.Enumeration.FromId<Language>(id);

            // Assert
            Assert.Equal(expectedName, language.Name);
        }

        [Theory]
        [InlineData(1, "en-US")]
        [InlineData(2, "es-ES")]
        [InlineData(3, "fr-FR")]
        [InlineData(4, "de-DE")]
        [InlineData(5, "zh-CN")]
        [InlineData(6, "ja-JP")]
        [InlineData(7, "ar-SA")]
        [InlineData(8, "ru-RU")]
        [InlineData(9, "pt-BR")]
        [InlineData(10, "hi-IN")]
        public void CultureCode_ShouldMatchExpected(int id, string expectedCultureCode)
        {
            // Act
            var language = Vanguard.Common.Base.Enumeration.FromId<Language>(id);

            // Assert
            Assert.Equal(expectedCultureCode, language.CultureCode);
        }

        [Theory]
        [InlineData(1, "English")]
        [InlineData(2, "Español")]
        [InlineData(3, "Français")]
        [InlineData(4, "Deutsch")]
        [InlineData(5, "中文")]
        [InlineData(6, "日本語")]
        [InlineData(7, "العربية")]
        [InlineData(8, "Русский")]
        [InlineData(9, "Português")]
        [InlineData(10, "हिन्दी")]
        public void NativeName_ShouldMatchExpected(int id, string expectedNativeName)
        {
            // Act
            var language = Vanguard.Common.Base.Enumeration.FromId<Language>(id);

            // Assert
            Assert.Equal(expectedNativeName, language.NativeName);
        }

        [Fact]
        public void IsRightToLeft_ShouldBeTrueOnlyForArabic()
        {
            // Assert
            Assert.True(Language.Arabic.IsRightToLeft);
            Assert.False(Language.English.IsRightToLeft);
            Assert.False(Language.Spanish.IsRightToLeft);
            Assert.False(Language.French.IsRightToLeft);
            Assert.False(Language.German.IsRightToLeft);
            Assert.False(Language.Chinese.IsRightToLeft);
            Assert.False(Language.Japanese.IsRightToLeft);
            Assert.False(Language.Russian.IsRightToLeft);
            Assert.False(Language.Portuguese.IsRightToLeft);
            Assert.False(Language.Hindi.IsRightToLeft);
        }

        [Fact]
        public void Culture_ShouldReturnCorrectCultureInfo()
        {
            // Act
            var englishCulture = Language.English.Culture;
            var japaneseCulture = Language.Japanese.Culture;

            // Assert
            Assert.Equal("en-US", englishCulture.Name);
            Assert.Equal("ja-JP", japaneseCulture.Name);
            Assert.IsType<CultureInfo>(englishCulture);
        }

        [Fact]
        public void FromCultureCode_ShouldReturnEnglishWhenNotFound()
        {
            // Act
            var language = Language.FromCultureCode("xx-XX");

            // Assert
            Assert.Equal(Language.English, language);
        }

        [Fact]
        public void GetSupportedLanguages_ShouldReturnAllLanguages()
        {
            // Act
            var supportedLanguages = Language.GetSupportedLanguages();

            // Assert
            Assert.Equal(10, supportedLanguages.Count());
            Assert.Contains(Language.English, supportedLanguages);
            Assert.Contains(Language.Spanish, supportedLanguages);
            Assert.Contains(Language.French, supportedLanguages);
        }

        [Fact]
        public void SupportsLocalization_ShouldReturnTrue()
        {
            // Act & Assert
            Assert.True(Language.English.SupportsLocalization());
            Assert.True(Language.Spanish.SupportsLocalization());
        }
    }
}