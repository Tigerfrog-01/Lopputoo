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
            LocalizationService.LanguageChanged += OnLanguageChanged;
            ApplyLocalization();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ApplyLocalization();
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

            await DisplayAlert(LocalizationService.Get("Login"), result.Message, LocalizationService.Get("Ok"));
            await Shell.Current.GoToAsync("..");
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private void ApplyLocalization()
        {
            Title = LocalizationService.Get("Login");
            LoginTitleLabel.Text = LocalizationService.Get("Login");
            UsernameEntry.Placeholder = LocalizationService.Get("Username");
            PasswordEntry.Placeholder = LocalizationService.Get("Password");
            LoginButton.Text = LocalizationService.Get("Login");
            BackButton.Text = LocalizationService.Get("Back");
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            ApplyLocalization();
        }
    }
}
