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
    public partial class MainWindow : Window
    {
        private int diskCount;
        private List<Stack<Rectangle>> rods = new List<Stack<Rectangle>>();
        private const double BaseLineHeight = 5;  // Height of the base line
        private readonly List<Color> diskColors = new List<Color> { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown, Colors.Cyan, Colors.Magenta };
        private CancellationTokenSource? cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            InitializeRods();
        }

        // Initialize the 3 rods as stacks to hold the disks
        private void InitializeRods()
        {
            rods.Add(new Stack<Rectangle>());
            rods.Add(new Stack<Rectangle>());
            rods.Add(new Stack<Rectangle>());
        }

        // Handle the Start button click event
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource(); // Create new token source for each start

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


        // Clear all rods and reset the game state
        private void ResetGame()
        {
            RodA.Children.Clear();
            RodB.Children.Clear();
            RodC.Children.Clear();
            foreach (var rod in rods) rod.Clear();
        }

        // Create the disks and add them to the first rod (RodA)
        private void CreateDisks(int count)
        {
            double rodHeight = RodA.ActualHeight - BaseLineHeight;  // Adjust rod height to consider the base line
            double diskHeight = Math.Min(rodHeight / count, 40);  // Limit max disk height to 40
            double minWidth = 30;  // Minimum width of the smallest disk
            double maxWidth = RodA.ActualWidth - 20;  // Maximum width of the largest disk

            for (int i = count; i >= 1; i--)
            {
                double diskWidth = minWidth + (maxWidth - minWidth) * (i - 1) / (count - 1);

                var disk = new Rectangle
                {
                    Width = diskWidth,
                    Height = diskHeight,
                    Fill = new SolidColorBrush(diskColors[i - 1]),  // Set color from predefined list
                    Stroke = new SolidColorBrush(Colors.Black),  // Black outline
                    StrokeThickness = 2,  // Thickness of the outline
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                rods[0].Push(disk);
                RodA.Children.Add(disk);

                // Calculate position to stack from the bottom and respect the base line height
                double topPosition = rodHeight - (count - i + 1) * diskHeight + BaseLineHeight;
                Canvas.SetLeft(disk, (RodA.ActualWidth - disk.Width) / 2);  // Center horizontally
                Canvas.SetTop(disk, topPosition);  // Align from the bottom
            }
        }

        // Recursive method to solve the Tower of Hanoi
        private async Task TowerOfHanoi(int n, int from, int to, int aux, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (n == 0) return;

            await TowerOfHanoi(n - 1, from, aux, to, cancellationToken);
            await MoveDisk(from, to, cancellationToken);
            await TowerOfHanoi(n - 1, aux, to, from, cancellationToken);
        }

        // Animate the movement of a disk from one rod to another
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

                // Calculate the new top position, respecting the base line height
                double rodHeight = toRod.ActualHeight - BaseLineHeight;
                double topPosition = rodHeight - (toRod.Children.Count) * disk.Height + BaseLineHeight;

                Canvas.SetLeft(disk, (toRod.ActualWidth - disk.Width) / 2);  // Center horizontally
                Canvas.SetTop(disk, topPosition);  // Align from the bottom
            });

            await Task.Delay(20);  // Pause for animation effect
        }


        // Get the Canvas corresponding to a rod index
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

        private void DiskInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();  // Cancel the ongoing recursive task
        }

    }
}