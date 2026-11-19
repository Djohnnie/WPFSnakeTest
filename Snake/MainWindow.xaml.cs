using System.Windows;

namespace Snake;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
}
