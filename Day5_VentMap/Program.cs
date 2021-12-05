using System.Text.RegularExpressions;

var lines = new InputProvider<Line>("Input.txt", GetLine).ToList();

Console.WriteLine(lines.Count);

var verticalOrHorizontalLines = lines.Where(w => w.x1 == w.x2 || w.y1 == w.y2).ToList();
var diagnoalLines = lines.Where(w => !verticalOrHorizontalLines.Contains(w)).ToList();

int maxX = lines.Select(w => w.x1).Concat(lines.Select(w => w.x2)).Max();
int maxY = lines.Select(w => w.y1).Concat(lines.Select(w => w.y2)).Max();

var world = new int[maxX + 1, maxY + 1];

List<(int x, int y)> pointsMoreThan1 = new();

foreach (var line in verticalOrHorizontalLines)
{
    if (line.x1 == line.x2)
    {
        int increment = line.y1 < line.y2 ? 1 : -1;
        for (int y = line.y1; y != (line.y2 + increment); y += increment)
        {
            world[line.x1, y]++;

            if (world[line.x1, y] > 1 && !pointsMoreThan1.Contains((line.x1, y)))
                pointsMoreThan1.Add((line.x1, y));
        }
    }
    else
    {
        int increment = line.x1 < line.x2 ? 1 : -1;
        for (int x = line.x1; x != (line.x2 + increment); x += increment)
        {
            world[x, line.y1]++;

            if (world[x, line.y1] > 1 && !pointsMoreThan1.Contains((x, line.y1)))
                pointsMoreThan1.Add((x, line.y1));
        }
    }
}

Console.WriteLine($"Part 1: {pointsMoreThan1.Count}");

foreach (var line in diagnoalLines)
{
    int incrementY = line.y1 < line.y2 ? 1 : -1;
    int incrementX = line.x1 < line.x2 ? 1 : -1;

    for (int x = line.x1, y = line.y1; x != (line.x2 + incrementX) && y != (line.y2 + incrementY); x += incrementX, y += incrementY)
    {
        world[x, y]++;

        if (world[x, y] > 1 && !pointsMoreThan1.Contains((x, y)))
            pointsMoreThan1.Add((x, y));
    }
}

Console.WriteLine($"Part 2: {pointsMoreThan1.Count}");

static bool GetLine(string? input, out Line? value)
{
    Regex numRegex = new(@"\d+");

    value = null;

    if (input == null) return false;

    var numbers = numRegex.Matches(input)
        .Select(w => int.Parse(w.Value))
        .ToList();

    if (numbers.Count != 4) throw new Exception();

    value = new Line(numbers[0], numbers[1], numbers[2], numbers[3]);

    return true;
}

record Line (int x1, int y1, int x2, int y2);