using System.Globalization;
using System.Resources;

namespace Lopputoo.Services
{
    public static class LocalizationService
    {
        private const string LanguagePreferenceKey = "SelectedLanguage";
        private const string DefaultLanguage = "en";
        private static readonly ResourceManager ResourceManager = new("Lopputoo.Resources.Localization.AppResources", typeof(LocalizationService).Assembly);

        public static event EventHandler? LanguageChanged;

        public static string CurrentLanguageCode => Preferences.Get(LanguagePreferenceKey, DefaultLanguage);

        public static string CurrentLanguageName => CurrentLanguageCode switch
        {
            "et" => Get("Estonian"),
            "ru" => Get("Russian"),
            _ => Get("English")
        };

        public static void ApplySavedLanguage()
        {
            ApplyCulture(CurrentLanguageCode);
        }

        public static void SetLanguage(string languageCode)
        {
            Preferences.Set(LanguagePreferenceKey, languageCode);
            ApplyCulture(languageCode);
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        public static string Get(string key)
        {
            return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        }

        public static string Format(string key, params object[] values)
        {
            return string.Format(CultureInfo.CurrentUICulture, Get(key), values);
        }

        private static void ApplyCulture(string languageCode)
        {
            var culture = new CultureInfo(languageCode);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
