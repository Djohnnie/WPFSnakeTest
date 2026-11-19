using System.Diagnostics.CodeAnalysis;

namespace Snake;

public sealed class Player
{
    private Player(string username)
    {
        Username = username;
    }

    public string Username { get; }

    public static bool TryCreate(string? username, [NotNullWhen(true)] out Player? player)
    {
        string? trimmedUsername = username?.Trim();
        if (string.IsNullOrEmpty(trimmedUsername))
        {
            player = null;
            return false;
        }

        player = new Player(trimmedUsername);
        return true;
    }
}
