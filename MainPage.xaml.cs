namespace Lopputoo
{
    public partial class MainPage : ContentPage
    {
        private const string CurrentUsernamePreferenceKey = "CurrentUsername";

        public MainPage()
        {
            InitializeComponent();
            UpdateCurrentUserText();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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

            var shouldLogOut = await DisplayAlert("Log out", $"Log out {username}?", "Log out", "Cancel");

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
            CurrentUserLabel.Text = string.IsNullOrWhiteSpace(username) ? "Not logged in" : $"User: {username}";
        }
    }
}
