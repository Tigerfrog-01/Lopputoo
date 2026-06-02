namespace Lopputoo.Services
{
    public static class ThemeService
    {
        private const string ThemePreferenceKey = "SelectedTheme";
        private const string LightThemeValue = "Light";
        private const string DarkThemeValue = "Dark";

        public static AppTheme CurrentTheme
        {
            get
            {
                var savedTheme = Preferences.Get(ThemePreferenceKey, LightThemeValue);
                return savedTheme == DarkThemeValue ? AppTheme.Dark : AppTheme.Light;
            }
        }

        public static string CurrentThemeName => CurrentTheme == AppTheme.Dark ? DarkThemeValue : LightThemeValue;

        public static void ApplySavedTheme()
        {
            ApplyTheme(CurrentTheme, saveTheme: false);
        }

        public static void SetTheme(AppTheme theme)
        {
            ApplyTheme(theme, saveTheme: true);
        }

        private static void ApplyTheme(AppTheme theme, bool saveTheme)
        {
            if (Application.Current is not null)
            {
                Application.Current.UserAppTheme = theme;
            }

            if (saveTheme)
            {
                var themeValue = theme == AppTheme.Dark ? DarkThemeValue : LightThemeValue;
                Preferences.Set(ThemePreferenceKey, themeValue);
            }
        }
    }
}
