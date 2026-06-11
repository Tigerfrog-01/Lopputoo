using Lopputoo.Services;

namespace Lopputoo.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly UserDatabaseService userDatabaseService = new();

        public RegisterPage()
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

            await DisplayAlert(LocalizationService.Get("Register"), result.Message, LocalizationService.Get("Ok"));
            await Shell.Current.GoToAsync("..");
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private void ApplyLocalization()
        {
            Title = LocalizationService.Get("Register");
            RegisterTitleLabel.Text = LocalizationService.Get("Register");
            UsernameEntry.Placeholder = LocalizationService.Get("Username");
            PasswordEntry.Placeholder = LocalizationService.Get("Password");
            ConfirmPasswordEntry.Placeholder = LocalizationService.Get("ConfirmPassword");
            CreateAccountButton.Text = LocalizationService.Get("CreateAccount");
            BackButton.Text = LocalizationService.Get("Back");
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            ApplyLocalization();
        }
    }
}
