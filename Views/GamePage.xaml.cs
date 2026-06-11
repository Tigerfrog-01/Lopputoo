namespace Lopputoo.Views
{
    public partial class GamePage : ContentPage
    {
        public GamePage()
        {
            InitializeComponent();
        }

        private void OnCactusDragStarting(object? sender, DragStartingEventArgs e)
        {
            e.Data.Properties["Plant"] = "Cactus";
        }

        private void OnPlantSquareDrop(object? sender, DropEventArgs e)
        {
            if (!e.Data.Properties.ContainsKey("Plant"))
            {
                return;
            }

            PlantSquareLabel.IsVisible = false;
            PlacedCactusImage.IsVisible = true;
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
