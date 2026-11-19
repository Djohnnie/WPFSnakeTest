namespace Snake;

public readonly record struct Cell(int X, int Y);

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public enum GameStatus
{
    Playing,
    Lost,
    Won
}

public sealed class SnakeGame
{
    public const int Width = 16;
    public const int Height = 12;

    private readonly List<Cell> segments = new();
    private readonly Random random;
    private Direction direction = Direction.Right;
    private bool turnPending;

    public SnakeGame(Random? random = null)
    {
        this.random = random ?? Random.Shared;
        segments.Add(new Cell(Width / 2, Height / 2));
        segments.Add(new Cell(Width / 2 - 1, Height / 2));
        segments.Add(new Cell(Width / 2 - 2, Height / 2));
        segments.Add(new Cell(Width / 2 - 3, Height / 2));
        Segments = segments.AsReadOnly();
        PlaceFood();
    }

    public IReadOnlyList<Cell> Segments { get; }
    public Cell? Food { get; private set; }
    public GameStatus Status { get; private set; } = GameStatus.Playing;

    public void Turn(Direction next)
    {
        if (Status != GameStatus.Playing || turnPending ||
            !Enum.IsDefined(next) || next == direction || IsOpposite(direction, next))
        {
            return;
        }

        direction = next;
        turnPending = true;
    }

    public void Step()
    {
        if (Status != GameStatus.Playing)
        {
            return;
        }

        Cell head = segments[0];
        Cell next = direction switch
        {
            Direction.Up => head with { Y = head.Y - 1 },
            Direction.Down => head with { Y = head.Y + 1 },
            Direction.Left => head with { X = head.X - 1 },
            Direction.Right => head with { X = head.X + 1 },
            _ => throw new InvalidOperationException("Invalid direction.")
        };
        next = new Cell((next.X + Width) % Width, (next.Y + Height) % Height);
        bool eating = next == Food;
        int occupiedCount = segments.Count - (eating ? 0 : 1);
        if (segments.Take(occupiedCount).Contains(next))
        {
            Status = GameStatus.Lost;
            return;
        }

        segments.Insert(0, next);
        if (eating)
        {
            PlaceFood();
        }
        else
        {
            segments.RemoveAt(segments.Count - 1);
        }

        turnPending = false;
    }

    private static bool IsOpposite(Direction current, Direction next) =>
        (current == Direction.Up && next == Direction.Down) ||
        (current == Direction.Down && next == Direction.Up) ||
        (current == Direction.Left && next == Direction.Right) ||
        (current == Direction.Right && next == Direction.Left);

    private void PlaceFood()
    {
        int freeCount = Width * Height - segments.Count;
        if (freeCount == 0)
        {
            Food = null;
            Status = GameStatus.Won;
            return;
        }

        int selected = random.Next(freeCount);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Cell cell = new(x, y);
                if (!segments.Contains(cell) && selected-- == 0)
                {
                    Food = cell;
                    return;
                }
            }
        }

        throw new InvalidOperationException("Unable to place food.");
    }
}
