using Lopputoo.Services;

namespace Lopputoo.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly UserDatabaseService userDatabaseService = new();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object? sender, EventArgs e)
        {
            var result = await userDatabaseService.RegisterAsync(
                UsernameEntry.Text ?? string.Empty,
                PasswordEntry.Text ?? string.Empty,
                ConfirmPasswordEntry.Text ?? string.Empty);

            RegisterMessageLabel.Text = result.Message;

            if (!result.Success)
            {
                return;
            }

            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;

            await DisplayAlert("Register", result.Message, "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
