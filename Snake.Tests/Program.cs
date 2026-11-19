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

SnakeGame game = new(new ScriptedRandom(101, 101, 0));
Check(game.Segments.SequenceEqual(new[]
{
    new Cell(8, 6), new Cell(7, 6), new Cell(6, 6), new Cell(5, 6)
}), "A new game must start with four contiguous blocks.");
Cell initialTail = game.Segments[^1];
Check(game.Food == new Cell(9, 6), "Food should start in the selected free cell.");
game.Step();
Check(game.Segments.Count == 5 && game.Segments[0] == new Cell(9, 6) &&
      game.Segments[^1] == initialTail, "Eating must add a head and retain the tail.");
Check(game.Food == new Cell(10, 6) && !game.Segments.Contains(game.Food.Value),
    "Food must respawn in an unoccupied cell.");
game.Step();
Check(game.Segments.Count == 6 && game.Segments[0] == new Cell(10, 6),
    "Each food eaten must grow the snake once.");
game.Turn(Direction.Up);
game.Step();
Check(game.Segments.Count == 6 && game.Segments[^1] != initialTail,
    "A move without food must keep the same length and advance the tail.");
game.Turn(Direction.Left);
game.Step();
game.Turn(Direction.Down);
game.Step();
Check(game.Status == GameStatus.Lost && game.Segments.Count == 6,
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

SnakeGame tail = new(new ScriptedRandom(0));
tail.Turn(Direction.Up);
tail.Step();
tail.Turn(Direction.Right);
tail.Step();
tail.Turn(Direction.Down);
tail.Step();
tail.Turn(Direction.Left);
tail.Step();
Check(tail.Status == GameStatus.Playing && tail.Segments[0] == new Cell(8, 6) &&
      tail.Segments.Count == 4, "Moving into the vacated tail cell must be allowed.");

SnakeGame rightWall = new(new ScriptedRandom(96, 0));
for (int i = 0; i < 8; i++)
{
    rightWall.Step();
}
Check(rightWall.Status == GameStatus.Playing && rightWall.Segments[0] == new Cell(0, 6) &&
      rightWall.Segments.Count == 5 && rightWall.Food == new Cell(0, 0),
    "Crossing the right wall must wrap and eat food on the opposite side.");
rightWall.Step();
Check(rightWall.Segments[0] == new Cell(1, 6) && rightWall.Segments.Count == 5,
    "The snake must continue moving normally after wrapping.");

SnakeGame leftWall = new(new ScriptedRandom(0));
leftWall.Turn(Direction.Up);
leftWall.Step();
leftWall.Turn(Direction.Left);
for (int i = 0; i < 9; i++)
{
    leftWall.Step();
}
Check(leftWall.Status == GameStatus.Playing && leftWall.Segments[0] == new Cell(15, 5),
    "Crossing the left wall must wrap to the right side.");

SnakeGame topWall = new(new ScriptedRandom(0));
topWall.Turn(Direction.Up);
for (int i = 0; i < 7; i++)
{
    topWall.Step();
}
Check(topWall.Status == GameStatus.Playing && topWall.Segments[0] == new Cell(8, 11),
    "Crossing the top wall must wrap to the bottom.");

SnakeGame bottomWall = new(new ScriptedRandom(0));
bottomWall.Turn(Direction.Down);
for (int i = 0; i < 6; i++)
{
    bottomWall.Step();
}
Check(bottomWall.Status == GameStatus.Playing && bottomWall.Segments[0] == new Cell(8, 0),
    "Crossing the bottom wall must wrap to the top.");

SnakeGame wrappedCollision = new(new ScriptedRandom(101, 101, 0));
for (int i = 0; i < 6; i++)
{
    wrappedCollision.Step();
}
wrappedCollision.Turn(Direction.Up);
wrappedCollision.Step();
wrappedCollision.Turn(Direction.Right);
wrappedCollision.Step();
wrappedCollision.Turn(Direction.Down);
wrappedCollision.Step();
wrappedCollision.Turn(Direction.Right);
wrappedCollision.Step();
wrappedCollision.Turn(Direction.Up);
wrappedCollision.Step();
wrappedCollision.Turn(Direction.Left);
wrappedCollision.Step();
Check(wrappedCollision.Status == GameStatus.Lost && wrappedCollision.Segments[0] == new Cell(0, 5),
    "Wrapping into the snake's body must end the game without moving.");

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
