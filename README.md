# WPFSnakeTest

A WPF application targeting .NET 11. On Windows, run it with:

```sh
dotnet run --project Snake/Snake.csproj
```

Enter a username and select Continue to identify the current player. A username
must contain at least one non-whitespace character; surrounding whitespace is
removed. The player can change the username from the next screen. This is the
identity screen for the future game; gameplay is not implemented yet.

Run the username checks on any platform with:

```sh
dotnet run --project Snake.Tests/Snake.Tests.csproj
```