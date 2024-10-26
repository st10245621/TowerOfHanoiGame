# 🏯 Tower of Hanoi Game

A **WPF-based Tower of Hanoi** implementation that plays the game automatically with animated moves. Users can configure the number of disks and adjust the speed of the animation. This project demonstrates the recursive solution to the Tower of Hanoi puzzle and offers a **play/pause** feature to control the game’s progress.

---

## 🎥 Towers Of Hanoi Game Demo

https://github.com/user-attachments/assets/5626d92b-27f3-4b89-aede-b4d206a26022


---
## 📂 Project Structure

Here’s the structure of the **Tower of Hanoi Game** project based on your setup:

```
TowerOfHanoiGame/
│
├── .gitignore               # Files to ignore in Git
├── App.xaml                 # Application definition for WPF
├── App.xaml.cs              # App entry logic (auto-generated by WPF)
├── LICENSE                  # License for the project
├── MainWindow.xaml          # Main UI layout (rods, controls)
├── MainWindow.xaml.cs       # Code-behind logic for MainWindow.xaml
└── README.md                # Documentation (you are here)
```

---

## ✨ Features

- **Tower of Hanoi Solver**: Automatically solves the Tower of Hanoi puzzle with animated moves.
- **Configurable Disk Count**: Users can configure the number of disks between **4 and 10**.
- **Adjustable Speed**: Use the slider to adjust the animation speed (20ms to 1000ms delay).
- **Play and Pause Controls**: Start or pause the game at any point.
- **Smooth Animations**: Disk movements are animated for a better user experience.

---

## 🛠 Requirements

- **Operating System**: Windows 10 or later  
- **Development Environment**: Visual Studio 2019+  
- **Framework**: .NET Core 3.1+ or .NET 5/6  

---

## 🚀 Setup and Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/tower-of-hanoi-game.git
   cd tower-of-hanoi-game
   ```

2. **Open the Solution**:
   - Open `TowerOfHanoiGame.sln` in **Visual Studio**.

3. **Build the Project**:
   - Press `Ctrl+Shift+B` to build the solution.

4. **Run the Application**:
   - Press `F5` to start the application.

---

## 🎮 How to Play

1. **Set the Disk Count**:  
   Use the input field to enter the number of disks (between **4 and 10**).

2. **Adjust Animation Speed**:  
   Use the slider to set the animation speed. The speed is shown in milliseconds.

3. **Start the Game**:  
   Click the **Start** button to begin the automatic solution.

4. **Pause the Game**:  
   Click **Pause** to stop the game at any point.

5. **Watch the Animation**:  
   Enjoy the smooth animation of disks moving from one rod to another.

---

## 🧑‍💻 Code Walkthrough

### 1. **Main Logic** (Tower of Hanoi Solver)

The recursive function **`TowerOfHanoi`** handles the solution logic by moving disks from the source rod to the target rod using an auxiliary rod.

```csharp
private async Task TowerOfHanoi(int n, int from, int to, int aux, CancellationToken cancellationToken)
{
    if (cancellationToken.IsCancellationRequested) return;
    if (n == 0) return;

    await TowerOfHanoi(n - 1, from, aux, to, cancellationToken);
    await MoveDisk(from, to, cancellationToken);
    await TowerOfHanoi(n - 1, aux, to, from, cancellationToken);
}
```

### 2. **Animating Disk Movement**

Each disk move is animated using **WPF Canvas**. The disk is removed from the source rod and added to the target rod with smooth transitions.

```csharp
private async Task MoveDisk(int from, int to, CancellationToken cancellationToken)
{
    if (cancellationToken.IsCancellationRequested) return;

    var disk = rods[from].Pop();
    rods[to].Push(disk);

    await Dispatcher.InvokeAsync(() =>
    {
        Canvas fromRod = GetRod(from);
        Canvas toRod = GetRod(to);

        fromRod.Children.Remove(disk);
        toRod.Children.Add(disk);

        double rodHeight = toRod.ActualHeight - BaseLineHeight;
        double topPosition = rodHeight - toRod.Children.Count * disk.Height + BaseLineHeight;

        Canvas.SetLeft(disk, (toRod.ActualWidth - disk.Width) / 2);
        Canvas.SetTop(disk, topPosition);
    });

    await Task.Delay(moveDelay);
}
```

### 3. **Adjustable Speed and Pause Functionality**

- **Speed Control**: The `Slider` lets users adjust the delay between moves.
- **Pause Feature**: Users can pause the game using a **CancellationTokenSource**.

```csharp
private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
{
    moveDelay = (int)e.NewValue;
    SpeedLabel.Content = $"Speed: {moveDelay} ms";
}

private void PauseButton_Click(object sender, RoutedEventArgs e)
{
    cancellationTokenSource?.Cancel();
}
```

---


## 📜 License

This project is licensed under the **MIT License**. See the `LICENSE` file for more details.

---

## 📧 Contact

For questions or support, please contact **[ggovender77@gmail.com](mailto:ggovender77@gmail.com)**.
