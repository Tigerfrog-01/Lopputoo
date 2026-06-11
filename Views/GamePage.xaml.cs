using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Layouts;

namespace Lopputoo.Views
{
    public partial class GamePage : ContentPage
    {
        private readonly Dictionary<int, IDispatcherTimer> shootingTimers = new();
        private bool isFishFingerMoving;

        public GamePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            isFishFingerMoving = true;
            _ = MoveFishFingerAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isFishFingerMoving = false;
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

            if (sender == PlantSquare1)
            {
                PlaceCactus(1, PlantSquareLabel1, CactusHealthBar1, PlacedCactusImage1, Lane1ShotArea);
            }
            else if (sender == PlantSquare2)
            {
                PlaceCactus(2, PlantSquareLabel2, CactusHealthBar2, PlacedCactusImage2, Lane2ShotArea);
            }
            else if (sender == PlantSquare3)
            {
                PlaceCactus(3, PlantSquareLabel3, CactusHealthBar3, PlacedCactusImage3, Lane3ShotArea);
            }
        }

        private void PlaceCactus(int laneNumber, Label squareLabel, ProgressBar healthBar, Image cactusImage, AbsoluteLayout shotArea)
        {
            squareLabel.IsVisible = false;
            healthBar.IsVisible = true;
            healthBar.Progress = 1;
            cactusImage.IsVisible = true;

            if (shootingTimers.ContainsKey(laneNumber))
            {
                return;
            }

            var timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1200);
            timer.Tick += async (_, _) => await ShootBallAsync(shotArea);
            shootingTimers[laneNumber] = timer;
            timer.Start();

            _ = ShootBallAsync(shotArea);
        }

        private async Task ShootBallAsync(AbsoluteLayout shotArea)
        {
            if (shotArea.Width <= 0)
            {
                return;
            }

            var ball = new BoxView
            {
                Color = Color.FromArgb("#F2C94C"),
                WidthRequest = 14,
                HeightRequest = 14,
                CornerRadius = new CornerRadius(7),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            AbsoluteLayout.SetLayoutBounds(ball, new Rect(0, 0.5, 14, 14));
            AbsoluteLayout.SetLayoutFlags(ball, AbsoluteLayoutFlags.PositionProportional);
            shotArea.Children.Add(ball);

            await ball.TranslateTo(Math.Max(0, shotArea.Width - 18), 0, 900, Easing.Linear);

            shotArea.Children.Remove(ball);
        }

        private async Task MoveFishFingerAsync()
        {
            while (isFishFingerMoving)
            {
                if (Lane1ShotArea.Width <= 0)
                {
                    await Task.Delay(100);
                    continue;
                }

                FishFingerHealthBar.Progress = 1;
                FishFingerEnemy.TranslationX = 0;
                FishFingerEnemy.IsVisible = true;

                await FishFingerEnemy.TranslateTo(-Lane1ShotArea.Width, 0, 4500, Easing.Linear);

                FishFingerEnemy.IsVisible = false;
                FishFingerEnemy.TranslationX = 0;

                await Task.Delay(800);
            }
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
