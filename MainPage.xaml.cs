namespace Lopputoo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }

        private async void OnLoginClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.LoginPage));
        }

        private async void OnSettingsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.SettingsPage));
        }
    }
}
