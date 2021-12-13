using System.Drawing;

var points = new InputProvider<PaperPoint?>("Input.txt", GetPoint).Where(w => w != null).Cast<PaperPoint>().ToList();
var world = new FoldableWorld(points);
var printer = new WorldPrinter();


world.FoldX(655);

Console.WriteLine($"Part 1: {world.WorldObjects.Count()}");

world.FoldY(447);
world.FoldX(327);
world.FoldY(223);
world.FoldX(163);
world.FoldY(111);
world.FoldX(81);
world.FoldY(55);
world.FoldX(40);
world.FoldY(27);
world.FoldY(13);
world.FoldY(6);

printer.Print(world);
Console.ReadKey();

static bool GetPoint(string? input, out PaperPoint? value)
{
    value = null;

    if (input == null) return false;

    var numbers = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(w => int.Parse(w))
        .ToArray();

    value = new PaperPoint(numbers[0], numbers[1]);

    return true;
}

class PaperPoint : IWorldObject
{
    public Point Position { get; init; }

    public int X => this.Position.X;
    public int Y => this.Position.Y;
    public int Z => 0;

    public char CharRepresentation => '#';

    public PaperPoint(int x, int y)
    {
        this.Position = new Point(x, y);
    }
}

class FoldableWorld : IWorld
{
    public IEnumerable<IWorldObject> WorldObjects => this.objects;
    private readonly List<PaperPoint> objects = new();

    public FoldableWorld(IEnumerable<PaperPoint> worldObjects)
    {
        this.objects.AddRange(worldObjects);
    }

    public void FoldX(int x)
    {
        var right = this.objects.Where(w => w.X > x);

        var pointsToAdd = new List<PaperPoint>();

        foreach (var point in right)
        {
            var newX = x - (point.X - x);

            pointsToAdd.Add(new PaperPoint(newX, point.Y));
        }

        this.objects.AddRange(pointsToAdd.Where(w => !this.objects.Any(ww => w.X == ww.X && w.Y == ww.Y)));
        this.objects.RemoveAll(w => right.Contains(w));
    }

    public void FoldY(int y)
    {
        var above = this.objects.Where(w => w.Y > y);

        var pointsToAdd = new List<PaperPoint>();

        foreach (var point in above)
        {
            var newY = y - (point.Y - y);

            pointsToAdd.Add(new PaperPoint(point.X, newY));
        }

        this.objects.AddRange(pointsToAdd.Where(w => !this.objects.Any(ww => w.X == ww.X && w.Y == ww.Y)));
        this.objects.RemoveAll(w => above.Contains(w));
    }
}