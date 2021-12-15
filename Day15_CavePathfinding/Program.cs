using System.Drawing;

var input = new InputProvider<int[]>("Input.txt", ParseRow).ToList();
var factory = new UniqueFactory<(int x, int y, int risk), Cell>(w => new Cell(w.x, w.y, w.risk));

int mapHeight = input.Count;
int mapWidth = input[0].Length;

int minX = 0;
int minY = 0;
int maxX = mapWidth;
int maxY = mapHeight;

var startNode = factory.GetOrCreateInstance((0, 0, input[0][0]));
var part1EndNode = factory.GetOrCreateInstance((mapWidth - 1, mapHeight - 1, input[^1][^1]));

var part1CheapestPath = AStarPathfinder.FindPath(startNode, part1EndNode, GetRiskHeuristic, GetNeighbours);

if (part1CheapestPath == null) throw new Exception();

Console.WriteLine($"Part 1: {part1CheapestPath.Skip(1).Sum(w => w.Cost)}");

maxX = 5 * mapWidth;
maxY = 5 * mapHeight;

var part2EndNode = factory.GetOrCreateInstance((5 * mapWidth - 1, 5 * mapHeight - 1, GetRiskForLocation(5 * mapWidth - 1, 5 * mapHeight - 1)));

var part2CheapestPath = AStarPathfinder.FindPath(startNode, part2EndNode, GetRiskHeuristic, GetNeighbours);

if (part2CheapestPath == null) throw new Exception();

Console.WriteLine($"Part 2: {part2CheapestPath.Skip(1).Sum(w => w.Cost)}");

IEnumerable<Cell> GetNeighbours(Cell c)
{
    if (c.X - 1 >= minX) yield return factory.GetOrCreateInstance((c.X - 1, c.Y, GetRiskForLocation(c.X - 1, c.Y)));
    if (c.X + 1 < maxX) yield return factory.GetOrCreateInstance((c.X + 1, c.Y, GetRiskForLocation(c.X + 1, c.Y)));

    if (c.Y - 1 >= minY) yield return factory.GetOrCreateInstance((c.X, c.Y - 1, GetRiskForLocation(c.X, c.Y - 1)));
    if (c.Y + 1 < maxY) yield return factory.GetOrCreateInstance((c.X, c.Y + 1, GetRiskForLocation(c.X, c.Y + 1)));
}

int GetRiskForLocation(int x, int y)
{
    int additionX = 0;
    int additionY = 0;

    if (x >= mapWidth) additionX = x / mapWidth;
    if (y >= mapHeight) additionY = y / mapHeight;

    int value = input[y % mapHeight][x % mapWidth] + additionX + additionY;

    while (value > 9) value -= 9;

    return value;
}

int GetRiskHeuristic(Cell c)
{
    return 0; // always return 0 to get from A* to Dijkstra algorithm
}

static bool ParseRow(string? input, out int[] row)
{
    row = Array.Empty<int>();

    if (string.IsNullOrWhiteSpace(input)) return false;

    row = input.ToCharArray().Select(w => w - '0').ToArray();

    return true;
}

class Cell : INode
{
    public Point Position { get; init; }

    public char CharRepresentation => Cost.ToString()[0];

    public int X => this.Position.X;
    public int Y => this.Position.Y;

    public int Z => 0;

    public int Cost { get; init; }

    public Cell(int x, int y, int risk)
    {
        this.Position = new Point(x, y);
        this.Cost = risk;
    }
}