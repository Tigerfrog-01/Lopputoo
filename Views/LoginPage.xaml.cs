using Lopputoo.Services;

namespace Lopputoo.Views
{
    public partial class LoginPage : ContentPage
    {
        private const string CurrentUsernamePreferenceKey = "CurrentUsername";
        private readonly UserDatabaseService userDatabaseService = new();

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            var result = await userDatabaseService.LoginAsync(
                UsernameEntry.Text ?? string.Empty,
                PasswordEntry.Text ?? string.Empty);

            LoginMessageLabel.Text = result.Message;

            if (!result.Success || result.User is null)
            {
                return;
            }

            Preferences.Set(CurrentUsernamePreferenceKey, result.User.Username);
            PasswordEntry.Text = string.Empty;

            await DisplayAlert("Login", result.Message, "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
