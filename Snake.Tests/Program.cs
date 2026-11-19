using Snake;

foreach (string? invalidUsername in new[] { null, "", " \t\r\n" })
{
    if (Player.TryCreate(invalidUsername, out Player? invalidPlayer) || invalidPlayer is not null)
    {
        throw new Exception("A blank username must not create a player.");
    }
}

if (!Player.TryCreate("  Ada Lovelace  ", out Player? player) ||
    player?.Username != "Ada Lovelace")
{
    throw new Exception("A player must retain the trimmed username.");
}

Console.WriteLine("Player username checks passed.");
