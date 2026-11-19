# WPFSnakeTest

A WPF application targeting .NET 11. On Windows, run it with:

```sh
dotnet run --project Snake/Snake.csproj
```

Enter a username and select Continue to identify the current player. A username
must contain at least one non-whitespace character; surrounding whitespace is
removed. The player can change the username from the next screen. This is the
identity screen. Select Play to start a game. Use the arrow keys to steer and
eat the gold food to grow. Crossing a wall wraps the snake to the opposite
side; hitting its body ends the game and filling the board wins. Use Restart
to play again or Back to player to return to the identity screen.

Run the username and game checks on any platform with:

```sh
dotnet run --project Snake.Tests/Snake.Tests.csproj
```