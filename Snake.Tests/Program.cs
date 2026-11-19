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

SnakeGame game = new(new ScriptedRandom(102, 102, 0));
Cell initialTail = game.Segments[^1];
Check(game.Food == new Cell(9, 6), "Food should start in the selected free cell.");
game.Step();
Check(game.Segments.Count == 4 && game.Segments[0] == new Cell(9, 6) &&
      game.Segments[^1] == initialTail, "Eating must add a head and retain the tail.");
Check(game.Food == new Cell(10, 6) && !game.Segments.Contains(game.Food.Value),
    "Food must respawn in an unoccupied cell.");
game.Step();
Check(game.Segments.Count == 5 && game.Segments[0] == new Cell(10, 6),
    "Each food eaten must grow the snake once.");
game.Turn(Direction.Up);
game.Step();
Check(game.Segments.Count == 5 && game.Segments[^1] != initialTail,
    "A move without food must keep the same length and advance the tail.");
game.Turn(Direction.Left);
game.Step();
game.Turn(Direction.Down);
game.Step();
Check(game.Status == GameStatus.Lost && game.Segments.Count == 5,
    "Moving into the body must end the game without changing the snake.");
Cell lostHead = game.Segments[0];
game.Step();
Check(game.Segments[0] == lostHead, "A finished game must not advance.");

SnakeGame turns = new(new ScriptedRandom(0));
turns.Turn(Direction.Left);
turns.Step();
Check(turns.Segments[0] == new Cell(9, 6), "The snake must not reverse direction.");
turns.Turn(Direction.Up);
turns.Turn(Direction.Left);
turns.Step();
Check(turns.Segments[0] == new Cell(9, 5), "Only one turn is allowed per step.");

SnakeGame tail = new(new ScriptedRandom(102, 0));
tail.Step();
tail.Turn(Direction.Up);
tail.Step();
tail.Turn(Direction.Left);
tail.Step();
tail.Turn(Direction.Down);
tail.Step();
Check(tail.Status == GameStatus.Playing && tail.Segments[0] == new Cell(8, 6) &&
      tail.Segments.Count == 4, "Moving into the vacated tail cell must be allowed.");

SnakeGame wall = new(new ScriptedRandom(0));
for (int i = 0; i < 8; i++)
{
    wall.Step();
}
Check(wall.Status == GameStatus.Lost && wall.Segments[0] == new Cell(15, 6),
    "Hitting a wall must end the game without moving outside the board.");

Console.WriteLine("Snake game checks passed.");

static void Check(bool condition, string message)
{
    if (!condition)
    {
        throw new Exception(message);
    }
}

sealed class ScriptedRandom(params int[] values) : Random
{
    private int index;

    public override int Next(int maxValue)
    {
        int value = values[index++];
        if (value < 0 || value >= maxValue)
        {
            throw new InvalidOperationException("Scripted food index is outside the available cells.");
        }
        return value;
    }
}
