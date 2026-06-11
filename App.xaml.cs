using Lopputoo.Services;

namespace Lopputoo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LocalizationService.ApplySavedLanguage();
            ThemeService.ApplySavedTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
