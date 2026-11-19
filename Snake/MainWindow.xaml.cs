using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer gameTimer = new() { Interval = TimeSpan.FromMilliseconds(180) };
    private SnakeGame? game;

    public MainWindow()
    {
        InitializeComponent();
        gameTimer.Tick += GameTimer_Tick;
        Closed += (_, _) => gameTimer.Stop();
    }

    public Player? CurrentPlayer { get; private set; }

    private void Continue_Click(object sender, RoutedEventArgs e)
    {
        if (!Player.TryCreate(UsernameInput.Text, out Player? player))
        {
            ValidationMessage.Text = "Enter a username.";
            UsernameInput.Focus();
            return;
        }

        CurrentPlayer = player;
        PlayerName.Text = player.Username;
        ValidationMessage.Text = string.Empty;
        IdentityPanel.Visibility = Visibility.Collapsed;
        PlayerPanel.Visibility = Visibility.Visible;
        ChangeUsernameButton.Focus();
    }

    private void ChangeUsername_Click(object sender, RoutedEventArgs e)
    {
        PlayerPanel.Visibility = Visibility.Collapsed;
        IdentityPanel.Visibility = Visibility.Visible;
        UsernameInput.Focus();
        UsernameInput.SelectAll();
    }

    private void Play_Click(object sender, RoutedEventArgs e)
    {
        gameTimer.Stop();
        game = new SnakeGame();
        PlayerPanel.Visibility = Visibility.Collapsed;
        GamePanel.Visibility = Visibility.Visible;
        RenderGame();
        gameTimer.Start();
        RestartButton.Focus();
    }

    private void BackToPlayer_Click(object sender, RoutedEventArgs e)
    {
        gameTimer.Stop();
        game = null;
        GamePanel.Visibility = Visibility.Collapsed;
        PlayerPanel.Visibility = Visibility.Visible;
        ChangeUsernameButton.Focus();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (game is null || game.Status != GameStatus.Playing)
        {
            return;
        }

        Direction? direction = e.Key switch
        {
            Key.Up => Direction.Up,
            Key.Down => Direction.Down,
            Key.Left => Direction.Left,
            Key.Right => Direction.Right,
            _ => null
        };
        if (direction is Direction next)
        {
            game.Turn(next);
            e.Handled = true;
        }
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        if (game is null)
        {
            return;
        }

        game.Step();
        RenderGame();
        if (game.Status != GameStatus.Playing)
        {
            gameTimer.Stop();
        }
    }

    private void RenderGame()
    {
        if (game is null)
        {
            return;
        }

        GameBoard.Children.Clear();
        foreach (Cell segment in game.Segments)
        {
            DrawCell(segment, Brushes.LightGreen);
        }
        if (game.Food is Cell food)
        {
            DrawCell(food, Brushes.Gold);
        }

        GameMessage.Text = game.Status switch
        {
            GameStatus.Playing => $"Length: {game.Segments.Count}",
            GameStatus.Lost => $"Game over. Length: {game.Segments.Count}",
            GameStatus.Won => $"You won! Length: {game.Segments.Count}",
            _ => throw new InvalidOperationException("Invalid game status.")
        };
    }

    private void DrawCell(Cell cell, Brush color)
    {
        const int cellSize = 20;
        Rectangle shape = new() { Width = cellSize - 1, Height = cellSize - 1, Fill = color };
        Canvas.SetLeft(shape, cell.X * cellSize);
        Canvas.SetTop(shape, cell.Y * cellSize);
        GameBoard.Children.Add(shape);
    }
}
