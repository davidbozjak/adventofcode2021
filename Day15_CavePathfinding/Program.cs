using System.Drawing;

var input = new InputProvider<int[]>("Input.txt", ParseRow).ToList();
var factory = new UniqueFactory<(int x, int y, int risk), Cell>(w => new Cell(w.x, w.y, w.risk));

int height = input.Count;
int width = input[0].Length;

var startNode = factory.GetOrCreateInstance((0, 0, input[0][0]));

//part 1:
//var endNode = factory.GetOrCreateInstance((width - 1, height - 1, input[^1][^1]));

//part 2:
var endNode = factory.GetOrCreateInstance((5*width - 1, 5*height - 1, GetRiskForLocation(5 * width - 1, 5 * height - 1)));

var cheapestPath = FindPath(startNode, endNode, GetRiskHeuristic);

Console.WriteLine($"Part 1: {cheapestPath.Skip(1).Sum(w => w.Risk)}");

List<Cell>? FindPath(Cell start, Cell goal, Func<Cell, int> hFunc)
{
    var openSet = new List<Cell> { start };
    var cameFrom = new Dictionary<Cell, Cell>();
    var gScore = new Dictionary<Cell, int>
    {
        [start] = 0
    };

    var fScore = new Dictionary<Cell, int>
    {
        [start] = hFunc(start)
    };

    while (openSet.Count > 0)
    {
        var current = openSet.Where(w => fScore.ContainsKey(w)).OrderBy(w => fScore[w]).First();

        if (current == goal)
        {
            //reconstruct path!
            var path = new List<Cell>() { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Insert(0, current);
            }

            return path;
        }

        openSet.Remove(current);

        foreach (var neighbour in GetNeighbours(current))
        {
            var tentativeScore = gScore[current] + neighbour.Risk;

            var g = gScore.ContainsKey(neighbour) ? gScore[neighbour] : UInt16.MaxValue;
            if (tentativeScore < g)
            {
                cameFrom[neighbour] = current;
                gScore[neighbour] = tentativeScore;
                fScore[neighbour] = tentativeScore + hFunc(neighbour);

                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
            }
        }
    }

    return null;
}

IEnumerable<Cell> GetNeighbours(Cell c)
{
    int minX = 0;
    int minY = 0;

    //Part 1 limits: 
    //int maxX = width;
    //int maxY = height;

    //Part 2 limits:
    int maxX = 5 * width;
    int maxY = 5 * height;

    if (c.X - 1 >= minX) yield return factory.GetOrCreateInstance((c.X - 1, c.Y, GetRiskForLocation(c.X - 1, c.Y)));
    if (c.X + 1 < maxX) yield return factory.GetOrCreateInstance((c.X + 1, c.Y, GetRiskForLocation(c.X + 1, c.Y)));

    if (c.Y - 1 >= minY) yield return factory.GetOrCreateInstance((c.X, c.Y - 1, GetRiskForLocation(c.X, c.Y - 1)));
    if (c.Y + 1 < maxY) yield return factory.GetOrCreateInstance((c.X, c.Y + 1, GetRiskForLocation(c.X, c.Y + 1)));
}

int GetRiskForLocation(int x, int y)
{
    int additionX = 0;
    int additionY = 0;

    if (x >= width) additionX = x / width;
    if (y >= height) additionY = y / height;

    int value = input[y % height][x % width] + additionX + additionY;

    while (value > 9) value -= 9;

    return value;
}

int GetRiskHeuristic(Cell c)
{
    //return (Math.Abs(height - c.Y - 1) + Math.Abs(width - c.X - 1)) * 10;
    return 0; // always return 0 to get from A* to Dijkstra algorithm
}

static bool ParseRow(string? input, out int[] row)
{
    row = Array.Empty<int>();

    if (string.IsNullOrWhiteSpace(input)) return false;

    row = input.ToCharArray().Select(w => w - '0').ToArray();

    return true;
}

class Cell : IWorldObject
{
    public Point Position { get; init; }

    public char CharRepresentation => Risk.ToString()[0];

    public int X => this.Position.X;
    public int Y => this.Position.Y;

    public int Z => 0;

    public int Risk { get; init; }

    public Cell(int x, int y, int risk)
    {
        this.Position = new Point(x, y);
        this.Risk = risk;
    }
}