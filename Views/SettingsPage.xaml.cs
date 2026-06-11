using Lopputoo.Services;

namespace Lopputoo.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            LocalizationService.LanguageChanged += OnLanguageChanged;
            ApplyLocalization();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ApplyLocalization();
        }

        private void OnLightModeClicked(object? sender, EventArgs e)
        {
            ThemeService.SetTheme(AppTheme.Light);
            ApplyLocalization();
        }

        private void OnDarkModeClicked(object? sender, EventArgs e)
        {
            ThemeService.SetTheme(AppTheme.Dark);
            ApplyLocalization();
        }

        private void OnEnglishClicked(object? sender, EventArgs e)
        {
            LocalizationService.SetLanguage("en");
        }

        private void OnEstonianClicked(object? sender, EventArgs e)
        {
            LocalizationService.SetLanguage("et");
        }

        private void OnRussianClicked(object? sender, EventArgs e)
        {
            LocalizationService.SetLanguage("ru");
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private void ApplyLocalization()
        {
            Title = LocalizationService.Get("Settings");
            SettingsTitleLabel.Text = LocalizationService.Get("Settings");
            LightModeButton.Text = LocalizationService.Get("LightMode");
            DarkModeButton.Text = LocalizationService.Get("DarkMode");
            LanguageTitleLabel.Text = LocalizationService.Get("Language");
            EnglishButton.Text = LocalizationService.Get("English");
            EstonianButton.Text = LocalizationService.Get("Estonian");
            RussianButton.Text = LocalizationService.Get("Russian");
            BackButton.Text = LocalizationService.Get("Back");

            var themeName = ThemeService.CurrentTheme == AppTheme.Dark
                ? LocalizationService.Get("DarkMode")
                : LocalizationService.Get("LightMode");

            CurrentThemeLabel.Text = LocalizationService.Format("CurrentThemeFormat", themeName);
            CurrentLanguageLabel.Text = LocalizationService.Format("CurrentLanguageFormat", LocalizationService.CurrentLanguageName);
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            ApplyLocalization();
        }
    }
}
