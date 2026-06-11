using Lopputoo.Services;

namespace Lopputoo
{
    public partial class MainPage : ContentPage
    {
        private const string CurrentUsernamePreferenceKey = "CurrentUsername";

        public MainPage()
        {
            InitializeComponent();
            LocalizationService.LanguageChanged += OnLanguageChanged;
            ApplyLocalization();
            UpdateCurrentUserText();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ApplyLocalization();
            UpdateCurrentUserText();
        }

        private async void OnRegisterClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.LoginPage));
        }

        private async void OnPlayClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.GamePage));
        }

        private async void OnSettingsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.SettingsPage));
        }

        private async void OnCurrentUserTapped(object? sender, TappedEventArgs e)
        {
            var username = Preferences.Get(CurrentUsernamePreferenceKey, string.Empty);

            if (string.IsNullOrWhiteSpace(username))
            {
                await Shell.Current.GoToAsync(nameof(Views.LoginPage));
                return;
            }

            var shouldLogOut = await DisplayAlert(
                LocalizationService.Get("LogOutTitle"),
                LocalizationService.Format("LogOutMessageFormat", username),
                LocalizationService.Get("LogOut"),
                LocalizationService.Get("Cancel"));

            if (!shouldLogOut)
            {
                return;
            }

            Preferences.Remove(CurrentUsernamePreferenceKey);
            UpdateCurrentUserText();
        }

        private void UpdateCurrentUserText()
        {
            var username = Preferences.Get(CurrentUsernamePreferenceKey, string.Empty);
            CurrentUserLabel.Text = string.IsNullOrWhiteSpace(username)
                ? LocalizationService.Get("NotLoggedIn")
                : LocalizationService.Format("UserFormat", username);
        }

        private void ApplyLocalization()
        {
            RegisterButton.Text = LocalizationService.Get("Register");
            LoginButton.Text = LocalizationService.Get("Login");
            GardenDefenderLabel.Text = LocalizationService.Get("GardenDefender");
            PrototypeLabel.Text = LocalizationService.Get("Prototype");
            PlayButton.Text = LocalizationService.Get("Play");
            SettingsButton.Text = LocalizationService.Get("Settings");
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            ApplyLocalization();
            UpdateCurrentUserText();
        }
    }
}
