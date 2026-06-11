using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Layouts;

namespace Lopputoo.Views
{
    public partial class GamePage : ContentPage
    {
        private const double MaxHealth = 100;
        private const double BallDamage = 25;
        private const double FishDamage = 25;
        private const double BallSize = 14;
        private const double FishWidth = 90;
        private const double BallSpeed = 6;
        private const double FishSpeed = 2;

        private readonly Dictionary<int, IDispatcherTimer> shootingTimers = new();
        private readonly Dictionary<int, double> cactusHealthByLane = new();
        private bool isFishFingerMoving;
        private bool isFishFingerActive;
        private bool isGameOver;
        private double fishFingerHealth;
        private double baseHealth = MaxHealth;

        public GamePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (isGameOver)
            {
                return;
            }

            isFishFingerMoving = true;
            _ = MoveFishFingerAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isFishFingerMoving = false;
            isFishFingerActive = false;
        }

        private void OnCactusDragStarting(object? sender, DragStartingEventArgs e)
        {
            e.Data.Properties["Plant"] = "Cactus";
        }

        private void OnPlantSquareDrop(object? sender, DropEventArgs e)
        {
            if (isGameOver)
            {
                return;
            }

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
            if (cactusHealthByLane.TryGetValue(laneNumber, out var currentHealth) && currentHealth > 0)
            {
                return;
            }

            cactusHealthByLane[laneNumber] = MaxHealth;
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
            if (isGameOver)
            {
                return;
            }

            if (shotArea.Width <= 0)
            {
                return;
            }

            var ball = new BoxView
            {
                Color = Color.FromArgb("#F2C94C"),
                WidthRequest = BallSize,
                HeightRequest = BallSize,
                CornerRadius = new CornerRadius(7),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            AbsoluteLayout.SetLayoutBounds(ball, new Rect(0, 0.5, BallSize, BallSize));
            AbsoluteLayout.SetLayoutFlags(ball, AbsoluteLayoutFlags.PositionProportional);
            shotArea.Children.Add(ball);

            var targetX = Math.Max(0, shotArea.Width - BallSize);
            var ballX = 0d;

            while (!isGameOver && ballX < targetX && ball.Parent is not null)
            {
                ballX = Math.Min(targetX, ballX + BallSpeed);
                ball.TranslationX = ballX;

                if (shotArea == Lane1ShotArea && BallHitsFishFinger(ballX))
                {
                    DamageFishFinger(BallDamage);
                    break;
                }

                await Task.Delay(16);
            }

            shotArea.Children.Remove(ball);
        }

        private async Task MoveFishFingerAsync()
        {
            await Task.Delay(15000);

            while (isFishFingerMoving && !isGameOver)
            {
                if (Lane1ShotArea.Width <= 0)
                {
                    await Task.Delay(100);
                    continue;
                }

                fishFingerHealth = MaxHealth;
                FishFingerHealthBar.Progress = 1;
                FishFingerEnemy.TranslationX = 0;
                FishFingerEnemy.IsVisible = true;
                isFishFingerActive = true;

                while (isFishFingerMoving && isFishFingerActive && !isGameOver)
                {
                    FishFingerEnemy.TranslationX -= FishSpeed;

                    if (GetFishFingerLeft() <= 0)
                    {
                        if (HasLivingCactus(1))
                        {
                            DamageCactus(1, FishDamage);
                        }
                        else
                        {
                            DamageBase(FishDamage);
                        }

                        isFishFingerActive = false;
                    }

                    await Task.Delay(16);
                }

                FishFingerEnemy.IsVisible = false;
                FishFingerEnemy.TranslationX = 0;

                await Task.Delay(800);
            }
        }

        private bool BallHitsFishFinger(double ballX)
        {
            if (!isFishFingerActive || !FishFingerEnemy.IsVisible)
            {
                return false;
            }

            var ballRight = ballX + BallSize;
            var fishLeft = GetFishFingerLeft();
            var fishRight = fishLeft + FishWidth;

            return ballRight >= fishLeft && ballX <= fishRight;
        }

        private double GetFishFingerLeft()
        {
            return Lane1ShotArea.Width - FishWidth + FishFingerEnemy.TranslationX;
        }

        private void DamageFishFinger(double damage)
        {
            if (isGameOver || !isFishFingerActive)
            {
                return;
            }

            fishFingerHealth = Math.Max(0, fishFingerHealth - damage);
            FishFingerHealthBar.Progress = fishFingerHealth / MaxHealth;

            if (fishFingerHealth > 0)
            {
                return;
            }

            isFishFingerActive = false;
            FishFingerEnemy.IsVisible = false;
        }

        private void DamageCactus(int laneNumber, double damage)
        {
            if (isGameOver)
            {
                return;
            }

            if (!cactusHealthByLane.TryGetValue(laneNumber, out var cactusHealth) || cactusHealth <= 0)
            {
                return;
            }

            cactusHealth = Math.Max(0, cactusHealth - damage);
            cactusHealthByLane[laneNumber] = cactusHealth;
            UpdateCactusHealthBar(laneNumber, cactusHealth / MaxHealth);

            if (cactusHealth > 0)
            {
                return;
            }

            RemoveCactus(laneNumber);
        }

        private bool HasLivingCactus(int laneNumber)
        {
            return cactusHealthByLane.TryGetValue(laneNumber, out var cactusHealth) && cactusHealth > 0;
        }

        private void UpdateCactusHealthBar(int laneNumber, double progress)
        {
            if (laneNumber == 1)
            {
                CactusHealthBar1.Progress = progress;
            }
            else if (laneNumber == 2)
            {
                CactusHealthBar2.Progress = progress;
            }
            else if (laneNumber == 3)
            {
                CactusHealthBar3.Progress = progress;
            }
        }

        private void RemoveCactus(int laneNumber)
        {
            if (shootingTimers.TryGetValue(laneNumber, out var timer))
            {
                timer.Stop();
                shootingTimers.Remove(laneNumber);
            }

            if (laneNumber == 1)
            {
                PlantSquareLabel1.IsVisible = true;
                CactusHealthBar1.IsVisible = false;
                PlacedCactusImage1.IsVisible = false;
            }
            else if (laneNumber == 2)
            {
                PlantSquareLabel2.IsVisible = true;
                CactusHealthBar2.IsVisible = false;
                PlacedCactusImage2.IsVisible = false;
            }
            else if (laneNumber == 3)
            {
                PlantSquareLabel3.IsVisible = true;
                CactusHealthBar3.IsVisible = false;
                PlacedCactusImage3.IsVisible = false;
            }
        }

        private void DamageBase(double damage)
        {
            if (isGameOver)
            {
                return;
            }

            baseHealth = Math.Max(0, baseHealth - damage);
            BaseHealthBar.Progress = baseHealth / MaxHealth;

            if (baseHealth > 0)
            {
                return;
            }

            EndGame();
        }

        private void EndGame()
        {
            isGameOver = true;
            isFishFingerMoving = false;
            isFishFingerActive = false;
            FishFingerEnemy.IsVisible = false;
            GameOverLabel.IsVisible = true;

            foreach (var timer in shootingTimers.Values)
            {
                timer.Stop();
            }

            shootingTimers.Clear();
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
