using Lopputoo.Services;

namespace Lopputoo.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            UpdateCurrentThemeText();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateCurrentThemeText();
        }

        private void OnLightModeClicked(object? sender, EventArgs e)
        {
            ThemeService.SetTheme(AppTheme.Light);
            UpdateCurrentThemeText();
        }

        private void OnDarkModeClicked(object? sender, EventArgs e)
        {
            ThemeService.SetTheme(AppTheme.Dark);
            UpdateCurrentThemeText();
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private void UpdateCurrentThemeText()
        {
            CurrentThemeLabel.Text = $"Current theme: {ThemeService.CurrentThemeName}";
        }
    }
}
