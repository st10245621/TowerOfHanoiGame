using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TowerOfHanoiGame
{
    // Represents the main window of the Tower of Hanoi game, managing the UI components and game logic.
    public partial class MainWindow : Window
    {
        private int diskCount;
        private List<Stack<Rectangle>> rods = new List<Stack<Rectangle>>();
        private const double BaseLineHeight = 5;  
        private readonly List<Color> diskColors = new List<Color> 
        { 
            Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, 
            Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown, 
            Colors.Cyan, Colors.Magenta 
        };
        private CancellationTokenSource? cancellationTokenSource;
        private int moveDelay = 500;  // Default speed

        // Initializes the main window and sets up the rods.
        public MainWindow()
        {
            InitializeComponent();
            InitializeRods();
        }

        // Initializes the 3 rods as stacks to hold the disks.
        private void InitializeRods()
        {
            rods.Add(new Stack<Rectangle>());
            rods.Add(new Stack<Rectangle>());
            rods.Add(new Stack<Rectangle>());
        }

        // Handles the Start button click event to start a new game.
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();

            if (int.TryParse(DiskInput.Text, out diskCount) && diskCount >= 4 && diskCount <= 10)
            {
                ResetGame();
                CreateDisks(diskCount);
                Task.Run(() => TowerOfHanoi(diskCount, 0, 2, 1, cancellationTokenSource.Token));
            }
            else
            {
                MessageBox.Show("Please enter a valid number of disks (4-10).");
            }
        }

        // Clears all rods and resets the game state.
        private void ResetGame()
        {
            RodA.Children.Clear();
            RodB.Children.Clear();
            RodC.Children.Clear();

            foreach (var rod in rods) 
                rod.Clear();
        }

        // Creates the disks and adds them to the first rod (RodA).
        private void CreateDisks(int count)
        {
            double rodHeight = RodA.ActualHeight - BaseLineHeight;
            double diskHeight = Math.Min(rodHeight / count, 40);
            double minWidth = 30;
            double maxWidth = RodA.ActualWidth - 20;

            for (int i = count; i >= 1; i--)
            {
                double diskWidth = minWidth + (maxWidth - minWidth) * (i - 1) / (count - 1);

                var disk = new Rectangle
                {
                    Width = diskWidth,
                    Height = diskHeight,
                    Fill = new SolidColorBrush(diskColors[i - 1]),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                rods[0].Push(disk);
                RodA.Children.Add(disk);

                double topPosition = rodHeight - (count - i + 1) * diskHeight + BaseLineHeight;
                Canvas.SetLeft(disk, (RodA.ActualWidth - disk.Width) / 2);
                Canvas.SetTop(disk, topPosition);
            }
        }

        // Solves the Tower of Hanoi puzzle using recursion.
        private async Task TowerOfHanoi(int n, int from, int to, int aux, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (n == 0) 
                return;

            await TowerOfHanoi(n - 1, from, aux, to, cancellationToken);
            await MoveDisk(from, to, cancellationToken);
            await TowerOfHanoi(n - 1, aux, to, from, cancellationToken);
        }

        // Animates the movement of a disk from one rod to another.
        private async Task MoveDisk(int from, int to, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var disk = rods[from].Pop();
            rods[to].Push(disk);

            await Dispatcher.InvokeAsync(() =>
            {
                Canvas fromRod = GetRod(from);
                Canvas toRod = GetRod(to);

                fromRod.Children.Remove(disk);
                toRod.Children.Add(disk);

                double rodHeight = toRod.ActualHeight - BaseLineHeight;
                double topPosition = rodHeight - (toRod.Children.Count) * disk.Height + BaseLineHeight;

                Canvas.SetLeft(disk, (toRod.ActualWidth - disk.Width) / 2);
                Canvas.SetTop(disk, topPosition);
            });

            await Task.Delay(moveDelay);
        }

        // Returns the Canvas corresponding to the specified rod index.
        private Canvas GetRod(int rodIndex)
        {
            return rodIndex switch
            {
                0 => RodA,
                1 => RodB,
                2 => RodC,
                _ => throw new ArgumentOutOfRangeException(nameof(rodIndex), "Invalid rod index.")
            };
        }

        // Handles text input changes in the disk input field.
        private void DiskInput_TextChanged(object sender, TextChangedEventArgs e) { }

        // Handles the Pause button click event to cancel the ongoing task.
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }

        // Adjusts the speed of the disk movement based on the slider value.
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            moveDelay = (int)e.NewValue;
            SpeedLabel.Content = $"Speed: {moveDelay} ms";
        }
    }
}
