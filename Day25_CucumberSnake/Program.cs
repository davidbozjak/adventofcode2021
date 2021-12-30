using System.Drawing;

var inputLines = new InputProvider<string>("Input.txt", GetString).ToList();

var world = new WrappedWorld(inputLines[0].Length, inputLines.Count);
var cucumbers = new List<SeaCucumber>();

for(int y = 0; y < inputLines.Count; y++)
{
    for (int x = 0; x < inputLines[0].Length; x++)
    {
        var spot = inputLines[y][x];

        if (spot == '.') continue;

        cucumbers.Add(new SeaCucumber(spot == 'v', x, y, world));
    }
}

var eastHeadingHeard = cucumbers.Where(w => !w.HeadingVerical).ToList();
var southHeadingHeard = cucumbers.Where(w => w.HeadingVerical).ToList();

int step = 0;
for (int totalMoves = 1; totalMoves > 0; step++)
{
    world.PrepareForMove();
    var eastHeardMovers = eastHeadingHeard.Where(w => w.CanMove()).ToList();

    eastHeardMovers.ForEach(w => w.Move());

    world.PrepareForMove();
    var southHeardMovers = southHeadingHeard.Where(w => w.CanMove()).ToList();

    southHeardMovers.ForEach(w => w.Move());

    totalMoves = eastHeardMovers.Count + southHeardMovers.Count;
}

Console.WriteLine($"Deadlock state after {step} steps");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

class SeaCucumber : IWorldObject
{
    public Point Position => new Point(this.X, this.Y);

    public char CharRepresentation => this.HeadingVerical ? 'v' : '>';

    public int X { get; private set; }

    public int Y { get; private set; }

    public int Z => 0;

    public bool HeadingVerical { get; }

    private readonly WrappedWorld world;

    public SeaCucumber(bool headingVertical, int initialX, int initialY, WrappedWorld world)
    {
        this.HeadingVerical = headingVertical;
        this.X = initialX;
        this.Y = initialY;
        this.world = world;
        this.world.RegisterCucumber(this);
    }

    public bool CanMove()
    {
        (int newX, int newY) = NextPosition();
        return !this.world.IsOccupied(newX, newY);
    }

    public void Move()
    {
        (this.X, this.Y) = NextPosition();
    }

    private (int x, int y) NextPosition()
    {
        if (this.HeadingVerical)
        {
            return (this.X, this.world.WrapY(this.Y + 1));
        }
        else
        {
            return (this.world.WrapX(this.X + 1), this.Y);
        }
    }
}

class WrappedWorld : IWorld
{
    private readonly List<SeaCucumber> cucumbers = new();
    private readonly Cached<IReadOnlyCollection<IWorldObject>> cachedReadOnlyWrapper;
    private readonly Cached<HashSet<(int x, int y)>> cachedOccupiedSpots;
    public IEnumerable<IWorldObject> WorldObjects => this.cachedReadOnlyWrapper.Value;

    public int Width { get; }
    public int Height { get; }

    public WrappedWorld(int width, int height)
    {
        this.Width = width;
        this.Height = height;

        this.cachedReadOnlyWrapper = new Cached<IReadOnlyCollection<IWorldObject>>(() => this.cucumbers.AsReadOnly());
        this.cachedOccupiedSpots = new(() => cucumbers.Select(w => (w.X, w.Y)).ToHashSet());
    }

    public void RegisterCucumber(SeaCucumber cucumber)
    {
        this.cucumbers.Add(cucumber);
    }

    public void PrepareForMove()
    {
        this.cachedOccupiedSpots.Reset();
    }

    public bool IsOccupied(int x, int y)
    {
        if (x < 0 || x >= Width) throw new Exception();
        if (y < 0 || y >= Height) throw new Exception();

        return this.cachedOccupiedSpots.Value.Contains((x, y));
    }

    public int WrapX(int x)
    {
        return x >= this.Width ? 0 : x;
    }

    public int WrapY(int y)
    {
        return y >= this.Height ? 0 : y;
    }
}